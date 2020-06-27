using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StoryBuckets.Client.Models
{
    public class SyncableBucket : Bucket, ISyncableBucket
    {
        private SyncableBucket _nextBiggerBucket;

        public SyncableBucket()
        {

        }
        public SyncableBucket(Bucket bucket):base(bucket.Stories)
        {
            Id = bucket.Id;
            base.NextBiggerBucketId = bucket.NextBiggerBucketId;
        }

        public event EventHandler Updated;

        public override void Add(Story story)
        {
            base.Add(story);
            TriggerUpdated();
        }

        public SyncableBucket NextBiggerBucket
        {
            get => _nextBiggerBucket; 
            
            set
            {
                _nextBiggerBucket = value;
                TriggerUpdated();
            }
        }

        public override int? NextBiggerBucketId 
        { 
            get => _nextBiggerBucket?.Id ?? base.NextBiggerBucketId; 
            set => throw new NotSupportedException("Setting NextBiggerBucketId manually is not supported. Set NextBiggerBucket instead"); 
        }

        private void TriggerUpdated()
        {
            var handler = Updated;
            handler?.Invoke(this, new EventArgs());
        }
    }
}
