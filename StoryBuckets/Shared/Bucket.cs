using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StoryBuckets.Shared
{
    public class Bucket : IBucket
    {
        private readonly Collection<IStory> _stories = new Collection<IStory>();
        private readonly IDataPersister<IBucket> _persister;

        public Bucket(IDataPersister<IBucket> persister)
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
