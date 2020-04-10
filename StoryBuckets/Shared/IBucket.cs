using System.Collections.Generic;

namespace StoryBuckets.Shared
{
    public interface IBucket:IModel
    {
        IReadOnlyCollection<IStory> Stories { get; }
        void Add(IStory story);
    }
}