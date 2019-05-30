using System;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float healthPoints;
        private bool isDead = false;

        public event Action OnDamageTaken;

        private void Start() {
            GetFullHealth();
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(float damage){
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints <= 0)
            {
                Die();
            }
            
            // checking if the event is null before triggering.
            if(OnDamageTaken!=null){
                OnDamageTaken();
            } 
        }

        public void GetFullHealth(){
            healthPoints = GetComponent<BaseStats>().GetHealth();
        }

        private void Die()
        {
            if(isDead){
                return;
            }
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
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
    }    
}

