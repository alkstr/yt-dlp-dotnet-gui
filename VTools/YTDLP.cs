using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
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

        public class Metadata {
            public required byte[]? Thumbnail;
            public required string Title;
            public required string Channel;
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
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardError = true,
                    StandardErrorEncoding = Encoding.UTF8,
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

        public async static Task<Metadata> MetadataAsync(WebMedia media, CancellationToken cancellationToken)
        {
            var metadata = new Metadata() { Title = "", Channel = "", Thumbnail = null };
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "yt-dlp.exe",
                    Arguments = $"{media.URL} -O thumbnail,title,channel --encoding utf-8",
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string thumbnailURL;
            using (var reader = process.StandardOutput)
            {
                var text = await reader.ReadToEndAsync(cancellationToken);
                var lines = text.Split('\n');
                if (lines.Length < 3)
                {
                    return metadata;
                }

                thumbnailURL = lines[0];
                metadata.Title = lines[1];
                metadata.Channel = lines[2];
            }
                
           
            if (string.IsNullOrWhiteSpace(thumbnailURL))
            {
                return metadata;
            }
            
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(thumbnailURL, cancellationToken);
                response.EnsureSuccessStatusCode();
                metadata.Thumbnail = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                return metadata;
            }
            catch (HttpRequestException)
            {
                return metadata;
            }
        }

        private static string StringArgumentsForDownload(WebMedia media)
        {
            var url = $"{media.URL}";
            var format = $"-f {media.Format.Trim('.')}";
            var encoding = "--encoding utf-8";
            return string.Join(' ', url, format, encoding);
        }
    }
}
