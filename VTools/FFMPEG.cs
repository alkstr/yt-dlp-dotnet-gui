using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VTools
{
    public static class FFMPEG
    {
        public static Process EditProcess(
            string ffmpegPath,
            string path,
            (string?, string?) cutInterval,
            string editedFileName,
            string editedFileExtension) => new()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ffmpegPath,
                    Arguments = JoinArguments(
                        $"-i \"{path}\"",
                        cutInterval.Item1 == null ? null : $"-ss \"{cutInterval.Item1}\"",
                        cutInterval.Item2 == null ? null : $"-to \"{cutInterval.Item2}\"",
                        $"\"{editedFileName}{editedFileExtension}\"",
                        "-y"),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                }
            };

        public static Process DurationProcess(
            string ffprobePath,
            string path) => new()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ffprobePath,
                    Arguments = JoinArguments(
                        "-v error",
                        "-show_entries format=duration",
                        "-of default=noprint_wrappers=1:nokey=1 -sexagesimal",
                        $"\"{path}\""),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                }
            };

        private static string JoinArguments(params string?[] arguments) =>
            string.Join(' ', arguments.Where(arg => arg != null));
    }
}
