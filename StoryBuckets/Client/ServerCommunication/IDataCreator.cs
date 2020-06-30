using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public interface IDataCreator<T> where T : ISyncable
    {
        Task<T> CreateEmptyAsync();
    }
}