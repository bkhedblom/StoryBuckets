using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface IDataUpdater<T> where T:ISyncable
    {
        Task UpdateAsync(T model);
    }
}
