using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface IDataSync<T> : IDataReader<T> where T:ISyncable
    {
        Task Update(T model);
    }
}
