using System;
using RPG.Control;
using RPG.Core;
using RPG.Saving;
using RPG.SceneManagement;
using RPG.Stats;
using UnityEngine;

namespace RPG.Resources
{
    //[Serializable]
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float> healthPoints;
        private bool isDead = false;
        [SerializeField] float healthPointsFromStamina = 10f;

        public delegate void DestinationIdentifer (RPG.SceneManagement.DestinationIdentifer[] destinationIdentifersInScene, RPG.SceneManagement.DestinationIdentifer[] destinationIdentifersOutOfScene);
        public event Action OnDamageTaken;
        public event DestinationIdentifer OnBossDeath;

        private void Awake() {
            healthPoints = new LazyValue<float>(GetIntialHealth);
        }

        private float GetIntialHealth()
        {
            float baseHealth = GetComponent<BaseStats>().GetStat(Stat.BaseHealth);
            float stamina = GetComponent<BaseStats>().GetStat(Stat.Stamina);
            return baseHealth + stamina*healthPointsFromStamina;
        }

        private void Start() {
            healthPoints.ForceInit();
        }
        
        private void OnEnable() {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }

        private void OnDisable() 
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            if (healthPoints.value == 0)
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

        public float GetCurrentHealthPoints()
        {
            return healthPoints.value; 
        }

        public float GetMaxHealthPoints(){
            return GetIntialHealth();
        }

        public void GetHealth(float healthGained)
        {
            if((healthGained + healthPoints.value) >= GetComponent<BaseStats>().GetStat(Stat.BaseHealth)){
                healthPoints.value = GetComponent<BaseStats>().GetStat(Stat.BaseHealth);
            } else
            {
                healthPoints.value += healthGained;
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

        public void GetFullHealth()
        {
            healthPoints.value = GetIntialHealth();
        }

        private void RegenerateHealth()
        {
            healthPoints.value = GetIntialHealth();
        }

        public object CaptureState()
        {
            return healthPoints.value; //floats are serializable by default.
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            if (healthPoints.value <= 0)
            {
                Die();
            }
        }

    }    
}

