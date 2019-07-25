using System;
using System.Collections.Generic;

namespace RPG.Core
{
    public class QueuePool<T> : IObjectPool<T>, IObjectPoolReturn<T>
    {
        Func<T> produce;
        int capacity;
        Queue<T> objects; 

        public QueuePool (Func<T> CreateMethod, int maxCapacity = 15)
        {
            produce = CreateMethod;
            capacity = maxCapacity;
            objects = new Queue<T>(maxCapacity);
            FillCapacity(CreateMethod);
        }

        private void FillCapacity(Func<T> CreateMethod)
        {
            for (int i = 0; i < capacity; i++)
            {
                objects.Enqueue(produce());
            }
        }

        public T GetInstance()
        {
            if(objects.Count != 0)
            {
                return objects.Dequeue(); 
            } else
            {
                objects.Enqueue(produce());
                return objects.Dequeue();
            }
        }

        public void ReturnInstanceToPool(T objectToReturn){
            objects.Enqueue(objectToReturn);
        }
    }
}
