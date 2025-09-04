using Avalonia;
using Avalonia.Controls.Primitives;
using FluentIcons.Common;

namespace YTDLP.Dotnet.GUI.Controls;

public class IconTextBlock : TemplatedControl
{
    public static readonly StyledProperty<Symbol> IconProperty =
        AvaloniaProperty.Register<IconTextBlock, Symbol>(nameof(Icon));

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<IconTextBlock, string>(nameof(Text));

    public Symbol Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}