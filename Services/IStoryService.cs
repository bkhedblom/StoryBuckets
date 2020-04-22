using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Services
{
    public interface IStoryService
    {
        Task<IEnumerable<Story>> GetAllAsync();
        Task UpdateAsync(int id, Story story);
    }
}
