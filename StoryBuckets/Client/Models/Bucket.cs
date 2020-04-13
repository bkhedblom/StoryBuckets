using StoryBuckets.Client.ServerSync;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StoryBuckets.Client.Models
{
    public class Bucket : IBucketModel
    {
        private readonly Collection<IStory> _stories = new Collection<IStory>();
        private readonly IServerSync<IBucketModel> _persister;

        public Bucket(IServerSync<IBucketModel> persister)
        {
            _persister = persister;
        }
        public IReadOnlyCollection<IStory> Stories => _stories;

        public async void Add(IStory story)
        {
            _stories.Add(story);
            story.Bucket = this;
            await _persister.Update(this);
        }
    }
}
