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
                IEnumerable<StreamEntry> streamMetadata,
                IEnumerable<FormatEntry> formatMetadata)
            {
                var streamEntriesStr = string.Join(',', streamMetadata.Select(entry => entry switch
                {
                    StreamEntry.Duration => "duration",
                    StreamEntry.Width => "width",
                    StreamEntry.Height => "height",
                    _ => throw new System.NotImplementedException(),
                }));
                var formatEntriesStr = string.Join(',', formatMetadata.Select(entry => entry switch
                {
                    FormatEntry.Duration => "duration",
                    _ => throw new System.NotImplementedException(),
                }));

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
                        RedirectStandardError = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8,
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

        public static Process EditProcess(
            string ffmpegPath,
            string path,
            (string?, string?) cutInterval,
            (uint?, uint?) scale,
            string editedFileName,
            string editedFileExtension)
        {
            var scaleArg = scale switch
            {
                (null, null) => null,
                (uint, null) => $"-vf \"scale={scale.Item1}:-1\"",
                (null, uint) => $"-vf \"scale=-1:{scale.Item2}\"",
                (uint, uint) => $"-vf \"scale={scale.Item1}:{scale.Item2}",
            };

            return new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ffmpegPath,
                    Arguments = JoinArguments(
                        $"-i \"{path}\"",
                        scaleArg,
                        cutInterval.Item1 == null ? null : $"-ss \"{cutInterval.Item1}\"",
                        cutInterval.Item2 == null ? null : $"-to \"{cutInterval.Item2}\"",
                        $"\"{editedFileName}{editedFileExtension}\"",
                        "-y"),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                }
            };
        }

        public static Process DurationProcess(
            string ffprobePath,
            string path) => new()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ffprobePath,
                    Arguments = JoinArguments(
                        "-v error",
                        "-show_entries format=duration",
                        "-of default=noprint_wrappers=1:nokey=1 -sexagesimal",
                        $"\"{path}\""),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                }
            };

        private static string JoinArguments(params string?[] arguments) =>
            string.Join(' ', arguments.Where(arg => arg != null));
    }
}
