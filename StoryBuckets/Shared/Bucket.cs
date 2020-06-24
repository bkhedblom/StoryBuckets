using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StoryBuckets.Shared
{
    public class Bucket : IBucket
    {
        private Collection<Story> _stories;

        public Bucket()
        {
        }

        public Bucket(IEnumerable<Story> stories) : this()
        {
            foreach (var story in stories)
            {
                Add(story);
            }
        }
        public int Id { get; set; }

        public IReadOnlyCollection<Story> Stories
        {
            get
            {
                InitializeStoriesIfNeeded();
                return _stories;
            }
            set
            { //The model binder needs to be able to set...
                if (_stories != null)
                    throw new InvalidOperationException("Stories already set! Try Add() instead");
                _stories = new Collection<Story>(value.ToList());
            }
        }

        public virtual IBucket NextBiggerBucket { get; set; }

        private void InitializeStoriesIfNeeded()
        {
            if (_stories == null)
                _stories = new Collection<Story>();
        }

        public virtual void Add(Story story)
        {
            InitializeStoriesIfNeeded();
            _stories.Add(story);
            story.IsInBucket = true;
        }
    }
}