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
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10;

        Health target = null;
        float damage = 0;

        private void Start() {
            transform.LookAt(GetAimLocation());
        }
        
        void Update()
        {
            if(target == null) return;

            if(isHoming && !target.IsDead()){
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (GetComponentInChildren<ParticleSystem>() != null && !GetComponentInChildren<ParticleSystem>().IsAlive())
            {
                gameObject.SetActive(false);
                ((QueuePool<GameObject>)PoolDictionary.pools[this.name]).ReturnInstanceToPool(gameObject);
            }
        }

        public void SetTarget(Health target, float damage){
            this.target = target;
            this.damage = damage;
            //Destroy(gameObject, maxLifeTime);
            
            // Destroy instead of return to pool - Find a way to know when to return if possible.
            // Currently a Destroy will remove all instances from the pool.
            // without having it in update. Most projectile will hit, so not as impactfull.
            // A collider around the world maybe
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
            if (other.GetComponent<Health>() != target)
            {
                return;
            }
            if (target.IsDead())
            {
                return;
            }
            target.TakeDamage(damage);

            HitEffect();

            gameObject.SetActive(false);
            // PoolDictionary takes an Interface, so have to case to specific queue type to call method.
            ((QueuePool<GameObject>)PoolDictionary.pools[this.name]).ReturnInstanceToPool(gameObject);
        }

        private void HitEffect()
        {
            if (hitEffect != null)
            {
                if (!PoolDictionary.pools.ContainsKey(hitEffect.name))
                {
                    PoolDictionary.AddPool(hitEffect.name, () => SpawnHitEffekt(), 3);
                }

                GameObject projectileInstance = PoolDictionary.pools[hitEffect.name].GetInstance();
                projectileInstance.transform.position = GetAimLocation();
                projectileInstance.transform.rotation = transform.rotation;
                projectileInstance.SetActive(true);
            }
        }

        private GameObject SpawnHitEffekt()
        {
            GameObject hitEffektToSpawn = Instantiate(hitEffect);
            hitEffektToSpawn.SetActive(false);
            hitEffektToSpawn.name = hitEffect.name;
            return hitEffektToSpawn;
        }
    }
}


