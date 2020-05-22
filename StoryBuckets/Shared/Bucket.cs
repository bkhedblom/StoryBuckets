using StoryBuckets.Shared.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StoryBuckets.Shared
{
    public class Bucket : IBucket
    {
        private readonly Collection<Story> _stories = new Collection<Story>();

        public Bucket()
        {
        }

        public Bucket(IEnumerable<Story> stories) : this()
        {
            foreach (var story in stories)
            {
                Add(story);
            }
        }
        public int Id { get; set; }

        public IReadOnlyCollection<Story> Stories => _stories;

        public virtual void Add(Story story)
        {
            _stories.Add(story);
            story.IsInBucket = true;
        }
    }
}