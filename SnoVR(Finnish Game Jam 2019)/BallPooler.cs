using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnoVR
{
    public class BallPooler : MonoBehaviour
    {

        public static BallPooler SharedInstance;

        [SerializeField] List<GameObject> pooledPlayerBalls;
        [SerializeField] List<GameObject> pooledEnemyBalls;

        [SerializeField] GameObject playerBall;
        [SerializeField] GameObject enemyBall;

        [SerializeField] int amountToPool;

        void Awake()
        {
            SharedInstance = this;
        }


        void Start()
        {

            pooledPlayerBalls = new List<GameObject>();

            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(playerBall);
                obj.SetActive(false);
                pooledPlayerBalls.Add(obj);
            }


            pooledEnemyBalls = new List<GameObject>();

            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj2 = (GameObject)Instantiate(enemyBall);
                obj2.SetActive(false);
                pooledEnemyBalls.Add(obj2);
            }


        }


        public GameObject GetPlayerBall()
        {

            for (int i = 0; i < pooledPlayerBalls.Count; i++)
            {

                if (!pooledPlayerBalls[i].activeInHierarchy)
                {
                    return pooledPlayerBalls[i];
                }
            }

            return null;
        }

        public GameObject GetEnemyBall()
        {

            for (int i = 0; i < pooledEnemyBalls.Count; i++)
            {

                if (!pooledEnemyBalls[i].activeInHierarchy)
                {
                    return pooledEnemyBalls[i];
                }
            }

            return null;
        }

    }
}