
using Serilog.Core;
using Serilog.Events;
using System;
using System.Windows.Forms;

namespace UEContentExtractor;

public class RichTextBoxSink(RichTextBox richTextBox, IFormatProvider? formatProvider = null) : ILogEventSink
{
    private readonly RichTextBox _richTextBox = richTextBox;
    private readonly IFormatProvider? _formatProvider = formatProvider;

    public void Emit(LogEvent logEvent)
    {
        string message = logEvent.RenderMessage(_formatProvider);

        // Thread-safe call
        if (_richTextBox.InvokeRequired)
        {
            _richTextBox.Invoke(new Action(() => AppendLog(logEvent, message)));
        }
        else
        {
            AppendLog(logEvent, message);
        }
    }

    private void AppendLog(LogEvent logEvent, string message)
    {
        _richTextBox.SelectionColor = logEvent.Level switch
        {
            LogEventLevel.Information => System.Drawing.Color.White,
            LogEventLevel.Warning => System.Drawing.Color.DarkOrange,
            LogEventLevel.Error => System.Drawing.Color.Red,
            LogEventLevel.Debug => System.Drawing.Color.Gray,
            LogEventLevel.Fatal => System.Drawing.Color.DarkRed,
            _ => System.Drawing.Color.White
        };

        _richTextBox.AppendText($"{logEvent.Timestamp:HH:mm:ss} [{logEvent.Level}] {message}{Environment.NewLine}");
        _richTextBox.ScrollToCaret();
    }
}
