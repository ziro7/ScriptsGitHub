using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        
        Health target = null;
        float damage = 0;

        // Update is called once per frame
        void Update()
        {
            if(target == null) return;

            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, float damage){
            this.target = target;
            this.damage = damage;
        }
       
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null){
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        } 

        private void OnTriggerEnter(Collider other) 
        {
            if(other.GetComponent<Health>() != target){
                return;
            }    
            target.TakeDamage(damage);

            //Destroy(gameObject);
            gameObject.SetActive(false);
            // PoolDictionary takes an Interface, so have to case to specific queue type to call method.
            ((QueuePool<GameObject>)PoolDictionary.pools[this.name]).ReturnInstanceToPool(gameObject);
        }
    }
}


