using StoryBuckets.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Components.SortingBuckets
{
    public class SortingBucketsViewModel : ISortingBucketsViewModel
    {
        private readonly IStorylist _storylist;

        public SortingBucketsViewModel(IStorylist storylist)
        {
            _storylist = storylist;
        }
        public string TextForNextStoryToSort => _storylist.NextUnbucketedStory?.ToString() ?? "";
        public bool StoryHidden => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories == 0;
        public bool AllDoneHidden => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories > 0;
        public bool LoaderHidden => _storylist.DataIsready;

        public bool BtnNextDisabled => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories == 0;

        public void OnClickBtnNext()
            => _storylist.NextUnbucketedStory.IsInBucket = true;
        public async Task OnInitializedAsync()
            => await _storylist.InitializeAsync();
    }
}
