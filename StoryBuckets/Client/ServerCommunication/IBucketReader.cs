using StoryBuckets.Client.Models;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface IBucketReader
    {
        Task<LinkedBucketModels> ReadLinkedBucketsAsync();
    }
}