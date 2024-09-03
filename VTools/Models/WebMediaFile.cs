using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using VTools.Assets;
using VTools.Utilities;

namespace VTools.Models
{
    public partial class WebMediaFile : ObservableObject
    {
        public static HashSet<Format> FormatSet { get; } =
        [
            new Format(YTDLP.Format.Best, Resources.BestQuality_ComboBox_Item),
            new Format(YTDLP.Format.BestAudioOnly, Resources.BestQualityAudioOnly_ComboBox_Item),
        ];
        public static HashSet<Subtitles> SubtitlesSet { get; } =
        [
            new Subtitles(YTDLP.Subtitles.None, Resources.NoSubtitles_ComboBox_Item),
            new Subtitles(YTDLP.Subtitles.Embedded, Resources.EmbeddedSubtitles_ComboBox_Item),
            new Subtitles(YTDLP.Subtitles.File, Resources.FileSubtitles_ComboBox_Item),
        ];

        public string URL { get; set; } = "";
        public Format Format { get; set; } = FormatSet.First();
        public Subtitles Subtitles { get; set; } = SubtitlesSet.First();

        [ObservableProperty]
        private Bitmap? thumbnail = null;
        [ObservableProperty]
        private string title = "";
        [ObservableProperty]
        private string channel = "";
    }

    public readonly struct Subtitles(YTDLP.Subtitles type, string description)
    {
        public YTDLP.Subtitles Type { get; } = type;
        public string Description { get; } = description;
    }

    public readonly struct Format(YTDLP.Format type, string description)
    {
        public YTDLP.Format Type { get; } = type;
        public string Description { get; } = description;
    }
}
