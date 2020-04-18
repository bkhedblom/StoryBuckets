using StoryBuckets.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.Server.Integrations
{
    public interface IIntegration
    {
        Task<IEnumerable<Story>> FetchAsync();
    }
}