namespace StoryBuckets.Client.ServerCommunication
{
    public interface IDataCreator<T> where T : ISyncable
    {
        T CreateEmpty();
    }
}