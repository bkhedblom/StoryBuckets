using StoryBuckets.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.Integrations
{
    public interface IIntegration
    {
        Task<IEnumerable<IStoryFromIntegration>> FetchAsync();
    }
}