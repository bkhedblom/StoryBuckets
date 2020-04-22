using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models
{
    public class Storylist : IStorylist
    {
        private readonly IDataReader<SyncableStory> _datareader;
        private IReadOnlyCollection<SyncableStory> _stories;

        public Storylist(IDataReader<SyncableStory> datareader)
        {
            _datareader = datareader;
        }

        public Story NextUnbucketedStory => _stories?.FirstOrDefault(story => !story.IsInBucket);

        public bool DataIsready => _stories != null;

        public uint? NumberOfUnbucketedStories => (uint?) _stories?.Count(story => !story.IsInBucket);

        public async Task InitializeAsync() 
            => _stories = await _datareader.ReadAsync();
    }
}
