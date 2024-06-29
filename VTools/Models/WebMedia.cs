using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VTools.Models
{
    public class WebMedia : ObservableObject
    {
        public static string[] Formats { get; } = [".mp4", ".webm", ".mp3", ".wav", ".ogg", ".m4a", ".aac", ".flv", ".3gp"];

        public string URL { get; set; } = "";
        public string Format { get; set; } = Formats.First();
    }
}
