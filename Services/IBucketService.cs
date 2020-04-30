using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Services
{
    public interface IBucketService
    {
        Task Add(Bucket bucket);
        Task<IEnumerable<Bucket>> GetAllAsync();
    }
}
