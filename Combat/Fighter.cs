using System;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        Health target = null;
        Weapon currentWeapon = null;
        float timeSinceLastAttack = Mathf.Infinity;
        bool isAttacking;

        public bool IsAttacking { get => isAttacking; private set => isAttacking = value; }

        private void Start() {
            EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            
            if (target == null){
                return;
            }
            if(target.IsDead()){
                return;
            }

            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
            IsAttacking = true;
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            IsAttacking = false;
            GetComponent<Mover>().Cancel();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        // Animation Event (triggered from the animation at the point in the animation that hit the enemy)
        void Hit()
        {
            if (target != null)
            {
                target.TakeDamage(currentWeapon.WeaponDamage);
            }
        }


        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack> defaultWeapon.WeaponSpeed)
            {
                // This will trigger the Hit() event.
                TriggerAttack();
                timeSinceLastAttack = 0;
            }

        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.WeaponRange;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
    }
}

