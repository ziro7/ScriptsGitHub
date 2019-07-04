using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PoolDictionary: MonoBehaviour {
        
        public static Dictionary<string, IObjectPool<GameObject>> pools = new Dictionary<string,IObjectPool<GameObject>>();

        public static void AddPool(string prefabName, Func<GameObject> CreateMethod, int maxCapacity = 15){
            
            if(pools.ContainsKey(prefabName)) {return;}
            pools.Add(prefabName,new QueuePool<GameObject>(CreateMethod,maxCapacity));
        }
    }
}