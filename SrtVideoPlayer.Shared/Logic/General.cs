using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Models.Playback;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.Logic
{
    public class General
    {
        public static string RemoveProtocolAndSlashesFromAddress(string address)
        {
            const string protocolDelimiter = "://";
            const string slash = "/";
            const string dash = "-";
            var sections = address.Split(protocolDelimiter);
            var addressWithoutProtocol = sections.Length > 1 ? sections[1] : sections[0];
            var addressWithoutSlashes = addressWithoutProtocol.Replace(slash, dash);
            return addressWithoutSlashes;
        }

        public static async Task<string> ReadSubtitlesFileContent(string path)
        {
            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using var streamReader = new StreamReader(stream, Encoding.UTF8);
                return await streamReader.ReadToEndAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<Subtitle[]> GetSubtitlesFromContent(string content) =>
            await Task.Run(() =>
            {
                const int subtitlesSetLines = 4;
                const string timesDelimiter = "-->";
                const string timesFormat = @"hh\:mm\:ss\,fff";
                var subtitles = new List<Subtitle>();
                var lines = content.Split(Environment.NewLine).ToList();
                var lineSets = new List<List<string>>();
                for (int i = 0; i < lines.Count; i += subtitlesSetLines)
                    lineSets.Add(lines.GetRange(i, Math.Min(subtitlesSetLines, lines.Count - i)));
                foreach (var lineSet in lineSets)
                {
                    if (lineSet.Count != subtitlesSetLines)
                        continue;

                    var index = lineSet[0].Trim();
                    if (!int.TryParse(index, out var indexAsInt))
                        continue;

                    var times = lineSet[1].Split(timesDelimiter);
                    if (times.Length != 2)
                        continue;
                    var startTime = times[0].Trim();
                    if (!TimeSpan.TryParseExact(startTime, timesFormat, CultureInfo.InvariantCulture, out var startTimeAsTimeSpan))
                        continue;
                    var endTime = times[1].Trim();
                    if (!TimeSpan.TryParseExact(endTime, timesFormat, CultureInfo.InvariantCulture, out var endTimeAsTimeSpan))
                        continue;

                    var text = lineSet[2].Trim();

                    subtitles.Add(new Subtitle(indexAsInt, new SubtitleSpan(startTimeAsTimeSpan, endTimeAsTimeSpan), text));
                }

                return subtitles.ToArray();
            });

        public static string ConvertTimeSpanToShortestString(TimeSpan timeSpan) =>
            timeSpan.TotalHours >= 1 ?
                timeSpan.ToString(Strings.HoursMinutesAndSecondsFormat) :
                timeSpan.ToString(Strings.MinutesAndSecondsFormat);

        public static TimeSpan? ConvertShortestStringToTimeSpan(string stringTime) =>
            TimeSpan.TryParseExact(stringTime,
                new string[] { Strings.HoursMinutesAndSecondsFormat, Strings.MinutesAndSecondsFormat },
                CultureInfo.InvariantCulture,
                out var timeSpan) ?
                timeSpan :
                (TimeSpan?)null;
    }
}
