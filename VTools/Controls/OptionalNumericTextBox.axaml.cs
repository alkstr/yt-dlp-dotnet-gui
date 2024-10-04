using Avalonia;
using Avalonia.Controls.Primitives;

namespace VTools.Controls;

public partial class OptionalNumericTextBox : TemplatedControl
{
    public static readonly StyledProperty<bool> IsTextBoxEnabledProperty =
        AvaloniaProperty.Register<OptionalNumericTextBox, bool>(nameof(IsTextBoxEnabled));
    public static readonly StyledProperty<int>  TextBoxWidthProperty =
        AvaloniaProperty.Register<OptionalNumericTextBox, int>(nameof(TextBoxWidth), 64);
    public static readonly StyledProperty<int?> ValueProperty =
        AvaloniaProperty.Register<OptionalNumericTextBox, int?>(nameof(Value));

    public bool IsTextBoxEnabled
    {
        get => GetValue(IsTextBoxEnabledProperty);
        set => SetValue(IsTextBoxEnabledProperty, value);
    }

    public int TextBoxWidth
    {
        get => GetValue(TextBoxWidthProperty);
        set => SetValue(TextBoxWidthProperty, value);
    }

    public int? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}