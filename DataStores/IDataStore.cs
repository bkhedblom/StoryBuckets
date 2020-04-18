using StoryBuckets.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores
{
    public interface IDataStore<T> where T : IData
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(IEnumerable<T> items);
        bool IsEmpty { get; }
    }
}