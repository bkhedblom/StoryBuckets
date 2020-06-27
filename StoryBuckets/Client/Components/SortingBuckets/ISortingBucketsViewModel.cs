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
        Task OnClickCreateBucket();
        void OnBucketChosen(ISyncableBucket bucket);
    }
}