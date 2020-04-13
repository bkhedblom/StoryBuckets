namespace StoryBuckets.Shared
{
    public interface IStory:IData
    {
        IBucket Bucket { get; set; }
        bool IsInBucket { get; }
    }
}