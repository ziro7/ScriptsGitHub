using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Saving;
using RPG.Resources;
using UnityEngine.AI;
using RPG.Control;

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

        private void Start()
        {
            var possibleBosses = FindObjectOfType<BossBehavior>();
            if (possibleBosses != null)
            {
                possibleBosses.GetComponent<Health>().OnBossDeath += PortalEnablerHandler;
            }
            EnablePortal();
        }

        private void EnablePortal()
        {
            if (!isEnabled)
            {
                GetComponent<Collider>().enabled = false;
            } else {
                GetComponent<Collider>().enabled = true;
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

        public void PortalEnablerHandler(DestinationIdentifer[] portalsToEnable, BossBehavior bossBehavior)
        {
            foreach (DestinationIdentifer destinationIdentifer in portalsToEnable)
            {
                Debug.Log("portal in destinationIdentifer: " + destinationIdentifer);
                foreach (Portal portal in FindObjectsOfType<Portal>())
                {
                    Debug.Log("portal in findObjects: " + portal.name);
                    if (destinationIdentifer == portal.location)
                    {
                        portal.isEnabled=true;
                        portal.EnablePortal();
                        Debug.Log("portal enabled: " + portal.name);
                    }
                }
            }
        }
    }
}

