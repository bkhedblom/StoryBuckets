using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface IDataReader<T> where T : IData
    {
        Task<IReadOnlyCollection<T>> Read();
    }
}
