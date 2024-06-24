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
        public async static Task<DownloadResult> DownloadAsync(Media video, DataReceivedEventHandler handler)
        {
            if (!File.Exists(ExecutableName))
            {
                return DownloadResult.ExecutableNotFoundError;
            }
            if (string.IsNullOrWhiteSpace(video.URL))
            {
                return DownloadResult.InvalidInput;
            }

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "yt-dlp.exe",
                    Arguments = video.URL,
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
    }
}
