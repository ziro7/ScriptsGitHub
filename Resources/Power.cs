using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Resources
{
    public class Power : MonoBehaviour, ISaveable
    {
        [SerializeField] float powerPoints = 0;
        private float powerLevel = 1;

        public void GainPower(float powerGained)
        {
            powerPoints += powerGained;
        }

        public (float, float) GetLevelAndPoints()
        {
            var levelAndPoints = (powerLevel, powerPoints);
            return levelAndPoints;
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

