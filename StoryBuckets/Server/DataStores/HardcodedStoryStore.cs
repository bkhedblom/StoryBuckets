using StoryBuckets.Shared;
using StoryBuckets.Shared.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Server.DataStores
{
    public class HardcodedStoryStore : IDataStore<IStory>
    {
        public async Task<IEnumerable<IStory>> GetAllAsync() => new[]
                       {
                new Story
                {
                    Id = 42,
                    Title = "A Planet-sized computer simulation",
                },
                new Story
                {
                    Id = 31415,
                    Title = "Squaring the circle"
                }
            };
    }
}
