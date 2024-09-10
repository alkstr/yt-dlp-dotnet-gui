using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VTools.Models
{
    public partial class LocalMediaFile : ObservableObject
    {
        public static string[] Formats { get; } = [".mp4", ".webm", ".gif"];

        public string Format { get; set; } = Formats.First();

        [ObservableProperty]
        private MediaTime duration = new();
        [ObservableProperty]
        private MediaTime cutStart = new();
        [ObservableProperty]
        private MediaTime cutEnd = new();
        [ObservableProperty]
        private string editedFileName = "";
        [ObservableProperty]
        private bool cut = false;
        [ObservableProperty]
        private string path = "";
    }

    public partial class MediaTime : ObservableObject
    {
        // The NumericUpDown control requires its value property to be nullable;
        // otherwise, having null in the control's text field crashes the program.
        public uint? Hours { get => hours; set => SetProperty(ref hours, value ?? 0); }
        public uint? Minutes { get => minutes; set => SetProperty(ref minutes, value ?? 0); }
        public uint? Seconds { get => seconds; set => SetProperty(ref seconds, value ?? 0); }

        public override string ToString() => $"{Hours}:{Minutes}:{Seconds}";

        private uint hours = 0;
        private uint minutes = 0;
        private uint seconds = 0;
    }
}
