using SrtVideoPlayer.Shared.Models.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static async Task<Subtitle[]> GetSubtitlesFromContent(string content) =>
            await Task.Run(() =>
            {
                const int subtitlesSetLines = 4;
                const string timesDelimiter = "-->";
                //const string timesFormat = "hh:mm:ss,fff";
                var subtitles = new List<Subtitle>();
                var lines = content.Split(Environment.NewLine).ToList();
                var lineSets = new List<List<string>>();
                for (int i = 0; i < lines.Count; i += subtitlesSetLines)
                    lineSets.Add(lines.GetRange(i, Math.Min(subtitlesSetLines, lines.Count - i)));
                foreach (var lineSet in lineSets)
                {
                    if (lines.Count != subtitlesSetLines)
                        continue;

                    var index = lineSet[0].Trim();
                    if (!int.TryParse(index, out var indexAsInt))
                        continue;

                    var times = lineSet[1].Split(timesDelimiter);
                    if (times.Length != 2)
                        continue;
                    var startTime = times[0].Trim();
                    if (!TimeSpan.TryParse(startTime, out var startTimeAsTimeSpan))
                        continue;
                    var endTime = times[1].Trim();
                    if (!TimeSpan.TryParse(endTime, out var endTimeAsTimeSpan))
                        continue;

                    var text = lineSet[2].Trim();

                    subtitles.Add(new Subtitle(indexAsInt, new SubtitleSpan(startTimeAsTimeSpan, endTimeAsTimeSpan), text));
                }

                return subtitles.ToArray();
            });
    }
}
