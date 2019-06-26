using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Saving;
using RPG.Resources;
using UnityEngine.AI;
using RPG.Control;
using System.Collections.Generic;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifer destination = DestinationIdentifer.HomeBase;
        [SerializeField] DestinationIdentifer location= DestinationIdentifer.HomeBase;
        [SerializeField] float fadeOutTime = 0.5f;
        [SerializeField] float fadeInTime = 0.8f;
        [SerializeField] float fadeWaitTime= 0.5f;
        [SerializeField] bool isEnabled = false;

        public static Dictionary<DestinationIdentifer, Boolean> PortalsEnabled = new Dictionary<DestinationIdentifer, Boolean>();

        private void Awake() {
            if (!PortalsEnabled.ContainsKey(location))
            {
                PortalsEnabled.Add(location, isEnabled);
            }
            EnablePortal();
        }

        private void Start()
        {
            var possibleBosses = FindObjectOfType<BossBehavior>();
            if (possibleBosses != null)
            {
                possibleBosses.GetComponent<Health>().OnBossDeath += PortalEnablerHandler;
            }
            UpdatePortalsIfEnabled();
        }

        private void EnablePortal()
        {
            if (!isEnabled)
            {
                GetComponent<Collider>().enabled = false;
                foreach (Transform child in transform)
                {
                    foreach (ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>())
                    {
                        particleSystem.Stop();
                    }
                }
            } else {
                GetComponent<Collider>().enabled = true;
                foreach (Transform child in transform)
                {
                    foreach (ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>())
                    {
                        particleSystem.Play();
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.tag =="Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if(sceneToLoad<0){
                Debug.LogError("Scene to load not set");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>(); 

            yield return fader.Fadeout(fadeOutTime);

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            savingWrapper.Load();
            
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            savingWrapper.Save();
            
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            Destroy(gameObject);
        }

        private void UpdatePortalsIfEnabled()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                portal.isEnabled=PortalsEnabled[portal.location];
                portal.EnablePortal();
            }
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if(destination == portal.location){
                    return portal;
                }
            } 

            return null;
        }

        public void PortalEnablerHandler(DestinationIdentifer[] portalsToEnableInScene, DestinationIdentifer[] portalsToEnableOutOfScene )
        {
            foreach (DestinationIdentifer destinationIdentifer in portalsToEnableInScene)
            {
                foreach (Portal portal in FindObjectsOfType<Portal>())
                {
                    if (destinationIdentifer == portal.location)
                    {
                        portal.isEnabled=true;
                        portal.EnablePortal();
                        PortalsEnabled[portal.location]=true;
                    }
                }
            }
            foreach (DestinationIdentifer destinationIdentifer in portalsToEnableOutOfScene)
            {
                PortalsEnabled[destinationIdentifer]=true;
            }
        }
    }
}

