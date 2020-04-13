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
        private readonly IDataReader<IStory> _datareader;

        public Storylist(IDataReader<IStory> datareader)
        {
            _datareader = datareader;
        }

        public IStory NextUnbucketedStory { get; }

        public bool DataIsready { get; }

        public uint? NumberOfUnbucketedStories { get; }

        public async Task InitializeAsync()
        {
            await _datareader.ReadAsync();
        }
    }
}
