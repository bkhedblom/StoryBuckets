using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface IHttpClient
    {
        Task<T> GetJsonAsync<T>(string requestUri);
        Task<T> PutJsonAsync<T>(string endpoint, T content) where T : ISyncable;
    }
}