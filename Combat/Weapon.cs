using System;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponSpeed = 1.1f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        public float WeaponRange { get => weaponRange; private set => weaponRange = value; }
        public float WeaponSpeed { get => weaponSpeed; private set => weaponSpeed = value; }
        public float WeaponDamage { get => weaponDamage; private set => weaponDamage = value; }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if(equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                Instantiate(equippedPrefab, handTransform);
            }

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            return isRightHanded ? rightHand : leftHand;
        }

        public bool HasProjectile()
        {
            if(projectile != null){
                if (!PoolDictionary.pools.ContainsKey(projectile.name)){
                    PoolDictionary.AddPool(projectile.name, () => SpawnProjectile(), 25);
                }
                return true;
            } else
            {
                return false;    
            }
        }

        private GameObject SpawnProjectile()
        {
            GameObject projectilePrefab = GameObject.Find(projectile.name);
            GameObject projectileToSpawn = Instantiate(projectilePrefab);
            projectileToSpawn.SetActive(false);
            projectileToSpawn.name = projectilePrefab.name; 
            return projectileToSpawn;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            GameObject projectileInstance = PoolDictionary.pools[projectile.name].GetInstance();
            projectileInstance.transform.position = GetTransform(rightHand,leftHand).position;
            projectileInstance.transform.rotation = Quaternion.identity;
            projectileInstance.GetComponent<Projectile>().SetTarget(target, weaponDamage);
            projectileInstance.SetActive(true);
            
            //Projectile projectileInstance = Instantiate(projectileInstance, GetTransform(rightHand,leftHand).position, Quaternion.identity);
            //projectileInstance.SetTarget(target, weaponDamage);
        }


    }    
}


