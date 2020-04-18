using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Server.Integrations
{
    public class HardcodedIntegration : IIntegration
    {
        public Task<IEnumerable<Story>> FetchAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<Story>>();
            tcs.SetResult(new[]
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
            });
            return tcs.Task;
        }
    }
}
