using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    public class HealthPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] float healthAmount = 5f;
        [SerializeField] float respawnTime = 1f;

        // TODO Combine Health and Weapon Pick up - Violate DRY atm.
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.GetComponent<Health>());
            }
        }

        private void Pickup(Health health)
        {
            health.GetHealth(healthAmount);
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(respawnTime);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.GetComponent<Health>());
            }
            return true;
        }
    }
}


