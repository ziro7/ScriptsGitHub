using UnityEngine;

namespace RPG.Resources
{
    public class Power : MonoBehaviour
    {
        [SerializeField] float powerPoints = 0;

        public void GainPower(float powerGained)
        {
            powerPoints += powerGained;
        }
    }    
}

