using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Resources;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Control
{
    public class BossBehavior : MonoBehaviour
    {
        [SerializeField] GameObject minionToSpawn;
        [SerializeField] float waitForSpawns = 5f;
        [SerializeField] int amountToSpawn = 5;
        [SerializeField] List<GameObject> spawnPoints;
        [SerializeField] DestinationIdentifer[] portalsToEnableInScene = null;
        [SerializeField] DestinationIdentifer[] portalsToEnableInOtherScenes = null;
        List<GameObject> spawns = new List<GameObject>();  
        int poolCapacity = 15; 
        GameObject player;
        bool isEngaged = false;

        public DestinationIdentifer[] PortalsToEnable { get => portalsToEnableInOtherScenes; private set => portalsToEnableInOtherScenes = value; }
        public DestinationIdentifer[] PortalsToEnableInOtherScenes { get => portalsToEnableInOtherScenes; set => portalsToEnableInOtherScenes = value; }

        private void Awake() 
        {
            Assert.IsNotNull(minionToSpawn);
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Start() {
            PoolDictionary.AddPool(minionToSpawn.name, () => SpawnMethod(), 25);
        }

        private void Update() {
            if (GetComponent<Fighter>().IsAttacking && !isEngaged) 
            {
                isEngaged = true;
                StartCoroutine(SpecialSpawnAttack());
            }
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            foreach (var enemy in spawns)
            {
                if(GetComponent<Health>().IsDead()){
                    enemy.SetActive(false);
                    // PoolDictionary takes an Interface, so have to case to specific queue type to call method.
                    ((QueuePool<GameObject>)PoolDictionary.pools[minionToSpawn.name]).ReturnInstanceToPool(enemy);
                }
            }
        }

        private IEnumerator SpecialSpawnAttack(){
            System.Random random = new System.Random();

            while(!player.GetComponent<Health>().IsDead()){
                for (int i = 0; i < amountToSpawn; i++)
                {
                    GameObject spawn = PoolDictionary.pools[minionToSpawn.name].GetInstance();
                    int spawnLocation = random.Next(0, spawnPoints.Count);
                    spawn.transform.position = spawnPoints[spawnLocation].transform.position;
                    spawn.transform.rotation = spawnPoints[spawnLocation].transform.rotation;
                    spawn.GetComponent<Health>().GetFullHealth();
                    spawn.SetActive(true);
                    spawn.GetComponent<Fighter>().Attack(player);
                    spawns.Add(spawn);
                }
                yield return new WaitForSeconds(waitForSpawns);
            }  
        }

        private GameObject SpawnMethod(){
            GameObject objectToSpawn = Instantiate(minionToSpawn);
            objectToSpawn.name = minionToSpawn.name;
            objectToSpawn.SetActive(false);
            return objectToSpawn;
        }
    }    
}

