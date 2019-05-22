using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        void Update()
        {
            if(!GetComponent<ParticleSystem>().IsAlive()){
                gameObject.SetActive(false);
                ((QueuePool<GameObject>)PoolDictionary.pools[this.name]).ReturnInstanceToPool(gameObject);
            }
        }
    }
}


