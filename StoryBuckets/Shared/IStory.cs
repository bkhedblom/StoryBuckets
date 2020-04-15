namespace StoryBuckets.Shared
{
    public interface IStory:IData, IWorkItem
    {
        IBucket Bucket { get; set; }
        bool IsInBucket { get; }
    }
}