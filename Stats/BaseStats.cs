using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Header("Stats")]
        [Range(1,10)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
    }
}

