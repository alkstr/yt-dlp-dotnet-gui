using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VTools.Utilities
{
    public static class YTDLP
    {
        public enum Subtitles
        {
            None,
            Embedded,
            File,
        }

        public enum Format
        {
            Best,
            BestAudioOnly,
        }

        public class MetadataInfo
        {
            public required string ExecutablePath;
            public required string URL;
            public required IEnumerable<string> Fields;
        }

        public static Process DownloadProcess(
            string ytdlpPath,
            string ffmpegPath,
            string url,
            Format format,
            Subtitles subtitles,
            string downloadPath)
        {
            var formatArg = format switch
            {
                Format.Best => null,
                Format.BestAudioOnly => "-f bestaudio",
                _ => throw new System.NotImplementedException(),
            };

            var subtitlesArg = subtitles switch
            {
                Subtitles.None => null,
                Subtitles.Embedded => "--embed-subs",
                Subtitles.File => "--write-subs",
                _ => throw new System.NotImplementedException()
            };

            return new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ytdlpPath,
                    Arguments = JoinArguments(
                        $"-P \"{downloadPath}\"",
                        formatArg,
                        subtitlesArg,
                        "-o \"%(title)s [%(id)s].%(ext)s\"",
                        $"--ffmpeg-location \"{ffmpegPath}\"",
                        "--extractor-args \"youtube:player_client=mediaconnect\"",
                        "--encoding utf-8",
                        $"\"{url}\""),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                }
            };
        }

        public static Process GetMetadataProcess(MetadataInfo info) => new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = info.ExecutablePath,
                Arguments = $"{info.URL} -O {string.Join(',', info.Fields)} --encoding utf-8",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
            }
        };

        public static Process UpdateProcess(string executablePath) => new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = executablePath,
                Arguments = $"-U --encoding utf-8",
                CreateNoWindow = false,
            }
        };

        private static string JoinArguments(params string?[] arguments) =>
            string.Join(' ', arguments.Where(arg => arg != null));
    }
}
