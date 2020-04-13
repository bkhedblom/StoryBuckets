using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerSync
{
    public interface IServerSync<T> where T:ISyncable
    {
        Task Update(T model);
    }
}
