using System.Collections.Generic;

namespace StoryBuckets.Shared.Interfaces
{
    public interface IBucket : IData
    {
        IReadOnlyCollection<Story> Stories { get; }
    }
}