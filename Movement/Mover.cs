﻿using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        NavMeshAgent NavMeshAgent;
        Health health;
        // Update is called once per frame
        private void Start() {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        
        void Update()
        {
            NavMeshAgent.enabled = !health.IsDead(); 
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedPercentage = 1){
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedPercentage);
        }


        public void MoveTo(Vector3 destination, float speedPercentage = 1)
        {
            NavMeshAgent.destination = destination;
            NavMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedPercentage);
            NavMeshAgent.isStopped = false;
        }

        public void Cancel(){
            NavMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = NavMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3) state;
            GetComponent<NavMeshAgent>().enabled = false; //avoid a known bug where the navmesh can cause issues if enabled before setting position.
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}


