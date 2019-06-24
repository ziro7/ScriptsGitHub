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
        float damagePoints = 0f;
        private bool isDead = false;

        public delegate void DestinationIdentifer (RPG.SceneManagement.DestinationIdentifer[] destinationIdentifersInScene, RPG.SceneManagement.DestinationIdentifer[] destinationIdentifersOutOfScene);

        public event Action OnDamageTaken;
        public event DestinationIdentifer OnBossDeath;

        private void Start() {
            GetComponent<BaseStats>().OnLevelUp += GetFullHealth;
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            print("damage is: " + damage );
            damagePoints += damage;
            if (ShouldDie())
            {
                Die();
                AwardPower(instigator);
            }

            // checking if the event is null before triggering.
            if (OnDamageTaken != null)
            {
                OnDamageTaken();
            }
        }

        private bool ShouldDie()
        {
            return damagePoints >= GetComponent<BaseStats>().GetStat(Stat.BaseHealth);
        }

        public (float, float) GetHealthPoints()
        {
            float maxHealth = GetComponent<BaseStats>().GetStat(Stat.BaseHealth);
            var healthAndMaxHealth = (maxHealth-damagePoints,maxHealth);
            return healthAndMaxHealth; 
        }

        public void GetFullHealth(){
            damagePoints = 0f;
        }

        public void GetHealth(float healthGained)
        {
            damagePoints -= Mathf.Min(healthGained,damagePoints);
        }

        public object CaptureState()
        {
            return damagePoints; //floats are serializable by default.
        }

        public void RestoreState(object state)
        {
            damagePoints = (float) state;

            if(ShouldDie()){
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
            if (isABoss != null && (isABoss.PortalsToEnableInScene != null || isABoss.PortalsToEnableInOtherScenes != null ))
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
            power.GainPower(GetComponent<BaseStats>().GetStat(Stat.PowerReward));
        }
    }    
}

