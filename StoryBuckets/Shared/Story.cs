using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Shared
{
    public class Story : IData, IWorkItem
    {
        public IBucket Bucket { get; set; }

        public virtual bool IsInBucket { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public override string ToString()
            => $"#{Id} {Title}";
    }
}
