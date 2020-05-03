using StoryBuckets.DataStores.Generic.Model;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoryBuckets.DataStores.Buckets.Model
{
    public class FileStoredBucket : IFileStoredData<Bucket>
    {
        public int Id { get; set; }
        public IEnumerable<int> StoryIds { get; set; }

        public void MapFromData(Bucket data)
        {
            Id = data.Id;
            StoryIds = data.Stories.Select(story => story.Id);
        }

        public Bucket ToData() => ToData(Enumerable.Empty<Story>());

        public Bucket ToData(IEnumerable<Story> stories) => new Bucket(stories)
        {
            Id = this.Id
        };
    }
}
