using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Server.Services
{
    public interface IStoryService
    {
        Task<IEnumerable<IStory>> GetAllAsync();
    }
}
