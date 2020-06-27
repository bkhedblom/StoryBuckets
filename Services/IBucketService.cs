using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Services
{
    public interface IBucketService
    {
        Task AddAsync(Bucket bucket);
        Task<IEnumerable<Bucket>> GetAllAsync();
        Task UpdateAsync(int id, Bucket bucket);
    }
}
