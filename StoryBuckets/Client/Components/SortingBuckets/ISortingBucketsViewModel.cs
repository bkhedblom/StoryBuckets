﻿using StoryBuckets.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Components.SortingBuckets
{
    public interface ISortingBucketsViewModel
    {
        string TextForNextStoryToSort { get; }
        bool StoryHidden { get; }
        bool AllDoneHidden { get; }
        bool LoaderHidden { get; }
        bool BtnNextDisabled { get; }
        bool BucketsHidden { get; }
        IReadOnlyCollection<IBucketModel> Buckets { get; }

        Task OnInitializedAsync();
        void OnClickBtnNext();
        Task OnClickCreateBucket();
    }
}