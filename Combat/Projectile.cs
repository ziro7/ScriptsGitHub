using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] Transform target = null;
        [SerializeField] float speed = 1;

        // Update is called once per frame
        void Update()
        {
            if(target == null) return;

            transform.LookAt(target.position);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        /* Used for offset if the targets center was not equal to the aim location - but it was.       
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null){
                return target.position;
            }
            return target.position + Vector3.up * targetCapsule.height / 2;
        } 
        */

    }
}


