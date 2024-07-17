using System.Diagnostics;
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

        public class DownloadInfo
        {
            public required string URL;
            public required string Format;
        }

        public class Metadata
        {
            public required byte[]? Thumbnail;
            public required string Title;
            public required string Channel;
        }

        public static Process GetDownloadProcess(DownloadInfo info) => new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = ExecutableName,
                Arguments = $"{info.URL} -f {info.Format} --encoding utf-8",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
            }
        };

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
    }
}
