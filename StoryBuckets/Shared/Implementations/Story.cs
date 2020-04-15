using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Shared.Implementations
{
    public class Story : IStory
    {
        public IBucket Bucket { get; set; }

        public bool IsInBucket { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public override string ToString()
            => $"#{Id} {Title}";
    }
}
