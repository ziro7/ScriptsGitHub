using System;
using RPG.Control;
using RPG.Core;
using RPG.Saving;
using RPG.SceneManagement;
using RPG.Stats;
using UnityEngine;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float healthPoints;
        private bool isDead = false;

        public delegate void DestinationIdentifer (RPG.SceneManagement.DestinationIdentifer[] destinationIdentifersInScene, RPG.SceneManagement.DestinationIdentifer[] destinationIdentifersOutOfScene);

        public event Action OnDamageTaken;
        public event DestinationIdentifer OnBossDeath;

        private void Start() {
            GetFullHealth();
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage){
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints <= 0)
            {
                Die();
                AwardPower(instigator);
            }
            
            // checking if the event is null before triggering.
            if(OnDamageTaken!=null){
                OnDamageTaken();
            } 
        }

        public (float, float) GetHealthPoints()
        {
            float maxHealth = GetComponent<BaseStats>().GetHealth();
            var healthAndMaxHealth = (healthPoints,maxHealth);
            return healthAndMaxHealth; 
        }

        public void GetFullHealth(){
            healthPoints = GetComponent<BaseStats>().GetHealth();
        }

        public object CaptureState()
        {
            return healthPoints; //floats are serializable by default.
        }

        public void RestoreState(object state)
        {
            healthPoints = (float) state;

            if(healthPoints<= 0){
                Die();
            }
        }

        private void Die()
        {
            if (isDead)
            {
                return;
            }
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();

            // Enables portals if a boss is killed
            var isABoss = GetComponent<BossBehavior>();
            if (isABoss != null && isABoss.PortalsToEnableInScene != null)
            {
                if (OnDamageTaken != null)
                {
                    OnBossDeath(isABoss.PortalsToEnableInScene, isABoss.PortalsToEnableInOtherScenes);
                }
            }
        }

        private void AwardPower(GameObject instigator)
        {
            Power power = instigator.GetComponent<Power>();
            if(power == null)
            {
                return;
            }
            power.GainPower(GetComponent<BaseStats>().GetPowerReward());
        }
    }    
}

