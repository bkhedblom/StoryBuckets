using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Server.Integrations
{
    public interface IStoryFromIntegration : IWorkItem
    {
        int Id { get; }
    }
}
