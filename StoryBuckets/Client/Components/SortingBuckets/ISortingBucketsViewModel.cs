using System.Threading.Tasks;

namespace StoryBuckets.Client.Components.SortingBuckets
{
    public interface ISortingBucketsViewModel
    {
        string TextForNextStoryToSort { get; }
        public bool StoryHidden { get; }
        public bool AllDoneHidden { get; }
        bool LoaderHidden { get; }
        Task OnInitializedAsync();
    }
}