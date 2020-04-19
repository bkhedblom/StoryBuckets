using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace StoryBuckets.Integrations.CsvIntegration
{
    public class HantverkarprogrammetBacklogExportCsvReader : IFileReader
    {
        public const string CorrectFileHeader = "Work Item Type,State,ID,Parent,Title,Description";
        private readonly IFilesystemIo _filesystem;
        private readonly IIntegrationFilePathProvider _pathProvider;

        public HantverkarprogrammetBacklogExportCsvReader(IFilesystemIo filesystem, IIntegrationFilePathProvider pathProvider)
        {
            _filesystem = filesystem;
            _pathProvider = pathProvider;
        }
        public async IAsyncEnumerable<FlattenedHierarchyItem> ParseAsync()
        {
            using (var filereader = _filesystem.OpenText(_pathProvider.GetPath()))
            {
                var header = await filereader.ReadLineAsync();
                if (header != CorrectFileHeader)
                    throw new UnexpectedCsvHeaderException();

                var parser = new Parser(filereader);
                while (await parser.CheckForMoreItemsAsync())
                {
                    yield return parser.GetNextItem();
                }
            }
        }

        private class Parser
        {
            private static readonly Regex Regexp = new Regex("(?:^\"|,\")((?:[^\"]|\"{2})*?)\"(?!\")");
            private readonly StreamReader _filereader;
            private string _nextLine;

            public Parser(StreamReader filereader)
            {
                _filereader = filereader;
            }

            public FlattenedHierarchyItem GetNextItem()
            {
                if (_nextLine == null)
                    return null;

                var columns = new List<string>();
                Match match = Regexp.Match(_nextLine);

                while (match.Success)
                {
                    columns.Add(match.Groups[1].Value.Replace("\"\"", "\""));
                    match = match.NextMatch();
                }

                var newItem = new FlattenedHierarchyItem
                {
                    Id = int.Parse(columns[2]),
                    Title = columns[4],
                };

                if (int.TryParse(columns[3], out var parsedParent))
                {
                    newItem.ParentId = parsedParent;
                }
                return newItem;
            }

            public async Task<bool> CheckForMoreItemsAsync()
            {
                _nextLine = await _filereader.ReadLineAsync();
                return _nextLine != null;
            }

        }
    }
}
