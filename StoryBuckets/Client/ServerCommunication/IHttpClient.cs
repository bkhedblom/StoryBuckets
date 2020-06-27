using StoryBuckets.Shared.Interfaces;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface IHttpClient
    {
        Task<T> GetJsonAsync<T>(string requestUri);
        Task<T> PostJsonAsync<T>(string endpoint, T content);
        Task<T> PutJsonAsync<T>(string endpoint, T content) where T : IData;
    }
}