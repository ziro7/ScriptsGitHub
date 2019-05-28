using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float startingHealth = 100f;
        [SerializeField] float healthPoints = 100f;
        private bool isDead = false;

        public event Action OnDamageTaken;

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

        public void ReturnToFullHealth(){
            healthPoints = startingHealth;
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

