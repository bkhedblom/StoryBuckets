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
        public bool HideStory => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories == 0;
        public bool HideAllDone => !_storylist.DataIsready || _storylist.NumberOfUnbucketedStories > 0;
        public bool HideLoader => _storylist.DataIsready;
    }
}
