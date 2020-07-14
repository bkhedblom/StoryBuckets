using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models
{
    public class SyncableStory:Story, ISyncable
    {
        public SyncableStory()
        {

        }
        public SyncableStory(Story story)
        {
            Id = story.Id;
            Title = story.Title;
            IsInBucket = story.IsInBucket;
            IsIrrelevant = story.IsIrrelevant;
        }

        public override bool IsInBucket
        {
            get => base.IsInBucket; 
            
            set
            {
                bool isUpdated = value != base.IsInBucket;
                
                base.IsInBucket = value;

                if (isUpdated)
                    OnUpdated();
            }
        }

        public override bool IsIrrelevant 
        { 
            get => base.IsIrrelevant; 
            set
            {
                var isUpdated = value != base.IsIrrelevant;
                base.IsIrrelevant = value;
                if(isUpdated)
                    OnUpdated();
            }
        }

        private void OnUpdated()
        {
            var handler = Updated;
            handler?.Invoke(this, new EventArgs());
        }

        public event EventHandler Updated;       
    }
}
