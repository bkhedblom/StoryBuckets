using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StoryBuckets.Client.Models
{
    public class Bucket : IBucketModel
    {
        private readonly Collection<Story> _stories = new Collection<Story>();

        public IReadOnlyCollection<Story> Stories => _stories;

        public int Id { get; }

        public event EventHandler Updated;

        public void Add(Story story)
        {
            _stories.Add(story);
            story.Bucket = this;
            TriggerUpdated();
        }

        private void TriggerUpdated()
        {
            var handler = Updated;
            handler?.Invoke(this, new EventArgs());
        }
    }
}
