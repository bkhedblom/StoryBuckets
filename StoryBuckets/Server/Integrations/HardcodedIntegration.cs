﻿using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Server.Integrations
{
    public class HardcodedIntegration : IIntegration
    {
        public Task<IEnumerable<IStoryFromIntegration>> FetchAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<IStoryFromIntegration>>();
            tcs.SetResult(new[]
                              {
                new StoryFromIntegration
                {
                    Id = 42,
                    Title = "A Planet-sized computer simulation",
                },
                new StoryFromIntegration
                {
                    Id = 31415,
                    Title = "Squaring the circle"
                }
            });
            return tcs.Task;
        }
    }
}
