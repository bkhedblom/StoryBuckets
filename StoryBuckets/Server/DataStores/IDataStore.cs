using StoryBuckets.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.Server.DataStores
{
    public interface IDataStore<T> where T : IData
    {
        Task<IEnumerable<T>> GetAllAsync();
    }
}