using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Components.Counter
{
    public class CounterViewModel : ICounterViewModel
    {
        public int CurrentCount { get; private set; }

        public void IncrementButtonClick() => CurrentCount++;
    }
}
