using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    public class HealthPickup : MonoBehaviour
    {
        [SerializeField] float healthAmount = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<Health>().GetHealth(healthAmount);
                Destroy(this.gameObject);
            }
        }
    }
}


