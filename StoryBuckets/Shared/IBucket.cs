using System.Collections.Generic;

namespace StoryBuckets.Shared
{
    public interface IBucket:IData
    {
        IReadOnlyCollection<IStory> Stories { get; }
    }
}