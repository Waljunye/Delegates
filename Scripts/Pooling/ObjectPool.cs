using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public class ObjectPool<T>
    {
        private Queue<T> _poolQueue;
        private readonly int _capacity;     
        public delegate T CreatePooledItem();
        public delegate T OnTakeFromPool(T obj);
        public delegate void OnReturnedInPool( T obj);
        public delegate void OnDestroyObject(T obj);
        
        private readonly CreatePooledItem _createObject;
        private readonly OnReturnedInPool _onReturnedInPool;
        private readonly OnDestroyObject _onDestroyObject;
        private readonly OnTakeFromPool _onTakeFromPool;
        
        public ObjectPool(int capacity, int length, CreatePooledItem createObject, OnTakeFromPool onTakeFromPool,  OnReturnedInPool onReturnedInPool, OnDestroyObject onDestroyObject)
        {
            _createObject = createObject ?? throw new System.ArgumentNullException(nameof(createObject));
            _onReturnedInPool = onReturnedInPool;
            _onDestroyObject = onDestroyObject;
            _onTakeFromPool = onTakeFromPool;

            if (capacity < length)
            {
                throw new System.ArgumentException("Capacity must be greater than or equal to length");
            }
            
            _capacity = capacity;
            
            _poolQueue = new Queue<T>(capacity);

            for (int i = 0; i < length; i++)
            {
                _poolQueue.Enqueue(createObject());
            }
        }

        public void Release(T obj)
        {
            if (_poolQueue.Count < _capacity)
            {
                Debug.Log($"Deinitializing: {_poolQueue.Count}");
                if (_onDestroyObject != null)
                {
                    _onReturnedInPool(obj);
                }
                _poolQueue.Enqueue(obj);
            }
            
            else if (_poolQueue.Count >= _capacity)
            {
                Debug.Log($"Destroying: {_poolQueue.Count}");
                if (_onDestroyObject != null)
                {
                    _onDestroyObject(obj);
                }
            }
        }
        
        public T Get()
        {
            if (_poolQueue.Count == 0)
            {
                var obj =  _createObject();
                return _onTakeFromPool(obj);
            }

            if (_onTakeFromPool != null)
            {
                return _onTakeFromPool(_poolQueue.Dequeue());
            }
            
            return _poolQueue.Dequeue();
        }

        public void Clear()
        {
            while (_poolQueue.Count > 0)
            {
                _onDestroyObject(_poolQueue.Dequeue());
            }
        }
    }
}