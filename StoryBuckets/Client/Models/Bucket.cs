using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StoryBuckets.Client.Models
{
    public class Bucket : IBucketModel
    {
        private readonly Collection<Story> _stories = new Collection<Story>();
        private readonly IDataSync<IBucketModel> _persister;

        public Bucket(IDataSync<IBucketModel> persister)
        {
            _persister = persister;
        }
        public IReadOnlyCollection<Story> Stories => _stories;

        public int Id => throw new NotImplementedException();

        public async void Add(Story story)
        {
            _stories.Add(story);
            story.Bucket = this;
            await _persister.Update(this);
        }
    }
}
