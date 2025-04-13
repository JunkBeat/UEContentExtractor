using CUE4Parse.UE4.Versions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace UEContentExtractor;

[Flags]
public enum ExportType
{
    None        = 0,
    Texture     = 1 << 0,
    Sound       = 1 << 1,
    Mesh        = 1 << 2,
    Animation   = 1 << 3,
    Font        = 1 << 4,
}

public class Settings(string archiveDir, EGame UEVersion)
{
    public string ProjectName { get; set; } = String.Empty;
    public string ArchiveDir { get; set; } = archiveDir;
    public string AesKey { get; set; } = String.Empty;
    public string UsmapPath { get; set; } = String.Empty;
    public EGame UEVersion { get; set; } = UEVersion;
    public ExportSettings ExportSettings { get; set; } = new ExportSettings();
}

public struct ExportSettings
{
    public bool ExportAudio;
    public bool ExportTexture;
    public bool ExportFont;
    public bool ExportMesh;
    public bool ExportAnimation;

    public readonly bool AnySelected =>
        ExportAudio || ExportTexture || ExportFont || ExportMesh || ExportAnimation;

    public ExportSettings()
    {
        ExportAudio = ExportTexture = ExportFont = ExportMesh = ExportAnimation = false;
    }

    public ExportType ToExportType()
    {
        ExportType type = ExportType.None;

        if (ExportAudio)     type |= ExportType.Sound;
        if (ExportTexture)   type |= ExportType.Texture;
        if (ExportFont)      type |= ExportType.Font;
        if (ExportMesh)      type |= ExportType.Mesh;
        if (ExportAnimation) type |= ExportType.Animation;

        return type;
    }
}

public static class Serializer
{
    public static void SerializeToFile<T>(T obj, string filePath)
    {
        try
        {
            var json = JsonSerializer.Serialize(obj);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while serializing: " + ex.Message);
        }
    }

    public static T? DeserializeFromFile<T>(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(json);
            }
            return default;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while serializing: " + ex.Message);
            return default;
        }
    }
}
