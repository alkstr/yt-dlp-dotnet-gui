using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VTools.Models;

namespace VTools
{
    public static class FFMPEG
    {
        public static readonly string ExecutableName = "ffmpeg.exe";
        public static readonly string FFprobeExecutableName = "ffprobe.exe";

        public enum EditResult
        {
            InvalidInput,
            AnotherInProgressError,
            ExecutableNotFoundError,
            Success,
        }
        public async static Task<EditResult> EditAsync(LocalMediaFile media, DataReceivedEventHandler onLogsReceived)
        {
            if (!File.Exists(ExecutableName))
            {
                return EditResult.ExecutableNotFoundError;
            }
            if (string.IsNullOrWhiteSpace(media.Path) || string.IsNullOrWhiteSpace(media.EditedFileName))
            {
                return EditResult.InvalidInput;
            }

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ExecutableName,
                    Arguments = StringArguments(media),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += onLogsReceived;
            process.BeginErrorReadLine();
            process.ErrorDataReceived += onLogsReceived;
            await process.WaitForExitAsync();
            return EditResult.Success;
        }

        public async static Task<MediaTime?> GetMediaDurationAsync(LocalMediaFile media)
        {
            if (!File.Exists(FFprobeExecutableName) || string.IsNullOrWhiteSpace(media.Path))
            {
                return null;
            }

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = FFprobeExecutableName,
                    Arguments = DurationStringArguments(media),
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                }
            };

            var duration = string.Empty;

            process.Start();
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => { duration += e.Data ?? ""; };
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();

            var durationSplit = duration.Split(':');
            if (durationSplit.Length != 3)
            {
                return null;
            }

            return new MediaTime()
            {
                Hours = uint.Parse(durationSplit[0]),
                Minutes = uint.Parse(durationSplit[1]),
                Seconds = (uint)float.Parse(durationSplit[2])
            };
        }

        private static string StringArguments(LocalMediaFile media)
        {
            var input = $@"-i ""{media.Path}""";
            var from = media.WillBeCut ? $"-ss {media.CutStart}" : string.Empty;
            var to = media.WillBeCut ? $"-to {media.CutEnd}" : string.Empty;
            var output = $@"""{media.EditedFileName}{media.Format}""";
            var flags = "-y";
            return string.Join(' ', input, from, to, flags, output);
        }

        private static string DurationStringArguments(LocalMediaFile media)
        {
            var logLevel = "-v error";
            var info = "-show_entries format=duration";
            var format = "-of default=noprint_wrappers=1:nokey=1 -sexagesimal";
            var input = $@"""{media.Path}""";
            return string.Join(' ', logLevel, info, format, input);
        }
    }
}
