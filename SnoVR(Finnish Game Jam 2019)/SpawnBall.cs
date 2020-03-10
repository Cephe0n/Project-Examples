using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace SnoVR
{
    public class SpawnBall : MonoBehaviour
    {

        GameObject snowBall;

        [SerializeField] GameObject leftHand;
        [SerializeField] GameObject rightHand;
        private Transform leftHandPos;
        private Transform rightHandPos;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            leftHandPos = leftHand.transform;
            rightHandPos = rightHand.transform;
        }

        public void GetBallLeft()
        {
            // if (ThrownBallScript.BallCreated == false)
            
                snowBall = BallPooler.SharedInstance.GetPlayerBall();
                if (snowBall != null)
                    snowBall.transform.position = leftHandPos.position;
                snowBall.SetActive(true);

                //ThrownBallScript.BallCreated = true;
            
        }

        public void GetBallRight()
        {
           // if (ThrownBallScript.BallCreated == false)
            
                snowBall = BallPooler.SharedInstance.GetPlayerBall();
                if (snowBall != null)
                    snowBall.transform.position = rightHandPos.position;
                snowBall.SetActive(true);

                // ThrownBallScript.BallCreated = true;
            
        }
        /* 
                public void BallGrabbed()
                {
                    snowBall.GetComponent<Rigidbody>().isKinematic = false;
                }

            
        */
    }
}