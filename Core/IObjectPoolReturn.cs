using System;

namespace RPG.Core
{        
    public interface IObjectPoolReturn<in T> { 
        void ReturnInstanceToPool(T objectToReturnToPool);
    }
}