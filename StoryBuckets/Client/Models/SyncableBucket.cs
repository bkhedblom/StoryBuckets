using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StoryBuckets.Client.Models
{
    public class SyncableBucket : Bucket, IBucketModel
    {
        public event EventHandler Updated;

        public override void Add(Story story)
        {
            base.Add(story);
            TriggerUpdated();
        }

        public override Bucket NextBiggerBucket
        {
            get => base.NextBiggerBucket; 
            
            set
            {
                base.NextBiggerBucket = value;
                TriggerUpdated();
            }
        }

        private void TriggerUpdated()
        {
            var handler = Updated;
            handler?.Invoke(this, new EventArgs());
        }
    }
}
