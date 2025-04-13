
namespace UEContentExtractor
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource _cts;
        private TableLayoutPanel mainTable;
        private FlowLayoutPanel exportOptionsPanel;
        private CheckBox chBoxExportAudio;
        private CheckBox chBoxExportTexture;
        private Label label5;
        private TextBox textBoxUsmap;
        private Button btnBrowseUsmap;
        private Label label2;
        private Label label1;
        private ComboBox comboBoxVersion;
        private Label label3;
        private TextBox textBoxAES;
        private Label label4;
        private TextBox textBoxPath;
        private Button btnBrowsePath;
        private CheckBox chBoxExportFont;
        private StatusStrip statusStrip;
        private Button btnAbort;
        private Button btnStart;
        private FlowLayoutPanel buttonPanel;
        private FlowLayoutPanel innerButtonPanel;
        private RichTextBox logBox;
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar progressBar;
        private CheckBox chBoxExportMesh;
        private CheckBox chBoxExportAnimation;
    }
}