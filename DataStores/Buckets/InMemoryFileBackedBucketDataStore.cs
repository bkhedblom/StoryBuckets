using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Generic;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Buckets
{
    public class InMemoryFileBackedBucketDataStore: InMemoryFileBackedDataStore<Bucket>
    {
        public InMemoryFileBackedBucketDataStore(IStorageFolderProvider folderProvider)
            :base(folderProvider.GetStorageFolder<Bucket>("buckets"))
        { }

        public override Task AddOrUpdateAsync(IEnumerable<Bucket> items)
            => throw new NotImplementedException();

        public override Task UpdateAsync(int id, Bucket item)
            => throw new NotImplementedException();

    }
}
