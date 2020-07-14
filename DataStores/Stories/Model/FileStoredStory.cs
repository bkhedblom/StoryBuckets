using StoryBuckets.DataStores.Generic.Model;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.DataStores.Stories.Model
{
    public class FileStoredStory : IFileStoredData<Story>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsInBucket { get; set; }
        public bool IsIrrelevant { get; set; }

        public void MapFromData(Story data)
        {
            Id = data.Id;
            Title = data.Title;
            IsInBucket = data.IsInBucket;
            IsIrrelevant = data.IsIrrelevant;
        }

        public Story ToData() => new Story
        {
            Id = Id,
            Title = Title,
            IsInBucket = IsInBucket,
            IsIrrelevant = IsIrrelevant
        };
    }
}
