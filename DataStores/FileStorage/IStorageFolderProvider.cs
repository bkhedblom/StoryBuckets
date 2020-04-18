using StoryBuckets.DataStores.FileStorage;

namespace StoryBuckets.DataStores.FileStore
{
    public interface IStorageFolderProvider
    {
        public IStorageFolder<T> GetStorageFolder<T>(string foldername);
    }
}