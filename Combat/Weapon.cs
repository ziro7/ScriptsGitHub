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

        public float WeaponRange { get => weaponRange; private set => weaponRange = value; }
        public float WeaponSpeed { get => weaponSpeed; private set => weaponSpeed = value; }
        public float WeaponDamage { get => weaponDamage; private set => weaponDamage = value; }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if(equippedPrefab != null)
            {
                Transform handTransform;
                handTransform = isRightHanded ? rightHand : leftHand;
                Instantiate(equippedPrefab, handTransform);
            }
            
            if(animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
        }
    }    
}


