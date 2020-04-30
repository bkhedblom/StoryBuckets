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
        private readonly IDataReader<IBucketModel> _bucketReader;
        private readonly IDataCreator<IBucketModel> _bucketCreator;
        private List<IBucketModel> _buckets;

        public SortingBucketsViewModel(IStorylist storylist, IDataReader<IBucketModel> bucketReader, IDataCreator<IBucketModel> bucketCreator)
        {
            _storylist = storylist;
            _bucketReader = bucketReader;
            _bucketCreator = bucketCreator;
        }
        public string TextForNextStoryToSort => _storylist.NextUnbucketedStory?.ToString() ?? "";
        public bool StoryHidden => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories == 0;
        public bool AllDoneHidden => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories > 0;
        public bool LoaderHidden => _storylist.DataIsready && Buckets != null;
        public bool BucketsHidden => Buckets == null;
        public bool BtnNextDisabled => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories == 0;

        public IReadOnlyCollection<IBucketModel> Buckets { get => _buckets; }

        public void OnClickBtnNext()
            => _storylist.NextUnbucketedStory.IsInBucket = true;

        public async Task OnClickCreateBucket()
        {
            _buckets.Add(await _bucketCreator.CreateEmptyAsync());
        }

        public async Task OnInitializedAsync()
        {
            var bucketReading = _bucketReader.ReadAsync();
            await Task.WhenAll(
                           _storylist.InitializeAsync(),
                           bucketReading
                       );
            _buckets = bucketReading.Result.ToList();
        }
    }
}
