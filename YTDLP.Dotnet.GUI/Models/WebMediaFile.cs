using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using YTDLP.Dotnet.GUI.Assets;
using YTDLP.Dotnet.GUI.Utilities;

namespace YTDLP.Dotnet.GUI.Models;

public partial class WebMediaFile : ObservableObject
{
    public static HashSet<Format> FormatSet { get; } =
    [
        new(YouTube.Download.Format.Best,          Resources.BestQuality_ComboBox_Item),
        new(YouTube.Download.Format.BestAudioOnly, Resources.BestQualityAudioOnly_ComboBox_Item)
    ];

    public static HashSet<Subtitles> SubtitlesSet { get; } =
    [
        new(YouTube.Download.Subtitles.None,     Resources.NoSubtitles_ComboBox_Item),
        new(YouTube.Download.Subtitles.Embedded, Resources.EmbeddedSubtitles_ComboBox_Item),
        new(YouTube.Download.Subtitles.File,     Resources.FileSubtitles_ComboBox_Item)
    ];

    public string    URL       { get; set; } = "";
    public Format    Format    { get; set; } = FormatSet.First();
    public Subtitles Subtitles { get; set; } = SubtitlesSet.First();

    [ObservableProperty] private Bitmap? thumbnail;
    [ObservableProperty] private string  title   = "";
    [ObservableProperty] private string  channel = "";
}

public readonly struct Subtitles(YouTube.Download.Subtitles type, string description)
{
    public YouTube.Download.Subtitles Type { get; } = type;
    public string Description { get; } = description;
}

public readonly struct Format(YouTube.Download.Format type, string description)
{
    public YouTube.Download.Format Type { get; } = type;
    public string Description { get; } = description;
}