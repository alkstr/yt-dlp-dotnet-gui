using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YTDLP.Dotnet.GUI.Models
{
    public partial class LocalMediaFile : ObservableObject
    {
        public static string[] Formats { get; } = [".mp4", ".webm", ".gif", ".png", ".mp3"];

        public string NewFormat { get; set; } = Formats.First();

        [ObservableProperty]
        private string    path     = "";
        [ObservableProperty]
        private MediaTime duration = new();
        [ObservableProperty]
        private int?      width    = null;
        [ObservableProperty]
        private int?      height   = null;

        [ObservableProperty]
        private bool      cut      = false;
        [ObservableProperty]
        private MediaTime cutStart = new();
        [ObservableProperty]
        private MediaTime cutEnd   = new();

        [ObservableProperty]
        private bool changeWidth  = false;
        [ObservableProperty]
        private int? newWidth     = null;
        [ObservableProperty]
        private bool changeHeight = false;
        [ObservableProperty]
        private int? newHeight    = null;

        [ObservableProperty]
        private string newFileName = "";
    }

    public partial class MediaTime : ObservableObject
    {
        // The NumericUpDown control requires its value property to be nullable;
        // otherwise, having null in the control's text field crashes the program.
        public uint? Hours   { get => hours;   set => SetProperty(ref hours, value ?? 0); }
        public uint? Minutes { get => minutes; set => SetProperty(ref minutes, value ?? 0); }
        public uint? Seconds { get => seconds; set => SetProperty(ref seconds, value ?? 0); }
        public string FullString => $"{hours:00}:{minutes:00}:{seconds:00}";

        public override string ToString() => (hours, minutes, seconds) switch
        {
            (0, 0, 0) => "",
            (0, 0, _) => $"{seconds:00}",
            (0, _, _) => $"{minutes:00}:{seconds:00}",
            (_, _, _) => $"{hours:00}:{minutes:00}:{seconds:00}",
        };

        private uint hours = 0;
        private uint minutes = 0;
        private uint seconds = 0;
    }
}
