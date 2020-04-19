using System.Collections.Generic;

namespace StoryBuckets.Integrations.CsvIntegration
{
    public interface IFileReader
    {
        IAsyncEnumerable<FlattenedHierarchyItem> ParseAsync();
    }
}