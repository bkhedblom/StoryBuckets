using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.FileStorage
{
    public interface IStorageFolder<T>
    {
        IAsyncEnumerable<T> GetStoredItemsAsync();
        Task CreateFileForItemAsync(T item, string filename);
        Task ReplaceFileWithItemAsync(string filename, T item);
    }
}