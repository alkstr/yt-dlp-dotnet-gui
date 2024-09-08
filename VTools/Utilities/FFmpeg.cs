using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VTools.Utilities
{
    public static class FFmpeg
    {
        public static Process EditProcess(
            string ffmpegPath,
            string path,
            (string?, string?) cutInterval,
            (uint?, uint?) scale,
            string editedFileName,
            string editedFileExtension)
        {
            var scaleArg = scale switch
            {
                (null, null) => null,
                (uint, null) => $"-vf \"scale={scale.Item1}:-1\"",
                (null, uint) => $"-vf \"scale=-1:{scale.Item2}\"",
                (uint, uint) => $"-vf \"scale={scale.Item1}:{scale.Item2}",
            };

            return new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ffmpegPath,
                    Arguments = JoinArguments(
                        $"-i \"{path}\"",
                        scaleArg,
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
        }

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
