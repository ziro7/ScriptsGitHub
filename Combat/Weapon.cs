using RPG.Core;
using RPG.Resources;
using RPG.Stats;
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
        [Tooltip("WeaponDamage is pr. 1 weaponspeed")]
        [SerializeField] float weaponDamage = 5f;
        [Tooltip("BaseHealth,PowerReward,PowerToLevelUp,Strength,Agility,Stamina,Intellect,Spirit,Armor,CritPercentage,MagicResistance")]
        [SerializeField] float[] modifiers = new float[11] {0,0,0,0,0,0,0,0,0,0,0};
        [Tooltip("BaseHealth,PowerReward,PowerToLevelUp,Strength,Agility,Stamina,Intellect,Spirit,Armor,CritPercentage,MagicResistance")]
        [SerializeField] float[] modifiersPercent = new float[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        [SerializeField] bool isRightHanded = true;
        [SerializeField] bool isMagicAttack = false;
        [SerializeField] Projectile projectile = null;
        [SerializeField] GameObject projectilePrefab = null;
        [SerializeField] AudioClip soundEffect = null;

        const string weaponName = "Weapon"; 

        public float WeaponRange { get => weaponRange; private set => weaponRange = value; }
        public float WeaponSpeed { get => weaponSpeed; private set => weaponSpeed = value; }
        public float WeaponDamage { get => weaponDamage; private set => weaponDamage = value; }
        public bool IsMagicAttack { get => isMagicAttack; set => isMagicAttack = value; }
        public AudioClip SoundEffect { get => soundEffect; set => soundEffect = value; }

        public float Modifiers(Stat stat)
        {
            return modifiers[(int)stat];
        }

        public float ModifiersPercent(Stat stat)
        {
            return modifiersPercent[(int)stat];
        }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if(equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }

            // Cast the current animatorcontroller as default controller on parent gameObject
            // If cast is succesfull it means that it is the parent
            // If unsucessful it returns null and means it is "just" a overrideController set by another weapon. 
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            // checks if the animatorOverride have been set on weapon.
            if (animatorOverride != null)
            {
                // sets the default controller to use the override controller.
                animator.runtimeAnimatorController = animatorOverride;
            } 
            // If there is not a override controller on weapon and the cast was succesfull.
            else if (overrideController != null) 
            {
                // we set the controller to the default on the parent.
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                // If we didn't do this the animation would display animation from last weapon.
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null){
                return;
            }

            oldWeapon.name = "DestroyingWeapon"; //bug fix if pickup and destroy is called in same frame
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            return isRightHanded ? rightHand : leftHand;
        }

        public bool HasProjectile()
        {
            if(projectile != null){
                if (!PoolDictionary.pools.ContainsKey(projectile.name))
                {
                    PoolDictionary.AddPool(projectile.name, () => SpawnProjectile(), 5);
                }
                return true;
            } else
            {
                return false;    
            }
        }

        private GameObject SpawnProjectile()
        {
            GameObject projectileToSpawn = Instantiate(projectilePrefab);
            projectileToSpawn.SetActive(false);
            projectileToSpawn.name = projectilePrefab.name; 
            return projectileToSpawn;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float damage)
        {
            GameObject projectileInstance = PoolDictionary.pools[projectile.name].GetInstance();
            projectileInstance.transform.position = GetTransform(rightHand,leftHand).position;
            projectileInstance.transform.rotation = Quaternion.identity;
            projectileInstance.GetComponent<Projectile>().SetTarget(target, instigator, damage);
            projectileInstance.SetActive(true);
        }
    }    
}


