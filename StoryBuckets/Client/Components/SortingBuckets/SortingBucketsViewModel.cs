using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Components.SortingBuckets
{
    public class SortingBucketsViewModel : ISortingBucketsViewModel
    {
        private readonly IStorylist _storylist;
        private readonly IBucketReader _bucketReader;
        private ILinkedSyncableBuckets _buckets;

        public SortingBucketsViewModel(IStorylist storylist, IBucketReader bucketReader)
        {
            _storylist = storylist;
            _bucketReader = bucketReader;
        }
        public string TextForNextStoryToSort => _storylist.NextUnbucketedStory?.ToString() ?? "";
        public bool StoryHidden => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories == 0;
        public bool AllDoneHidden => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories > 0;
        public bool LoaderHidden => _storylist.DataIsready && Buckets != null;
        public bool BucketsHidden => Buckets == null;
        public bool DisableBucketChoosing => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories == 0;

        public IEnumerable<ISyncableBucket> Buckets { get => _buckets; }

        public void OnBucketChosen(ISyncableBucket bucket)
        {
            if(_storylist.NextUnbucketedStory != null)
                bucket.Add(_storylist.NextUnbucketedStory);
        }

        public async Task OnClickCreateSmallestBucket()
        {
            await _buckets.CreateEmptyBiggerThan(null);
        }

        public async Task OnCreateBiggerBucket(ISyncableBucket bucket)
        {
            await _buckets.CreateEmptyBiggerThan(bucket);
        }

        public async Task OnInitializedAsync()
        {
            var bucketReading = _bucketReader.ReadLinkedBucketsAsync();
            await Task.WhenAll(
                           _storylist.InitializeAsync(),
                           bucketReading
                       );
            _buckets = bucketReading.Result;
        }
    }
}
