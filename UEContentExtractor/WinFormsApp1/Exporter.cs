using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Threading;
using CUE4Parse_Conversion;
using CUE4Parse_Conversion.Animations;
using CUE4Parse_Conversion.Meshes;
using CUE4Parse_Conversion.Sounds;
using CUE4Parse_Conversion.Textures;
using CUE4Parse_Conversion.UEFormat.Enums;
using CUE4Parse.Compression;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports.Animation;
using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.SkeletalMesh;
using CUE4Parse.UE4.Assets.Exports.Sound;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Exports.Wwise;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;
using Serilog;
using CUE4Parse.UE4.Assets.Exports.Engine.Font;

namespace UEContentExtractor;

public static class Exporter
{
    private const string _exportDirectory = "./exports";
    private const string _pluginsDirectory = "./plugins";

    public static void Run(Settings settings, CancellationToken token, Action<string>? updateStatus = null,
    Action<int>? updateProgress = null, Action<string>? updateTitle = null)
    {
        var version = new VersionContainer(settings.UEVersion, ETexturePlatform.DesktopMobile);
        var provider = new DefaultFileProvider(settings.ArchiveDir, SearchOption.AllDirectories, version);
        var usmap = settings.UsmapPath;
        var aes = settings.AesKey;
        if (!string.IsNullOrEmpty(usmap))
        {
            provider.MappingsContainer = new FileUsmapTypeMappingsProvider(usmap);
        }

        provider.Initialize();
        if (!string.IsNullOrEmpty(aes))
        {
            provider.SubmitKey(new FGuid(), new FAesKey(aes));
        }
        else
        {
            provider.SubmitKey(new FGuid(), null);
        }

        provider.LoadVirtualPaths();
        provider.PostMount();

        string projectName = provider.ProjectName;

        Log.Information($"Export begins...");
        Log.Information($"Project: {projectName} | Mounted: {provider.MountedVfs.Count}");
        
        updateTitle?.Invoke(projectName);
        settings.ProjectName = projectName;

        var options = new ExporterOptions
        {
            LodFormat = ELodFormat.FirstLod,
            MeshFormat = EMeshFormat.UEFormat,
            AnimFormat = EAnimFormat.UEFormat,
            MaterialFormat = EMaterialFormat.AllLayersNoRef,
            TextureFormat = ETextureFormat.Png,
            CompressionFormat = EFileCompressionFormat.None,
            Platform = version.Platform,
            SocketFormat = ESocketFormat.Bone,
            ExportMorphTargets = true,
            ExportMaterials = false
        };

        ExportType type = settings.ExportSettings.ToExportType();

        ExportWithProvider(provider, options, type, token, updateStatus, updateProgress);

        Log.Information("Export completed.");
    }

    public static void ExportWithProvider(DefaultFileProvider provider, ExporterOptions options, ExportType type, CancellationToken token, Action<string>? updateStatus = null,
    Action<int>? updateProgress = null)
    {
        OodleHelper.DownloadOodleDll();
        OodleHelper.Initialize(OodleHelper.OODLE_DLL_NAME);

        var assets = provider.Files;
        var files = provider.Files.Values
            .GroupBy(it => it.Path.SubstringBeforeLast('/'))
            .ToDictionary(it => it.Key, it => it.ToArray());

        int totalCount = files.Sum(pair => pair.Value.Length);
        int scannedCount = 0;
        var exportCount = 0;
        var watch = new Stopwatch();
        watch.Start();

        foreach (var (folder, packages) in files)
        {
            Log.Information("scanning {Folder} ({Count} packages)", folder, packages.Length);

            Parallel.ForEach(packages, new ParallelOptions { CancellationToken = token }, package =>
            {
                updateStatus?.Invoke($"Scanned: {scannedCount}/{totalCount}, Exported: {exportCount}");
                updateProgress?.Invoke(scannedCount * 100 / totalCount);

                scannedCount++;

                if (!provider.TryLoadPackage(package, out var pkg)) return;

                // optimized way of checking for exports type without loading most of them
                for (var i = 0; i < pkg.ExportMapLength; i++)
                {
                    token.ThrowIfCancellationRequested();

                    var pointer = new FPackageIndex(pkg, i + 1).ResolvedObject;
                    if (pointer?.Object is null) continue;

                    var dummy = ((AbstractUePackage)pkg).ConstructObject(pointer.Class?.Object?.Value as UStruct, pkg);
                    switch (dummy)
                    {
                        //case UFontFace when type.HasFlag(ExportType.Font):
                        //{
                        //        break;
                        //}
                        case UTexture when type.HasFlag(ExportType.Texture) && pointer.Object.Value is UTexture texture:
                        {
                            try
                            {
                                Log.Information("{ExportType} found in {PackageName}", dummy.ExportType, package.Name);
                                SaveTexture(folder, texture, options.Platform, options, ref exportCount);
                            }
                            catch (Exception e)
                            {
                                Log.Warning(e, "failed to decode {TextureName}", texture.Name);
                                return;
                            }
                            break;
                        }
                        case USoundWave when type.HasFlag(ExportType.Sound):
                        case UAkMediaAssetData when type.HasFlag(ExportType.Sound):
                        {
                            Log.Information("{ExportType} found in {PackageName}", dummy.ExportType, package.Name);

                            pointer.Object.Value.Decode(true, out var format, out var bytes);
                            if (bytes is not null)
                            {
                                var fileName = $"{pointer.Object.Value.Name}.{format.ToLower()}";
                                WriteToFile(folder, fileName, bytes, fileName, ref exportCount);
                                ConvertAudioIfNeeded(Path.Combine(_exportDirectory, folder, fileName));
                            }

                            break;
                        }
                        case UAnimSequenceBase when type.HasFlag(ExportType.Animation):
                        case USkeletalMesh when type.HasFlag(ExportType.Mesh):
                        case UStaticMesh when type.HasFlag(ExportType.Mesh):
                        case USkeleton when type.HasFlag(ExportType.Mesh):
                        {
                            Log.Information("{ExportType} found in {PackageName}", dummy.ExportType, package.Name);

                            var exporter = new CUE4Parse_Conversion.Exporter(pointer.Object.Value, options);
                            if (exporter.TryWriteToDir(new DirectoryInfo(_exportDirectory), out _, out var filePath))
                            {
                                WriteToLog(folder, Path.GetFileName(filePath), ref exportCount);
                            }
                            break;
                        }
                    }
                }
            });
        }
        watch.Stop();

        Log.Information("exported {ExportCount} files ({Types}) in {Time}",
            exportCount,
            type.ToStringBitfield(),
            watch.Elapsed);

        if (exportCount > 0) 
        {
            updateStatus?.Invoke($"Successfully exported {exportCount} files");
        }
        else
        {
            updateStatus?.Invoke($"Nothing was exported");
        }
    }

    private static void SaveTexture(string folder, UTexture texture, ETexturePlatform platform, ExporterOptions options, ref int exportCount)
    {
        var bitmaps = new[] { texture.Decode(platform) };
        switch (texture)
        {
            case UTexture2DArray textureArray:
                bitmaps = textureArray.DecodeTextureArray(platform);
                break;
            case UTextureCube:
                bitmaps[0] = bitmaps[0]?.ToPanorama();
                break;
        }

        var extension = options.TextureFormat.ToString().ToLower();
        for (int i = 0; i < bitmaps?.Length; i++)
        {
            SkiaSharp.SKBitmap? bitmap = bitmaps[i];
            if (bitmap is null) continue;
            var bytes = bitmap.Encode(options.TextureFormat, 100).ToArray();
            var fileName = $"{texture.Name}.{extension}";

            WriteToFile(folder, fileName, bytes, $"{fileName} ({bitmap.Width}x{bitmap.Height})", ref exportCount);
        }
    }

    private static void WriteToFile(string folder, string fileName, byte[] bytes, string logMessage, ref int exportCount)
    {
        Directory.CreateDirectory(Path.Combine(_exportDirectory, folder));
        File.WriteAllBytesAsync(Path.Combine(_exportDirectory, folder, fileName), bytes).Wait();
        WriteToLog(folder, logMessage, ref exportCount);
    }

    private static void WriteToLog(string folder, string logMessage, ref int exportCount)
    {
        Log.Information("exported {LogMessage} out of {Folder}", logMessage, folder);
        exportCount++;
    }

    private static bool ConvertAudioIfNeeded(string filePath)
    {
        if (!File.Exists(filePath)) return false;
        //if (!File.Exists(filePath)) { throw new Exception($"{filePath} doesnt exist"); }
        string ext = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant();
        
        return ext switch
        {
            "adpcm" or "opus" or "wem" or "at9" or "raw" => TryConvertVgm(filePath, out _),
            "binka" => TryDecodeBinka(filePath, out _),
            _ => true
        };
    }

    private static bool TryConvertVgm(string inputFilePath, out string wavFilePath)
    {
        wavFilePath = string.Empty;

        string vgmFilePath = Path.Combine(_pluginsDirectory, "vgmstream", "vgmstream-cli.exe");
        if (!File.Exists(vgmFilePath)) return false;

        wavFilePath = Path.ChangeExtension(inputFilePath, ".wav");
        var vgmProcess = Process.Start(new ProcessStartInfo
        {
            FileName = vgmFilePath,
            Arguments = $"-o \"{wavFilePath}\" \"{inputFilePath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        });
        vgmProcess?.WaitForExit(5000);

        File.Delete(inputFilePath);
        return vgmProcess?.ExitCode == 0 && File.Exists(wavFilePath);
    }

    private static bool TryDecodeBinka(string inputFilePath, out string rawFilePath)
    {
        rawFilePath = string.Empty;

        var binkadecPath = Path.Combine(_pluginsDirectory, "binkadec.exe");
        if (!File.Exists(binkadecPath)) return false;

        rawFilePath = Path.ChangeExtension(inputFilePath, ".wav");
        var binkadecProcess = Process.Start(new ProcessStartInfo
        {
            FileName = binkadecPath,
            Arguments = $"-i \"{inputFilePath}\" -o \"{rawFilePath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        });
        binkadecProcess?.WaitForExit(5000);

        File.Delete(inputFilePath);
        return binkadecProcess?.ExitCode == 0 && File.Exists(rawFilePath);
    }
}
