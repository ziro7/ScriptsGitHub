using RPG.Resources;
using RPG.Stats;
using UnityEngine;
using System;

namespace RPG.Combat
{
    public class Damage : MonoBehaviour
    {
        [SerializeField] float intMultiplier = 1.3f;
        [SerializeField] float strMultiplier = 1f;
        [SerializeField] float agiMultiplier = 0.5f;
        [SerializeField] float armorEffect= 0.1f;
        [SerializeField] float magicResistEffect = 0.1f;
        [SerializeField] float hitEqualLevel = 95f;
        [SerializeField] float hitChangePrLevel = 5f;
        [SerializeField] float dodgeFromAgility = 0.5f;
        [SerializeField] int randomMin = 85;
        [SerializeField] int randomMax = 115;

        float attackerLevel = 0;
        float attackerStrength = 0;
        float attackerAgility = 0;
        float attackerCritChange = 0;
        float attackerIntellect = 0;
        float weaponDamage = 0;
        float weaponSpeed = 0;

        // Defender stats
        float defenderLevel = 0;
        float defenderAgility = 0;
        float defenderArmor = 0;
        float defenderMagicResist = 0;

        BaseStats attackerStats = null;
        BaseStats defenderStats = null;

        System.Random random = new System.Random();

        public float CalculateDamage(GameObject instigator,Health target, Weapon weapon)
        {
            attackerStats = instigator.GetComponent<BaseStats>();
            attackerLevel = attackerStats.GetLevel();
            attackerCritChange = attackerStats.GetStat(Stat.CritPercentage);
            weaponDamage = weapon.WeaponDamage;
            weaponSpeed = weapon.WeaponSpeed;

            defenderStats = target.GetComponent<BaseStats>();
            defenderLevel = defenderStats.GetLevel();

            float hitChance = random.Next(1,100);
            if (hitChance <= (hitEqualLevel + hitChangePrLevel * (attackerLevel - defenderLevel))) 
            {
                if (weapon.IsMagicAttack)
                {
                    return CalculateMagicDamage();
                }
                else
                {
                    return CalculateMeleeDamage();
                }
            }
            else //miss
            {
                Debug.Log("Miss");
                return 0f;
            }
        }

        private float CalculateMagicDamage()
        {
            attackerIntellect = attackerStats.GetStat(Stat.Intellect);
            
            defenderMagicResist = defenderStats.GetStat(Stat.MagicResistance);

            float damageBeforeMagicResist = weaponDamage + intMultiplier * attackerIntellect;
            float damageReductionFromMagicResist = defenderMagicResist * magicResistEffect;
            float totaldamageBeforeCrit = (damageBeforeMagicResist - damageReductionFromMagicResist) * random.Next(randomMin, randomMax)/100;
            float critMultiplier = CritMultipler();

            return totaldamageBeforeCrit * critMultiplier * weaponSpeed;
        }

        private float CalculateMeleeDamage()
        {
            attackerStrength = attackerStats.GetStat(Stat.Strength);
            attackerAgility = attackerStats.GetStat(Stat.Agility);

            defenderAgility = defenderStats.GetStat(Stat.Agility);
            defenderArmor = defenderStats.GetStat(Stat.Armor);

            if (random.Next(1, 100) >= defenderAgility / dodgeFromAgility)
            {
                float damageBeforeArmor = weaponDamage + strMultiplier * attackerStrength + agiMultiplier * attackerAgility;
                float damageReductionFromArmor = defenderArmor * armorEffect;
                float totalDamageBeforeCrit = (damageBeforeArmor - damageReductionFromArmor) * random.Next(randomMin, randomMax)/100;
                float critMultiplier = CritMultipler();

                return totalDamageBeforeCrit * critMultiplier * weaponSpeed;
            } else //dodge
            {
                print("Dodge");
                return 0f;
            }
        }

        private float CritMultipler()
        {
            if (random.Next(1, 100) > (100 - attackerCritChange * 100))
            {
                return 1.5f;
            }
            else
            {
                return 1;
            }
        }
    }
}