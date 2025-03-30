using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public class ObjectPool<T>
    {
        private Queue<T> _poolQueue;
        private readonly int _capacity;     
        public delegate T InitializeObject();
        public delegate T GetObject(T obj);
        public delegate void DeinitializeObject( T obj);
        public delegate void DestroyObject(T obj);
        
        private readonly InitializeObject _initializer;
        private readonly DeinitializeObject _deinitializer;
        private readonly DestroyObject _destructor;
        private readonly GetObject _getter;
        
        public ObjectPool(int capacity, int length, InitializeObject initializer, GetObject getter,  DeinitializeObject deinitializer, DestroyObject destructor)
        {
            _initializer = initializer ?? throw new System.ArgumentNullException(nameof(initializer));
            _deinitializer = deinitializer ?? throw new System.ArgumentNullException(nameof(deinitializer));
            _destructor = destructor ?? throw new System.ArgumentNullException(nameof(destructor));
            _getter = getter ?? throw new System.ArgumentNullException(nameof(getter));

            if (capacity < length)
            {
                throw new System.ArgumentException("Capacity must be greater than or equal to length");
            }
            
            _capacity = capacity;
            
            _poolQueue = new Queue<T>(capacity);

            for (int i = 0; i < length; i++)
            {
                _poolQueue.Enqueue(initializer());
            }
        }

        public void Deinitialize(T obj)
        {
            if (_poolQueue.Count < _capacity)
            {
                Debug.Log($"Deinitializing: {_poolQueue.Count}");
                _deinitializer(obj);
                _poolQueue.Enqueue(obj);
            }
            
            else if (_poolQueue.Count >= _capacity)
            {
                Debug.Log($"Destroying: {_poolQueue.Count}");
                _destructor(obj);
            }
        }
        
        public T Get()
        {
            if (_poolQueue.Count == 0)
            {
                var obj =  _initializer();
                return _getter(obj);
            }
            return _getter(_poolQueue.Dequeue());
        }
    }
}