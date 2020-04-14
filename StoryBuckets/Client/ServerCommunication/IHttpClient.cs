using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface IHttpClient
    {
        public Task<T> GetJsonAsync<T>(string requestUri);
    }
}