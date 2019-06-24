using RPG.Core;
using RPG.Movement;
using RPG.Resources;
using RPG.Saving;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Damage))]
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        Health target = null;
        Weapon currentWeapon = null;
        Damage damage = null;
        AudioSource audioSource = null;
        float timeSinceLastAttack = Mathf.Infinity;
        bool isAttacking;

        public bool IsAttacking { get => isAttacking; private set => isAttacking = value; }

        private void Awake() 
        {
            damage = GetComponent<Damage>();
            if(currentWeapon == null){
                EquipWeapon(defaultWeapon);
            }
        }

        private void Start() {
            audioSource = GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            
            if (target == null || GetComponent<Health>().IsDead()){
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

        public Health GetTarget()
        {
            return target;
        }

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }

        // Animation Event (triggered from the animation at the point in the animation that hit the enemy)
        void Hit()
        {
            if (target != null)
            {
                float damageDone = damage.CalculateDamage(this.gameObject, target, currentWeapon);

                if(currentWeapon.HasProjectile())
                {
                    currentWeapon.LaunchProjectile(rightHandTransform,leftHandTransform, target, gameObject, damageDone);
                } 
                else
                {
                    target.TakeDamage(gameObject,damageDone);
                    if(GetComponentInChildren<ParticleSystem>()!=null){
                        GetComponentInChildren<ParticleSystem>().Play();
                    }
                }
                if (currentWeapon.SoundEffect != null)
                {
                    audioSource.PlayOneShot(currentWeapon.SoundEffect);
                }
            }
        }

        void HitOver()
        {
            if (GetComponentInChildren<ParticleSystem>() != null)
            {
                GetComponentInChildren<ParticleSystem>().Stop();
            }
        }

        // Anitmation event on ranged attack - Hit is the melee version - for now they do the same thing.
        void Shoot(){
            Hit();
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
            Collider targetCollider = target.GetComponent<Collider>();
            float offset = Mathf.Sqrt(Mathf.Pow(targetCollider.bounds.size.x/2, 2f)+Mathf.Pow(targetCollider.bounds.size.y/2, 2f)); 
            return Vector3.Distance(transform.position,target.transform.position) < (currentWeapon.WeaponRange + offset);
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifers(stat stat)
        {
            yield return currentWeapon.Modifier[(int)stat];
        }

        public IEnumerable<float> GetPercentageModifers(stat stat)
        {
            yield return currentWeapon.ModifierPercent[(int)stat];
        }
    }
}

