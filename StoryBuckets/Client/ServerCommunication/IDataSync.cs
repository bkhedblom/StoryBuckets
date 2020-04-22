using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface IDataSync<T> : IDataReader<T>, IDataUpdater<T> where T:ISyncable
    {
    }
}
