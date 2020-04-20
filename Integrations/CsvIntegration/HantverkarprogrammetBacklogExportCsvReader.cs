using StoryBuckets.Options;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace StoryBuckets.Integrations.CsvIntegration
{
    public class HantverkarprogrammetBacklogExportCsvReader : IFileReader
    {
        public const string CorrectFileHeader = "Work Item Type,State,ID,Parent,Title,Description";
        private readonly IFilesystemIo _filesystem;
        private readonly IIntegrationPathProvider _pathProvider;

        public HantverkarprogrammetBacklogExportCsvReader(IFilesystemIo filesystem, IIntegrationPathProvider pathProvider)
        {
            _filesystem = filesystem;
            _pathProvider = pathProvider;
        }
        public async IAsyncEnumerable<FlattenedHierarchyItem> ParseAsync()
        {
            using (var filereader = _filesystem.OpenText(_pathProvider.GetPathToIntegrationFile()))
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

                var sb = new StringBuilder();
                var inQuotedArea = false;

                for (int i = 0; i < _nextLine.Length; i++)
                {
                    var currentChar = _nextLine[i];
                    if (inQuotedArea)
                    {
                        if (currentChar == '"')
                        {
                            if (i + 1 < _nextLine.Length && _nextLine[i + 1] == '"')
                            {
                                sb.Append(currentChar);
                                i++;
                            }
                            else
                            {
                                inQuotedArea = false;
                            }
                            continue;
                        }
                        sb.Append(currentChar);
                    }
                    else
                    {
                        if (currentChar == '"')
                        {
                            inQuotedArea = true;
                        }

                        if (currentChar == ',')
                        {
                            columns.Add(sb.ToString());
                            sb.Clear();
                        }
                    }
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
