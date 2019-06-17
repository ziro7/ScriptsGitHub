using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestroy = null;

        void Update()
        {
            if(!GetComponent<ParticleSystem>().IsAlive()){

                if (targetToDestroy != null)
                {   
                    Destroy(targetToDestroy);
                } else
                {
                    gameObject.SetActive(false);
                    ((QueuePool<GameObject>)PoolDictionary.pools[this.name]).ReturnInstanceToPool(gameObject);
                }
            }
        }
    }
}


