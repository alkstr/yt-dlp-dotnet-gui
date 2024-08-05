using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace VTools
{
    public static class YTDLP
    {
        public class DownloadInfo
        {
            public required string ExecutablePath;
            public required string URL;
            public required string Format;
            public required string Directory;
        }

        public class MetadataInfo
        {
            public required string ExecutablePath;
            public required string URL;
            public required IEnumerable<string> Fields;
        }

        public static Process GetDownloadProcess(DownloadInfo info) => new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = info.ExecutablePath,
                Arguments = "" +
                @$"-P ""{info.Directory}"" " +
                @$"-f {info.Format} " +
                @$"-o ""%(title)s [%(id)s].%(ext)s"" " +
                @$"--encoding utf-8 " +
                @$"{info.URL}",
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
                FileName = info.ExecutablePath,
                Arguments = $"{info.URL} -O {string.Join(',', info.Fields)} --encoding utf-8",
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
                CreateNoWindow = true,
            }
        };
    }
}
