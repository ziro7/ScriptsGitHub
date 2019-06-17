using RPG.Resources;
using RPG.Stats;
using UnityEngine;
using System;

namespace RPG.Combat
{
    public class Damage
    {
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

            if (hitChance <= (95 + 5 * (attackerLevel - defenderLevel)))
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
                return 0f;
            }
        }

        private float CalculateMagicDamage()
        {
            attackerIntellect = attackerStats.GetStat(Stat.Intellect);
            
            defenderMagicResist = defenderStats.GetStat(Stat.MagicResistance);

            float damageBeforeMagicResist = DamageBeforeMagicResist(); //weaponDamge + 1,3*totalInt
            float damageReductionFromMagicResist = DamageReductionFromMagicResist();//totalMagicResist/10
            float totaldamageBeforeCrit = (damageBeforeMagicResist - damageReductionFromMagicResist) * random.Next(85, 115)/100;
            float critMultiplier = CritMultipler();

            return totaldamageBeforeCrit * critMultiplier * weaponSpeed;
        }

        private float CalculateMeleeDamage()
        {
            attackerStrength = attackerStats.GetStat(Stat.Strength);
            attackerAgility = attackerStats.GetStat(Stat.Agility);

            defenderAgility = defenderStats.GetStat(Stat.Agility);
            defenderArmor = defenderStats.GetStat(Stat.Armor);

            if (random.Next(1, 100) >= defenderAgility / 2)
            {
                float damageBeforeArmor = DamageBeforeArmor(); //weaponDamge + 1*totalstr + 0,5*totalagi
                float damageReductionFromArmor = DamageReductionFromArmor();//totalArmor/10
                float totaldamageBeforeCrit = (damageBeforeArmor - damageReductionFromArmor) * random.Next(85, 115)/100;
                float critMultiplier = CritMultipler();

                return totaldamageBeforeCrit * critMultiplier * weaponSpeed;
            } else //dodge
            {
                return 0f;
            }
        }

        private float DamageBeforeMagicResist()
        {
            return weaponDamage + 1.3f * attackerIntellect;
        }

        private float DamageReductionFromMagicResist()
        {
            return defenderMagicResist / 10f;
        }

        private float DamageBeforeArmor()
        {
            return weaponDamage + attackerStrength + 0.5f * attackerAgility;
        }

        private float DamageReductionFromArmor()
        {
            return defenderArmor / 10f;
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