using CUE4Parse.UE4.Versions;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UEContentExtractor.Properties;

namespace UEContentExtractor
{
    public partial class MainForm : Form
    {
        private const string SettingsFile = "settings.json";
        private Settings? _settings = null;
        private string? _titleText = String.Empty;

        public MainForm()
        {
            InitializeComponent();
            _titleText = this.Text;
            comboBoxVersion.DataSource = Enum.GetValues(typeof(EGame));
            comboBoxVersion.SelectedItem = EGame.GAME_UE5_1;

            statusLabel.Spring = true;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            progressBar.Alignment = ToolStripItemAlignment.Right;

            UpdateButtonPanelMargin();
            buttonPanel.Resize += (s, e) => UpdateButtonPanelMargin();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Sink(new RichTextBoxSink(logBox))
                .CreateLogger();

            this.FormClosing += MainForm_FormClosing;
            this.Load += MainForm_Load;
        }

        private void UpdateButtonPanelMargin()
        {
            innerButtonPanel.Margin = new Padding((buttonPanel.Width - innerButtonPanel.Width) / 2, 3, 3, 3);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
            mainTable = new TableLayoutPanel();
            textBoxUsmap = new TextBox();
            textBoxPath = new TextBox();
            exportOptionsPanel = new FlowLayoutPanel();
            chBoxExportAudio = new CheckBox();
            chBoxExportTexture = new CheckBox();
            chBoxExportFont = new CheckBox();
            chBoxExportMesh = new CheckBox();
            chBoxExportAnimation = new CheckBox();
            textBoxAES = new TextBox();
            btnBrowsePath = new Button();
            btnBrowseUsmap = new Button();
            label5 = new Label();
            label2 = new Label();
            label1 = new Label();
            comboBoxVersion = new ComboBox();
            label3 = new Label();
            label4 = new Label();
            logBox = new RichTextBox();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            progressBar = new ToolStripProgressBar();
            btnAbort = new Button();
            btnStart = new Button();
            buttonPanel = new FlowLayoutPanel();
            innerButtonPanel = new FlowLayoutPanel();
            mainTable.SuspendLayout();
            exportOptionsPanel.SuspendLayout();
            statusStrip.SuspendLayout();
            buttonPanel.SuspendLayout();
            innerButtonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainTable
            // 
            mainTable.AutoSize = true;
            mainTable.ColumnCount = 3;
            mainTable.ColumnStyles.Add(new ColumnStyle());
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTable.ColumnStyles.Add(new ColumnStyle());
            mainTable.Controls.Add(textBoxUsmap, 1, 3);
            mainTable.Controls.Add(textBoxPath, 1, 0);
            mainTable.Controls.Add(exportOptionsPanel, 1, 4);
            mainTable.Controls.Add(textBoxAES, 1, 2);
            mainTable.Controls.Add(btnBrowsePath, 2, 0);
            mainTable.Controls.Add(btnBrowseUsmap, 2, 3);
            mainTable.Controls.Add(label5, 0, 4);
            mainTable.Controls.Add(label2, 0, 1);
            mainTable.Controls.Add(label1, 0, 0);
            mainTable.Controls.Add(comboBoxVersion, 1, 1);
            mainTable.Controls.Add(label3, 0, 2);
            mainTable.Controls.Add(label4, 0, 3);
            mainTable.Dock = DockStyle.Top;
            mainTable.Location = new Point(0, 0);
            mainTable.Name = "mainTable";
            mainTable.Padding = new Padding(10);
            mainTable.RowCount = 5;
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 51.38889F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 48.61111F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 89F));
            mainTable.Size = new Size(561, 252);
            mainTable.TabIndex = 0;
            // 
            // textBoxUsmap
            // 
            textBoxUsmap.Dock = DockStyle.Fill;
            textBoxUsmap.Location = new Point(101, 115);
            textBoxUsmap.Name = "textBoxUsmap";
            textBoxUsmap.PlaceholderText = "(if packages has unversioned properties)";
            textBoxUsmap.ReadOnly = true;
            textBoxUsmap.Size = new Size(400, 27);
            textBoxUsmap.TabIndex = 6;
            // 
            // textBoxPath
            // 
            textBoxPath.Dock = DockStyle.Fill;
            textBoxPath.Location = new Point(101, 13);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.ReadOnly = true;
            textBoxPath.Size = new Size(400, 27);
            textBoxPath.TabIndex = 8;
            // 
            // exportOptionsPanel
            // 
            mainTable.SetColumnSpan(exportOptionsPanel, 2);
            exportOptionsPanel.Controls.Add(chBoxExportAudio);
            exportOptionsPanel.Controls.Add(chBoxExportTexture);
            exportOptionsPanel.Controls.Add(chBoxExportFont);
            exportOptionsPanel.Controls.Add(chBoxExportMesh);
            exportOptionsPanel.Controls.Add(chBoxExportAnimation);
            exportOptionsPanel.Dock = DockStyle.Fill;
            exportOptionsPanel.FlowDirection = FlowDirection.TopDown;
            exportOptionsPanel.Location = new Point(101, 155);
            exportOptionsPanel.MinimumSize = new Size(0, 90);
            exportOptionsPanel.Name = "exportOptionsPanel";
            exportOptionsPanel.Size = new Size(447, 90);
            exportOptionsPanel.TabIndex = 1;
            // 
            // chBoxExportAudio
            // 
            chBoxExportAudio.AutoSize = true;
            chBoxExportAudio.Location = new Point(3, 3);
            chBoxExportAudio.Name = "chBoxExportAudio";
            chBoxExportAudio.Size = new Size(71, 24);
            chBoxExportAudio.TabIndex = 2;
            chBoxExportAudio.Text = "Audio";
            chBoxExportAudio.UseVisualStyleBackColor = true;
            // 
            // chBoxExportTexture
            // 
            chBoxExportTexture.AutoSize = true;
            chBoxExportTexture.Location = new Point(3, 33);
            chBoxExportTexture.Name = "chBoxExportTexture";
            chBoxExportTexture.Size = new Size(79, 24);
            chBoxExportTexture.TabIndex = 2;
            chBoxExportTexture.Text = "Texture";
            chBoxExportTexture.UseVisualStyleBackColor = true;
            // 
            // chBoxExportFont
            // 
            chBoxExportFont.AutoSize = true;
            chBoxExportFont.Enabled = false;
            chBoxExportFont.Location = new Point(3, 63);
            chBoxExportFont.Name = "chBoxExportFont";
            chBoxExportFont.Size = new Size(131, 24);
            chBoxExportFont.TabIndex = 2;
            chBoxExportFont.Text = "Font (disabled)";
            chBoxExportFont.UseVisualStyleBackColor = true;
            // 
            // chBoxExportMesh
            // 
            chBoxExportMesh.AutoSize = true;
            chBoxExportMesh.Location = new Point(140, 3);
            chBoxExportMesh.Name = "chBoxExportMesh";
            chBoxExportMesh.Size = new Size(66, 24);
            chBoxExportMesh.TabIndex = 3;
            chBoxExportMesh.Text = "Mesh";
            chBoxExportMesh.UseVisualStyleBackColor = true;
            // 
            // chBoxExportAnimation
            // 
            chBoxExportAnimation.AutoSize = true;
            chBoxExportAnimation.Location = new Point(140, 33);
            chBoxExportAnimation.Name = "chBoxExportAnimation";
            chBoxExportAnimation.Size = new Size(100, 24);
            chBoxExportAnimation.TabIndex = 4;
            chBoxExportAnimation.Text = "Animation";
            chBoxExportAnimation.UseVisualStyleBackColor = true;
            // 
            // textBoxAES
            // 
            mainTable.SetColumnSpan(textBoxAES, 2);
            textBoxAES.Dock = DockStyle.Fill;
            textBoxAES.Location = new Point(101, 82);
            textBoxAES.Name = "textBoxAES";
            textBoxAES.PlaceholderText = "(if the game is encrypted)";
            textBoxAES.Size = new Size(447, 27);
            textBoxAES.TabIndex = 1;
            // 
            // btnBrowsePath
            // 
            btnBrowsePath.Location = new Point(507, 13);
            btnBrowsePath.Name = "btnBrowsePath";
            btnBrowsePath.Size = new Size(41, 29);
            btnBrowsePath.TabIndex = 7;
            btnBrowsePath.Text = "...";
            btnBrowsePath.UseVisualStyleBackColor = true;
            btnBrowsePath.Click += BtnBrowsePath_Click;
            // 
            // btnBrowseUsmap
            // 
            btnBrowseUsmap.Location = new Point(507, 115);
            btnBrowseUsmap.Name = "btnBrowseUsmap";
            btnBrowseUsmap.Size = new Size(41, 29);
            btnBrowseUsmap.TabIndex = 5;
            btnBrowseUsmap.Text = "...";
            btnBrowseUsmap.UseVisualStyleBackColor = true;
            btnBrowseUsmap.Click += BtnBrowseUsmap_Click;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Location = new Point(13, 187);
            label5.Name = "label5";
            label5.Size = new Size(52, 20);
            label5.TabIndex = 1;
            label5.Text = "Export";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(13, 52);
            label2.Name = "label2";
            label2.Size = new Size(79, 20);
            label2.TabIndex = 1;
            label2.Text = "UE Version";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(13, 17);
            label1.Name = "label1";
            label1.Size = new Size(70, 20);
            label1.TabIndex = 1;
            label1.Text = "Directory";
            // 
            // comboBoxVersion
            // 
            mainTable.SetColumnSpan(comboBoxVersion, 2);
            comboBoxVersion.Dock = DockStyle.Fill;
            comboBoxVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxVersion.FormattingEnabled = true;
            comboBoxVersion.Items.AddRange(new object[] { EGame.GAME_UE4_0, EGame.GAME_UE4_1, EGame.GAME_UE4_2, EGame.GAME_UE4_3, EGame.GAME_UE4_4, EGame.GAME_UE4_5, EGame.GAME_ArkSurvivalEvolved, EGame.GAME_UE4_6, EGame.GAME_UE4_7, EGame.GAME_UE4_8, EGame.GAME_UE4_9, EGame.GAME_UE4_10, EGame.GAME_SeaOfThieves, EGame.GAME_UE4_11, EGame.GAME_GearsOfWar4, EGame.GAME_UE4_12, EGame.GAME_UE4_13, EGame.GAME_StateOfDecay2, EGame.GAME_UE4_14, EGame.GAME_TEKKEN7, EGame.GAME_UE4_15, EGame.GAME_UE4_16, EGame.GAME_PlayerUnknownsBattlegrounds, EGame.GAME_TrainSimWorld2020, EGame.GAME_UE4_17, EGame.GAME_AWayOut, EGame.GAME_UE4_18, EGame.GAME_KingdomHearts3, EGame.GAME_FinalFantasy7Remake, EGame.GAME_AceCombat7, EGame.GAME_FridayThe13th, EGame.GAME_GameForPeace, EGame.GAME_UE4_19, EGame.GAME_Paragon, EGame.GAME_UE4_20, EGame.GAME_Borderlands3, EGame.GAME_UE4_21, EGame.GAME_StarWarsJediFallenOrder, EGame.GAME_Undawn, EGame.GAME_UE4_22, EGame.GAME_UE4_23, EGame.GAME_ApexLegendsMobile, EGame.GAME_UE4_24, EGame.GAME_TonyHawkProSkater12, EGame.GAME_UE4_25, EGame.GAME_UE4_25_Plus, EGame.GAME_RogueCompany, EGame.GAME_DeadIsland2, EGame.GAME_KenaBridgeofSpirits, EGame.GAME_Strinova, EGame.GAME_SYNCED, EGame.GAME_OperationApocalypse, EGame.GAME_Farlight84, EGame.GAME_StarWarsHunters, EGame.GAME_UE4_26, EGame.GAME_GTATheTrilogyDefinitiveEdition, EGame.GAME_ReadyOrNot, EGame.GAME_BladeAndSoul, EGame.GAME_TowerOfFantasy, EGame.GAME_FinalFantasy7Rebirth, EGame.GAME_TheDivisionResurgence, EGame.GAME_StarWarsJediSurvivor, EGame.GAME_Snowbreak, EGame.GAME_TorchlightInfinite, EGame.GAME_QQ, EGame.GAME_WutheringWaves, EGame.GAME_DreamStar, EGame.GAME_MidnightSuns, EGame.GAME_FragPunk, EGame.GAME_RacingMaster, EGame.GAME_UE4_27, EGame.GAME_Splitgate, EGame.GAME_HYENAS, EGame.GAME_HogwartsLegacy, EGame.GAME_OutlastTrials, EGame.GAME_Valorant, EGame.GAME_Gollum, EGame.GAME_Grounded, EGame.GAME_DeltaForceHawkOps, EGame.GAME_MortalKombat1, EGame.GAME_VisionsofMana, EGame.GAME_Spectre, EGame.GAME_KartRiderDrift, EGame.GAME_ThroneAndLiberty, EGame.GAME_UE4_28, EGame.GAME_UE4_28, EGame.GAME_UE5_0, EGame.GAME_MeetYourMaker, EGame.GAME_BlackMythWukong, EGame.GAME_UE5_1, EGame.GAME_3on3FreeStyleRebound, EGame.GAME_Stalker2, EGame.GAME_TheCastingofFrankStone, EGame.GAME_SilentHill2Remake, EGame.GAME_UE5_2, EGame.GAME_DeadByDaylight, EGame.GAME_PaxDei, EGame.GAME_TheFirstDescendant, EGame.GAME_MetroAwakening, EGame.GAME_UE5_3, EGame.GAME_MarvelRivals, EGame.GAME_WildAssault, EGame.GAME_NobodyWantsToDie, EGame.GAME_MonsterJamShowdown, EGame.GAME_Rennsport, EGame.GAME_AshesOfCreation, EGame.GAME_Avowed, EGame.GAME_UE5_4, EGame.GAME_FunkoFusion, EGame.GAME_InfinityNikki, EGame.GAME_NevernessToEverness, EGame.GAME_Gothic1Remake, EGame.GAME_UE5_5, EGame.GAME_UE5_6, EGame.GAME_UE5_6 });
            comboBoxVersion.Location = new Point(101, 48);
            comboBoxVersion.Name = "comboBoxVersion";
            comboBoxVersion.Size = new Size(447, 28);
            comboBoxVersion.TabIndex = 2;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new Point(13, 85);
            label3.Name = "label3";
            label3.Size = new Size(63, 20);
            label3.TabIndex = 3;
            label3.Text = "AES Key";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(13, 122);
            label4.Name = "label4";
            label4.Size = new Size(82, 20);
            label4.TabIndex = 4;
            label4.Text = "Usmap File";
            // 
            // logBox
            // 
            logBox.BackColor = SystemColors.InfoText;
            logBox.Dock = DockStyle.Top;
            logBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            logBox.ForeColor = SystemColors.Info;
            logBox.Location = new Point(0, 319);
            logBox.Name = "logBox";
            logBox.ReadOnly = true;
            logBox.Size = new Size(561, 261);
            logBox.TabIndex = 2;
            logBox.Text = "";
            logBox.WordWrap = false;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, progressBar });
            statusStrip.Location = new Point(0, 575);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(561, 28);
            statusStrip.TabIndex = 3;
            statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(50, 22);
            statusLabel.Text = "Ready";
            // 
            // progressBar
            // 
            progressBar.AutoToolTip = true;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(200, 20);
            // 
            // btnAbort
            // 
            btnAbort.Enabled = false;
            btnAbort.Location = new Point(116, 3);
            btnAbort.Name = "btnAbort";
            btnAbort.Size = new Size(100, 35);
            btnAbort.TabIndex = 1;
            btnAbort.Text = "Abort";
            btnAbort.UseVisualStyleBackColor = true;
            btnAbort.Click += BtnAbort_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(3, 3);
            btnStart.Margin = new Padding(3, 3, 10, 3);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(100, 35);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += BtnStart_Click;
            // 
            // buttonPanel
            // 
            buttonPanel.AutoSize = true;
            buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonPanel.Controls.Add(innerButtonPanel);
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.FlowDirection = FlowDirection.TopDown;
            buttonPanel.Location = new Point(0, 252);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Padding = new Padding(10);
            buttonPanel.Size = new Size(561, 67);
            buttonPanel.TabIndex = 1;
            // 
            // innerButtonPanel
            // 
            innerButtonPanel.AutoSize = true;
            innerButtonPanel.Controls.Add(btnStart);
            innerButtonPanel.Controls.Add(btnAbort);
            innerButtonPanel.Location = new Point(13, 13);
            innerButtonPanel.Name = "innerButtonPanel";
            innerButtonPanel.Size = new Size(219, 41);
            innerButtonPanel.TabIndex = 2;
            // 
            // MainForm
            // 
            ClientSize = new Size(561, 603);
            Controls.Add(statusStrip);
            Controls.Add(logBox);
            Controls.Add(buttonPanel);
            Controls.Add(mainTable);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(440, 576);
            Name = "MainForm";
            Text = "Unreal Content Extractor";
            mainTable.ResumeLayout(false);
            mainTable.PerformLayout();
            exportOptionsPanel.ResumeLayout(false);
            exportOptionsPanel.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            buttonPanel.ResumeLayout(false);
            buttonPanel.PerformLayout();
            innerButtonPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private async void BtnStart_Click(object? sender, EventArgs? e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
                return;
            }

            _cts = new CancellationTokenSource();

            try
            {
                if (!PrepareSettings())
                    return;

                logBox.Clear();

                btnAbort.Enabled = true;
                btnStart.Enabled = false;

                await Task.Run(() =>
                {
                    try
                    {
                        Exporter.Run(
                            _settings!,
                            _cts.Token,
                            status => Invoke(() => statusLabel.Text = status),
                            progress => Invoke(() => progressBar.Value = progress),
                            projectName => Invoke(() => this.Text = $"{_titleText} - {projectName}")
                        );
                    }
                    catch (OperationCanceledException)
                    {
                        Log.Warning("Process was canceled.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occurred during export.");
                    }
                })
                .ContinueWith(t =>
                {
                    btnAbort.Text = "Abort";
                    btnStart.Enabled = true;
                    progressBar.Value = 0;

                    if (_cts.Token.IsCancellationRequested)
                    {
                        statusLabel.Text = "Cancelled";
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            finally
            {
                _cts = null;
            }
        }

        private bool PrepareSettings(bool disableGUIChecks = false)
        {
            var dir = textBoxPath.Text;
            var aes = string.IsNullOrWhiteSpace(textBoxAES.Text) ? String.Empty : textBoxAES.Text;
            var usmap = string.IsNullOrWhiteSpace(textBoxUsmap.Text) ? String.Empty : textBoxUsmap.Text;
            var ueVersion = (EGame)comboBoxVersion.SelectedItem!;

            var exportSettings = new ExportSettings
            {
                ExportAudio = chBoxExportAudio.Checked,
                ExportTexture = chBoxExportTexture.Checked,
                ExportFont = chBoxExportFont.Checked,
                ExportMesh = chBoxExportMesh.Checked,
                ExportAnimation = chBoxExportAnimation.Checked
            };

            if (!disableGUIChecks)
            {
                if (string.IsNullOrWhiteSpace(dir))
                {
                    MessageBox.Show("Game folder not selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!exportSettings.AnySelected)
                {
                    MessageBox.Show("Select at least 1 export type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            _settings = new Settings(dir, ueVersion)
            {
                AesKey = aes,
                UsmapPath = usmap,
                ExportSettings = exportSettings
            };

            return true;
        }

        private void MainForm_Load(object? sender, EventArgs? e)
        {
            var settings = Serializer.DeserializeFromFile<Settings>(SettingsFile);
            if (settings != null)
            {
                textBoxPath.Text = settings.ArchiveDir;
                textBoxAES.Text = settings.AesKey;
                textBoxUsmap.Text = settings.UsmapPath;
                comboBoxVersion.SelectedItem = settings.UEVersion;

                if (settings.ProjectName != String.Empty)
                {
                    this.Text = $"{_titleText} - {settings.ProjectName}";
                }
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs? e)
        {
            if (_settings == null)
            {
                PrepareSettings(true);
            }

            Serializer.SerializeToFile(_settings, SettingsFile);
        }

        private void BtnAbort_Click(object? sender, EventArgs? e)
        {
            btnAbort.Enabled = false;
            btnAbort.Text = "Stopping...";
            _cts?.Cancel();
        }

        private void BtnBrowsePath_Click(object? sender, EventArgs? e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Paks folder";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void BtnBrowseUsmap_Click(object? sender, EventArgs? e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Select Mapping File";
                fileDialog.Filter = "USMAP File (*.usmap)|*.usmap";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxUsmap.Text = fileDialog.FileName;
                }
            }
        }
    }
}
