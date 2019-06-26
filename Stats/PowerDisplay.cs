using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class PowerDisplay : MonoBehaviour
    {
        Power power;
        BaseStats baseStats;
        GameObject player;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player"); 
            power = player.GetComponent<Power>();
            baseStats = player.GetComponent<BaseStats>();
        }

        private void Update()
        {
            GetComponent<Text>().text = string.Format("{0:0}/{1:0}", baseStats.GetLevel(),power.GetPowerPoints());
        }
    }
}