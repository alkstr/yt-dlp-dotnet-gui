using Avalonia.Data.Converters;
using FluentIcons.Common;

namespace YTDLP.GUI.Utilities;

public static class Converters
{
    public static readonly FuncValueConverter<byte, Symbol> DownloadButtonIconConverter =
        new(downloadersCount => downloadersCount switch
        {
            0 => Symbol.ArrowDownload,
            _ => Symbol.Dismiss
        });
}