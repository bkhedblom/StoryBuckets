namespace StoryBuckets.Client.Components.Counter
{
    public interface ICounterViewModel
    {
        void IncrementButtonClick();
        int CurrentCount { get; }
    }
}