using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class PowerDisplay : MonoBehaviour
    {
        Power power;

        private void Awake()
        {
            power = GameObject.FindWithTag("Player").GetComponent<Power>();
        }

        private void Update()
        {
            GetComponent<Text>().text = string.Format("{0:0}/{1:0}", power.GetLevelAndPoints().Item1, power.GetLevelAndPoints().Item2);
        }
    }
}