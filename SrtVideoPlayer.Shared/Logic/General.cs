using HtmlAgilityPack;
using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Models.Playback;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.Logic
{
    public static class General
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

        public static async Task<Subtitle[]> GetSubtitlesFromContent(string content, bool removeHtmlFormatting) =>
            await Task.Run(() =>
            {
                const int subtitlesSetLines = 3;
                const string timesDelimiter = "-->";
                const string timesFormat = @"hh\:mm\:ss\,fff";
                const string htmlLineBreak = "<br>";
                const string carriageReturn = "\r";
                const string lineFeed = "\n";

                var joinSeparator = removeHtmlFormatting ?
                    Environment.NewLine :
                    htmlLineBreak;

                var subtitles = new List<Subtitle>();
                var lines = content.Split(Environment.NewLine).ToList();
                var lineSets = new List<List<string>>();

                for (var i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    line = line.Replace(carriageReturn, string.Empty);
                    line = line.Replace(lineFeed, string.Empty);
                    lines[i] = line;
                }

                var j = 0;
                while (j < lines.Count)
                {
                    var nextStep = Math.Min(subtitlesSetLines, lines.Count - j);
                    lineSets.Add(lines.GetRange(j, nextStep));
                    j += subtitlesSetLines;

                    if (j < lines.Count)
                    {
                        var lastLineSet = lineSets.Last();
                        var lastLineSetLastIndex = lastLineSet.Count - 1;

                        do
                        {
                            var nextLine = lines[j];

                            j++;

                            if (string.IsNullOrWhiteSpace(nextLine))
                                break;

                            var lastLine = lastLineSet[lastLineSetLastIndex];
                            lastLine = string.Join(joinSeparator, lastLine, nextLine);
                            lastLineSet[lastLineSetLastIndex] = lastLine;

                            if (j == lines.Count)
                                break;

                        } while (true);
                    }
                }

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

                    if (removeHtmlFormatting)
                        try
                        {
                            var htmlText = new HtmlDocument();
                            htmlText.LoadHtml(text);
                            var currentNode = htmlText.DocumentNode;
                            while (currentNode.HasChildNodes)
                                currentNode = currentNode.ChildNodes.First();
                            htmlText.LoadHtml(currentNode.InnerText);
                            text = htmlText.ParsedText;
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine($"Error removing HTML formatting from subtitle line: {exception.Message}");
                        }

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
