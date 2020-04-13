using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models
{
    public class Storylist : IStorylist
    {
        public Storylist(IDataReader<IStory> datareader)
        {

        }

        public IStory NextUnbucketedStory => throw new NotImplementedException();
    }
}
