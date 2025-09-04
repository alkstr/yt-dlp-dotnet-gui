namespace YTDLP.GUI.Utilities;

public static class StringExtensions
{
    public static bool IsTrue(this string value) =>
        !value.IsFalse();

    public static bool IsFalse(this string value) =>
        value.Equals("false", System.StringComparison.CurrentCultureIgnoreCase)
        || string.IsNullOrWhiteSpace(value);
}