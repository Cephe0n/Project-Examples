using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace Mechalon
{
    public class EnemyMechs : MonoBehaviour
    {

        public float TurnSpeed;
        private NavMeshAgent navAgent;

        //private int destPoint = 0;
        //public Transform[] navPoints;

        // Use this for enemyweapons DeviateShots function to make em miss sometimes
        public int ShotDeviation;

        // Use these in EnemyWeapons script to randomize a time between shots
        public float minTimeToFire;
        public float maxTimeToFire;

        private GameObject currentTarget;
        private TakeDamage takeDamage;
        private ThreatScript threatScript;
        public float FightDistance;
        public float MoveSpeed;
        Transform torso;
        private Vector3 smoothVelocity = Vector3.zero;

        float angle1;
        float angle2;
        float rad = 700;
        Vector3 strafeVector;
        GameObject[] targets;

        public int ACLevel;
        public int PLLevel;
        public int SGLevel;



        void Start()
        {
            takeDamage = gameObject.GetComponent<TakeDamage>();
            threatScript = gameObject.GetComponent<ThreatScript>();
            navAgent = gameObject.GetComponent<NavMeshAgent>();
            torso = transform.Find("hips/Torso");
            targets = threatScript.targets.Where(i => i != null && i != this.gameObject).ToArray();

            // Patrol();
        }

        private void Update()
        {
            /* 
            if (!takeDamage.Dead)
            {
                if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
                {
                    Patrol();
                }
            }
            */
            if (!takeDamage.Dead && gameObject != null && gameObject.activeInHierarchy)
            {

                InvokeRepeating("ChooseTarget", 0, 1.5f);

                if (currentTarget != null && currentTarget.activeInHierarchy)
                FollowTarget();

            }
            else
            {
                navAgent.isStopped = true;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }


        }


        /* 
        void Patrol()
        {
            if (navPoints.Length == 0)
                return;

            navAgent.destination = navPoints[destPoint].position;

            destPoint = (destPoint + 1) % navPoints.Length;
        }
        */

        void ChooseTarget()
        {
            // Go through targets array, remove inactive or null (destroyed) targets as well as self
            targets = threatScript.targets.Where(i => i != null && i.activeInHierarchy && i != this.gameObject).ToArray();

            threatScript.targets = targets;

            // Find the highest threat target in targets array, set as current target the 
            // enemy that has highest threat + is closest
            if (targets.Length > 0)
            {
                currentTarget = targets.Aggregate((i, j) =>
                                    i.GetComponent<ThreatScript>().Threat > j.GetComponent<ThreatScript>().Threat
                                    && Vector3.Distance(transform.position, i.transform.position) <
                                    Vector3.Distance(transform.position, j.transform.position)
                                     ? i : j);

            }

            else
            {
                // Stop searching for targets if none left
                CancelInvoke();
            }
            // If current target dead, null it to choose another
            if (currentTarget.GetComponent<TakeDamage>().Dead)
            {
                currentTarget = null;
            }
        }

        void FollowTarget()
        {
            if (targets.Length > 0)
            {
                // Smoothly rotates towards current target based on turn speed
                Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - torso.position);
                torso.rotation = Quaternion.Slerp(torso.rotation, targetRotation, TurnSpeed * Time.deltaTime);

                float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

                // If enemy farther away than fight distance, close the gap
                if (distance > FightDistance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, MoveSpeed * Time.deltaTime);
                }
                // If within fight distance, commence strafing
                else
                {
                    //transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, MoveSpeed * Time.deltaTime * -1);
                    StartCoroutine("Strafe");
                }
            }
        }

        IEnumerator Strafe()
        {
            // Two angles to strafe around, clockwise and counterclockwise
            angle1 -= Time.deltaTime;
            angle2 += Time.deltaTime;

            // Every time function is run, choose between the two angles randomly
            if (Random.Range(0, 2) == 0)
            {
                strafeVector = new Vector3((currentTarget.transform.position.x + Mathf.Sin(angle1) * rad), 
                                            currentTarget.transform.position.y, ((currentTarget.transform.position.z + Mathf.Cos(angle1) * rad)));
            }
            else
            {
                strafeVector = new Vector3((currentTarget.transform.position.x + Mathf.Sin(angle2) * rad),
                                             currentTarget.transform.position.y, ((currentTarget.transform.position.z + Mathf.Cos(angle2) * rad)));

            }

            // When direction is chosen, start strafing in circle around target.
            transform.position = Vector3.SmoothDamp(transform.position, strafeVector, ref smoothVelocity, MoveSpeed);

            // Choose new direction within a random time
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        }

    }
}
