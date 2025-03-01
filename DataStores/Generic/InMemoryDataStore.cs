﻿using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Generic
{
    public class InMemoryDataStore<T> : IDataStore<T> where T : IData
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();
        private int? _currentMaxId;

        public bool IsEmpty => !_items.Any();

        public virtual bool IsInitialized => true;

        protected IReadOnlyDictionary<int, T> Items => _items;

        public virtual Task AddOrUpdateAsync(IEnumerable<T> items)
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

        public Task<IEnumerable<T>> GetAllAsync() => Task.FromResult(_items.Values.AsEnumerable());

        public Task<IEnumerable<T>> GetAsync(IEnumerable<int> ids) 
            => Task.FromResult(
                _items
                    .Where(kvp => ids.Contains(kvp.Key))
                    .Select(kvp => kvp.Value)
            );

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public virtual async Task UpdateAsync(int id, T item)
        {
            if (id != item.Id)
                throw new InvalidOperationException($"Cannot add item with one id ({item.Id}) to different id ({id})");

            if (!IdIsInStore(id))
                throw new InvalidOperationException($"Item with id {id} does not exist.");

            await AddOrUpdateAsync(new[] { item });
        }

        protected bool IdIsInStore(int id) => _items.ContainsKey(id);       

        protected int GetNextId()
        {
            if (_currentMaxId == null)
                _currentMaxId = Items.Any() ? Items.Max(kvp => kvp.Key) : 0;
            
            _currentMaxId++;
            return _currentMaxId.Value;
        }
    }
}
