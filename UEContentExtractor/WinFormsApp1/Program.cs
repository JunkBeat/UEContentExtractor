using System;
using System.Windows.Forms;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace UEContentExtractor;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Setup UI
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Create and run Form
        var mainForm = new MainForm();
        Application.Run(mainForm);
    }
}
