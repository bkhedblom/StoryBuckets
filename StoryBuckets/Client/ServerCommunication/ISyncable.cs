using StoryBuckets.Shared.Interfaces;
using System;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface ISyncable:IData
    {
        event EventHandler Updated;
    }
}