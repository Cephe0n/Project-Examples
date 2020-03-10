using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SnoVR
{
    public class EnemyControl : MonoBehaviour
    {
        Rigidbody enemyRb;
        GameObject snowBall;
        public static int EnemyHealth { get; set; }
        [SerializeField] Transform hand;
        [SerializeField] Transform player;

        // Use this for initialization
        void Start()
        {
            enemyRb = this.gameObject.GetComponent<Rigidbody>();
            EnemyHealth = 5;
            InvokeRepeating("Throw", 2, 2);
        }

        // Update is called once per frame
        void Update()
        {
            Die();
        }

        void Die()
        {
            if (EnemyHealth <= 0)
            {
               enemyRb.DORotate(new Vector3(90,0,0), 2, RotateMode.Fast);
            }
        }

        void Throw()
        {
            if (EnemyHealth > 0)
            {
                snowBall = BallPooler.SharedInstance.GetEnemyBall();

                if (snowBall != null)
                    snowBall.transform.position = hand.position;

                snowBall.SetActive(true);

                Transform playerPos = player;
                snowBall.GetComponent<Rigidbody>().DOJump(playerPos.position, 3, 1, 1f, false);
            }
        }
    }
}