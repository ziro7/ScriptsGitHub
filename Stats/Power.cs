using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Power : MonoBehaviour, ISaveable
    {
        [SerializeField] float powerPoints = 0;

        public event Action OnPowerGained;

        public void GainPower(float powerGained)
        {
            powerPoints += powerGained;
            OnPowerGained();
        }

        public float GetPowerPoints()
        {
            return powerPoints;
        }

        public object CaptureState()
        {
            return powerPoints;
        }

        public void RestoreState(object state)
        {
            powerPoints = (float) state;
        }
    }    
}

