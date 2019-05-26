using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 30f;
        [SerializeField] float aggroDistance = 15f;
        [SerializeField] Vector3 chaseOrigin;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;
        [Range (0,1)]
        [SerializeField] float patrolSpeedPercentage = 0.3f;

        Fighter fighter;
        Health health;
        GameObject player;
        Mover mover;

        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;
        bool isAttacking = false;
        
        private void Awake() {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
        }

        private void Start() {
            guardPosition = transform.position;
            SetChaseOrigin();
            health.OnDamageTaken+=DamageTakenHandler;
        }

        private void DamageTakenHandler()
        {
            if (fighter.CanAttack(player) && !isAttacking)
            {
                timeSinceLastSawPlayer = 0;
                AttackBehavior();
            }
        }

        private void SetChaseOrigin()
        {
            if(patrolPath != null){
                chaseOrigin=patrolPath.GetWaypoint(0);
            } else
            {
                chaseOrigin=transform.position;
            }
        }

        private void Update()
        {
            if (health.IsDead())
            {
                return;
            }

            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0;
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehavior()
        {
            isAttacking = false;
            Vector3 nextPosition = guardPosition;
            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if(timeSinceArrivedAtWaypoint > waypointDwellTime){
                mover.StartMoveAction(nextPosition, patrolSpeedPercentage);
            }
            
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position,GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
            isAttacking=false;
        }

        private void AttackBehavior()
        {
            isAttacking=true;
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer<aggroDistance;
        }

        // Called by unity
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
            Gizmos.DrawWireSphere(transform.position, aggroDistance);
        } 
    }
}

