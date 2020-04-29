using System.Threading.Tasks;

namespace StoryBuckets.DataStores
{
    public interface IInitializable
    {
        bool IsInitialized { get; }

        Task InitializeAsync();
    }
}