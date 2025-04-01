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
        private readonly OnReturnedInPool _ActionOnReturnedInPool;
        private readonly OnDestroyObject _ActionOnDestroyObject;
        private readonly OnTakeFromPool _ActionOnTakeFromPool;
        
        public ObjectPool(int capacity, int length, CreatePooledItem createObject, OnTakeFromPool actionOnTakeFromPool,  OnReturnedInPool actionOnReturnedInPool, OnDestroyObject actionOnDestroyObject)
        {
            _createObject = createObject ?? throw new System.ArgumentNullException(nameof(createObject));
            _ActionOnReturnedInPool = actionOnReturnedInPool;
            _ActionOnDestroyObject = actionOnDestroyObject;
            _ActionOnTakeFromPool = actionOnTakeFromPool;

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
                if (_ActionOnDestroyObject != null)
                {
                    _ActionOnReturnedInPool(obj);
                }
                _poolQueue.Enqueue(obj);
            }
            
            else 
            {
                Debug.Log($"Destroying: {_poolQueue.Count}");
                if (_ActionOnDestroyObject != null)
                {
                    _ActionOnDestroyObject(obj);
                }
            }
        }
        
        public T Get()
        {
            if (_poolQueue.Count == 0)
            {
                var obj =  _createObject();
                return _ActionOnTakeFromPool(obj);
            }

            if (_ActionOnTakeFromPool != null)
            {
                return _ActionOnTakeFromPool(_poolQueue.Dequeue());
            }
            
            return _poolQueue.Dequeue();
        }

        public void Clear()
        {
            while (_poolQueue.Count > 0)
            {
                _ActionOnDestroyObject(_poolQueue.Dequeue());
            }
            _poolQueue.Clear();
        }
    }
}