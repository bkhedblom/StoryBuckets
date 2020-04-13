namespace StoryBuckets.Client.Components.SortingBuckets
{
    public interface ISortingBucketsViewModel
    {
        string TextForNextStoryToSort { get; }
        public bool HideStory { get; }
        public bool HideAllDone { get; }
        bool HideLoader { get; }
    }
}