using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Integrations
{
    internal class StoryFromIntegration : IStoryFromIntegration
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}
