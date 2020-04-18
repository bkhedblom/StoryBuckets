﻿using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores
{
    public class InMemoryDataStore<T> : IDataStore<T> where T : IData
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();
        public bool IsEmpty => !_items.Any();

        public virtual Task AddAsync(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (_items.ContainsKey(item.Id))
                {
                    _items[item.Id] = item;
                }
                else
                {
                    _items.Add(item.Id, item);
                }
            }
            return Task.CompletedTask;
        }

        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<T>>();
            tcs.SetResult(_items.Values.ToArray());
            return tcs.Task;
        }
    }
}
