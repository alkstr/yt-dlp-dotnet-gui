using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace YTDLP.GUI.Utilities;

public static class FFmpeg
{
    public static class Metadata
    {
        public enum StreamEntry
        {
            Duration,
            Width,
            Height
        }

        public enum FormatEntry
        {
            Duration
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
                StreamEntry.Width => "width",
                StreamEntry.Height => "height",
                _ => null
            }).Where(e => e != null));

            var formatEntriesStr = string.Join(',', formatEntries.Select(entry => entry switch
            {
                FormatEntry.Duration => "duration",
                _ => null
            }).Where(e => e != null));

            var entriesArg = $"-show_entries format=\"{formatEntriesStr}\":stream=\"{streamEntriesStr}\"";

            return new Process
            {
                StartInfo = new ProcessStartInfo
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
                    CreateNoWindow = true
                }
            };
        }

        public class Output
        {
            public record StreamInfo(int Width, int Height);

            public record FormatInfo(string Duration);

            public required IEnumerable<StreamInfo> Streams { get; init; }
            public required FormatInfo Format { get; init; }
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
                (not null, null) => $"-vf \"scale={scale.Item1}:-1\"",
                (null, not null) => $"-vf \"scale=-1:{scale.Item2}\"",
                (not null, not null) => $"-vf \"scale={scale.Item1}:{scale.Item2}"
            };

            var cutIntervalArg = cutInterval switch
            {
                (null, null) => null,
                (not null, null) => $"-ss \"{cutInterval.Item1}\"",
                (null, not null) => $"-to \"{cutInterval.Item2}\"",
                (not null, not null) => $"-ss \"{cutInterval.Item1}\" -to \"{cutInterval.Item2}\""
            };

            var fullFileNameArg = $"\"{savePath}/{newFileName}{newFileExtension}\"";

            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = JoinArguments(
                        $"-i \"{path}\"",
                        scaleArg,
                        cutIntervalArg,
                        fullFileNameArg,
                        "-y"),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true
                }
            };
        }
    }

    private static string JoinArguments(params string?[] arguments) =>
        string.Join(' ', arguments.Where(arg => arg != null));
}