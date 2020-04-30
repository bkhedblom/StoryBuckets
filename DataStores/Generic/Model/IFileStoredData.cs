using StoryBuckets.Shared.Interfaces;
using System.Threading;

namespace StoryBuckets.DataStores.Generic.Model
{
    public interface IFileStoredData<TData> where TData : IData
    {
        public void MapFromData(TData data);

        public TData ToData();
    }
}