using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Stories
{
    public class HardcodedStoryStore : IDataStore<Story>
    {
        public Task<IEnumerable<Story>> GetAllAsync() =>  Task.FromResult(new[]
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
            }.AsEnumerable());

        public Task AddOrUpdateAsync(IEnumerable<Story> items)
        {
            throw new NotImplementedException();
        }

        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, Story item)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Story>> GetAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty => false;

        public bool IsInitialized => true;
    }
}
