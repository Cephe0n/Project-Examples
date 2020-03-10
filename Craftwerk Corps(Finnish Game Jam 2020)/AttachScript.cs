using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class AttachScript : MonoBehaviour
{

    public Hand leftHand, rightHand;

    public bool ReadyToBeHammered;

    private void Start() 
    {
        leftHand = GameObject.FindGameObjectWithTag("LeftHand").GetComponent<Hand>();
        rightHand = GameObject.FindGameObjectWithTag("RightHand").GetComponent<Hand>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            ReadyToBeHammered = true;
            SetInPlace(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            ReadyToBeHammered = false;
        }
    }

    void SetInPlace(GameObject pLimb)
    {
        pLimb.transform.parent.position = this.transform.position;
        pLimb.transform.parent.rotation = this.transform.rotation;
        pLimb.gameObject.transform.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        pLimb.gameObject.transform.parent.GetComponent<Interactable>().enabled = false;
        Destroy(pLimb.gameObject.transform.parent.GetComponent<Throwable>());
        leftHand.DetachObject(pLimb.gameObject.transform.parent.gameObject);
        rightHand.DetachObject(pLimb.gameObject.transform.parent.gameObject);
    }
}
