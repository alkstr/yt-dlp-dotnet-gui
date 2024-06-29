using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VTools.Models;

namespace VTools
{
    public static class YTDLP
    {
        public static readonly string ExecutableName = "yt-dlp.exe";

        public enum DownloadResult
        {
            InvalidInput,
            AnotherInProgressError,
            ExecutableNotFoundError,
            Success,
        }
        public async static Task<DownloadResult> DownloadAsync(WebMedia media, DataReceivedEventHandler handler)
        {
            if (!File.Exists(ExecutableName))
            {
                return DownloadResult.ExecutableNotFoundError;
            }
            if (string.IsNullOrWhiteSpace(media.URL))
            {
                return DownloadResult.InvalidInput;
            }

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "yt-dlp.exe",
                    Arguments = StringArgumentsForDownload(media),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += handler;
            process.BeginErrorReadLine();
            process.ErrorDataReceived += handler;
            await process.WaitForExitAsync();
            return DownloadResult.Success;
        }

        private static string StringArgumentsForDownload(WebMedia media)
        {
            var format = $"-f {media.Format.Trim('.')}";
            var url = $"{media.URL}";
            return string.Join(' ', format, url);
        }
    }
}
