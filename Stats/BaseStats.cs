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
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] AudioClip levelUpSfx = null;

        AudioSource audioSource;
        public event Action OnLevelUp;

        int currentLevel = 0;

        private void Awake() {
            audioSource = GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();
        }

        private void Start() {
            currentLevel = CalculateLevel();
            Power power = GetComponent<Power>();
            if(power != null){
                power.OnPowerGained += PowerGainedHandler;
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStats(stat) + GetAdditiveModifers(stat))* (1+GetPercentageModifiers(stat)/100);
        }

        public int GetLevel()
        {
            return currentLevel;
        }

        private float GetBaseStats(Stat stat) => progression.GetStat(stat, characterClass, GetLevel());

        private float GetAdditiveModifers(Stat stat)
        {
            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifiers(Stat stat)
        {
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private void PowerGainedHandler()
        {
            int newLevel = CalculateLevel();

            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUpEffect();
                OnLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
            audioSource.PlayOneShot(levelUpSfx);
        }

        private int CalculateLevel()
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

