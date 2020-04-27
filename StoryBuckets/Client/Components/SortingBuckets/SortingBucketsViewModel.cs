using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Components.SortingBuckets
{
    public class SortingBucketsViewModel : ISortingBucketsViewModel
    {
        private readonly IStorylist _storylist;
        private readonly IDataReader<IBucketModel> _bucketReader;

        public SortingBucketsViewModel(IStorylist storylist, IDataReader<IBucketModel> bucketReader)
        {
            _storylist = storylist;
            _bucketReader = bucketReader;
        }
        public string TextForNextStoryToSort => _storylist.NextUnbucketedStory?.ToString() ?? "";
        public bool StoryHidden => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories == 0;
        public bool AllDoneHidden => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories > 0;
        public bool LoaderHidden => _storylist.DataIsready && Buckets != null;
        public bool BucketsHidden => Buckets == null;
        public bool BtnNextDisabled => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories == 0;

        public IReadOnlyCollection<IBucketModel> Buckets { get; private set; }

        public void OnClickBtnNext()
            => _storylist.NextUnbucketedStory.IsInBucket = true;
        public async Task OnInitializedAsync()
        {
            var bucketReading = _bucketReader.ReadAsync();
            await Task.WhenAll(
                           _storylist.InitializeAsync(),
                           bucketReading
                       );
            Buckets = bucketReading.Result;
        }
    }
}
