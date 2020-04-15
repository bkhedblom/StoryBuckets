using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models
{
    public interface IStorylist
    {
        IStory NextUnbucketedStory { get; }
        bool DataIsready { get; }
        uint? NumberOfUnbucketedStories { get; }
        Task InitializeAsync();
    }
}
