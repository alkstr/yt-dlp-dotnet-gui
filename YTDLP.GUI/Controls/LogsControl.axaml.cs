using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using YTDLP.GUI.Utilities;

namespace YTDLP.GUI.Controls;

public class LogsControl : TemplatedControl
{
    public static readonly StyledProperty<Logger> LoggerProperty =
        AvaloniaProperty.Register<LogsControl, Logger>(nameof(Logger));

    public Logger Logger
    {
        get => GetValue(LoggerProperty);
        set => SetValue(LoggerProperty, value);
    }

    public RelayCommand CopyCommand { get; }
    public RelayCommand ClearCommand { get; }

    public LogsControl()
    {
        CopyCommand = new RelayCommand(Copy);
        ClearCommand = new RelayCommand(Clear);
    }

    private void Copy() => TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(string.Join('\n', Logger.Lines));
    private void Clear() => Logger.Clear();
}