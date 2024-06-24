using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VTools.Models;

namespace VTools
{
    public static class FFMPEG
    {
        public static readonly string ExecutableName = "ffmpeg.exe";

        public enum EditResult
        {
            InvalidInput,
            AnotherInProgressError,
            ExecutableNotFoundError,
            Success,
        }
        public async static Task<EditResult> EditAsync(MediaForEdit media, DataReceivedEventHandler onLogsReceived)
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

        private static string StringArguments(MediaForEdit media)
        {
            var input = $"-i {media.Path}";
            var from = media.WillBeCut ? $"-ss {media.CutStart}" : string.Empty;
            var to = media.WillBeCut ? $"-to {media.CutEnd}" : string.Empty;
            var output = $@"""{media.EditedFileName}{media.Format}""";
            var flags = "-y";
            return string.Join(' ', input, from, to, flags, output);
        }
    }
}
