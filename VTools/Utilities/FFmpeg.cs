using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VTools.Utilities
{
    public static class FFmpeg
    {
        public static class Metadata
        {
            public enum StreamEntry
            {
                Duration,
                Width,
                Height,
            }

            public enum FormatEntry
            {
                Duration,
            }

            public static Process Process(
                string ffprobePath,
                string filePath,
                IEnumerable<StreamEntry> streamEntries,
                IEnumerable<FormatEntry> formatEntries)
            {
                var streamEntriesStr = string.Join(',', streamEntries.Select(entry => entry switch
                {
                    StreamEntry.Duration => "duration",
                    StreamEntry.Width    => "width",
                    StreamEntry.Height   => "height",
                    _                    => null,
                }).Where(e => e != null));

                var formatEntriesStr = string.Join(',', formatEntries.Select(entry => entry switch
                {
                    FormatEntry.Duration => "duration",
                    _                    => null,
                }).Where(e => e != null));

                var entriesArg = $"-show_entries format=\"{formatEntriesStr}\":stream=\"{streamEntriesStr}\"";

                return new()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = ffprobePath,
                        Arguments = JoinArguments(
                            entriesArg,
                            "-select_streams v:0",
                            "-v error",
                            "-of json",
                            "-sexagesimal",
                            $"\"{filePath}\""),
                        RedirectStandardOutput = true,
                        RedirectStandardError  = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding  = Encoding.UTF8,
                        CreateNoWindow = true,
                    }
                };
            }

            public class Output
            {
                public class StreamInfo
                {
                    public int Width { get; set; }
                    public int Height { get; set; }
                }

                public class FormatInfo
                {
                    public required string Duration { get; set; }
                }

                public required IEnumerable<StreamInfo> Streams { get; set; }
                public required FormatInfo Format { get; set; }
            }
        }

        public static class Edit
        {
            public static Process Process(
                string ffmpegPath,
                string path,
                (string?, string?) cutInterval,
                (uint?, uint?) scale,
                string savePath,
                string newFileName,
                string newFileExtension)
            {
                var scaleArg = scale switch
                {
                    (null, null) => null,
                    (uint, null) => $"-vf \"scale={scale.Item1}:-1\"",
                    (null, uint) => $"-vf \"scale=-1:{scale.Item2}\"",
                    (uint, uint) => $"-vf \"scale={scale.Item1}:{scale.Item2}",
                };

                var cutIntervalArg = cutInterval switch
                {
                    (null,   null)   => null,
                    (string, null)   => $"-ss \"{cutInterval.Item1}\"",
                    (null,   string) => $"-to \"{cutInterval.Item2}\"",
                    (string, string) => $"-ss \"{cutInterval.Item1}\" -to \"{cutInterval.Item2}\"",
                };

                var fullFileNameArg = $"\"{savePath}/{newFileName}{newFileExtension}\"";

                return new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = ffmpegPath,
                        Arguments = JoinArguments(
                            $"-i \"{path}\"",
                            scaleArg,
                            cutIntervalArg,
                            fullFileNameArg,
                            "-y"),
                        RedirectStandardOutput = true,
                        RedirectStandardError  = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding  = Encoding.UTF8,
                        CreateNoWindow = true,
                    }
                };
            }
        }

        private static string JoinArguments(params string?[] arguments) =>
            string.Join(' ', arguments.Where(arg => arg != null));
    }
}
