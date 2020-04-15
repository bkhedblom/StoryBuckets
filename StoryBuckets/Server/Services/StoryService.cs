using StoryBuckets.Shared;
using StoryBuckets.Shared.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Server.Services
{
    public class StoryService : IStoryService
    {
        public async Task<IEnumerable<IStory>> GetAsync()
            => new[] 
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
