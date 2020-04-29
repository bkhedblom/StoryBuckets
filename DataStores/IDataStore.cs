using StoryBuckets.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores
{
    public interface IDataStore<T> : IInitializable where T : IData
    {
        bool IsEmpty { get; }


        Task InitializeAsync();
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(IEnumerable<int> ids);
        Task AddOrUpdateAsync(IEnumerable<T> items);
        Task UpdateAsync(int id, T item);
    }
}