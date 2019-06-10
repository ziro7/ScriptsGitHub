using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Header("Stats")]
        [Range(1,7)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        // TO DO move to Power and have a method that is called when a level is gained in order to do particle effekts.
        public int GetLevel()
        {
            Power power = GetComponent<Power>();

            if (power == null) return startingLevel; // Enemies

            float currentPower = power.GetPowerPoints();
            int penultimateLevel = progression.GetLevels(Stat.PowerToLevelUp, characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                float powerToLevelUp = progression.GetStat(Stat.PowerToLevelUp, characterClass, level);
                if(powerToLevelUp>currentPower){
                    return level;
                }
            }

            return penultimateLevel+1;

        }
    }
}

