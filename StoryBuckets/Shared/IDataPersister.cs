using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Shared
{
    public interface IDataPersister<T> where T:IModel
    {
        Task Update(T model);
    }
}
