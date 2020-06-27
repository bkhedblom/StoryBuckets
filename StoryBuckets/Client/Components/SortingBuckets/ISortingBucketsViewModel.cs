using StoryBuckets.Client.Models;
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
        bool DisableBucketChoosing { get; }
        bool BucketsHidden { get; }
        IEnumerable<ISyncableBucket> Buckets { get; }

        Task OnInitializedAsync();
        Task OnClickCreateSmallestBucket();
        void OnBucketChosen(ISyncableBucket bucket);
        Task OnCreateBiggerBucket(ISyncableBucket bucket);
    }
}