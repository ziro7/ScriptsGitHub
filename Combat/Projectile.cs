using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] float maxRange = 30f;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;

        GameObject player;
        GameObject instigator = null;
        Health target = null;

        float damage = 0;

        private void Awake() {
            player = GameObject.FindGameObjectWithTag("Player");
        }

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

            if (IsOutOfRange())
            {
                gameObject.SetActive(false);
                ((QueuePool<GameObject>)PoolDictionary.pools[this.name]).ReturnInstanceToPool(gameObject);
            } 
        }

        private bool IsOutOfRange()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer > maxRange;
        }

        public void SetTarget(Health target, GameObject instigator, float damage){
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
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
            target.TakeDamage(instigator, damage);

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


