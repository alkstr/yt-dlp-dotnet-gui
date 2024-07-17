using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


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

        public class MetadataInfo
        {
            public required string URL;
            public required IEnumerable<string> Fields;
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

        public static Process GetMetadataProcess(MetadataInfo info) => new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = ExecutableName,
                Arguments = $"{info.URL} -O {string.Join(',', info.Fields)} --encoding utf-8",
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
                CreateNoWindow = true,
            }
        };
    }
}
