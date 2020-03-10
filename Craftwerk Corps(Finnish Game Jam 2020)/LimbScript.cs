using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Valve.VR;
using Valve.VR.InteractionSystem;
using DarkTonic.MasterAudio;

public class LimbScript : MonoBehaviour
{
    public Hand leftHand, rightHand;
    public int InteractCounter = 0;
    Vector3 offsetOnHit, offsetOnPlace, rotOffsetOnPlace;
    AttachScript attachScript;
    ConstraintSource constraintSource;
    GameControl GameControlScript;

    public bool isAttached;

    private void Awake()
    {

    }

    private void Start()
    {
        leftHand = GameObject.FindGameObjectWithTag("LeftHand").GetComponent<Hand>();
        rightHand = GameObject.FindGameObjectWithTag("RightHand").GetComponent<Hand>();
        GameControlScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.gameObject.CompareTag("LeftArm") && other.gameObject.CompareTag("LeftArmAttach"))
        {
            offsetOnHit = new Vector3(-0.005f, 0, 0);
            offsetOnPlace = new Vector3(0.012f, -0.38f, 0);
            SetInPlace(other.gameObject);

        }
        if (this.gameObject.CompareTag("RightArm") && other.gameObject.CompareTag("RightArmAttach"))
        {
            offsetOnHit = new Vector3(0.005f, 0, 0);
            offsetOnPlace = new Vector3(-0.035f, -0.38f, 0);
            SetInPlace(other.gameObject);

        }
        else if (this.gameObject.CompareTag("Head") && other.gameObject.CompareTag("HeadAttach"))
        {
            offsetOnHit = new Vector3(0, -0.01f, 0);
            offsetOnPlace = new Vector3(0, 0, 0);
            SetInPlace(other.gameObject);

        }
        else if (this.gameObject.CompareTag("RightLeg") && other.gameObject.CompareTag("RightLegAttach"))
        {
            offsetOnHit = new Vector3(0, 0.01f, 0);
            offsetOnPlace = new Vector3(0, -0.35f, 0);
            SetInPlace(other.gameObject);

        }
        else if (this.gameObject.CompareTag("LeftLeg") && other.gameObject.CompareTag("LeftLegAttach"))
        {
            offsetOnHit = new Vector3(0, 0.01f, 0);
            offsetOnPlace = new Vector3(0, -0.35f, 0);
            SetInPlace(other.gameObject);
        }
    }


    void SetInPlace(GameObject pAttachPoint)
    {

        MasterAudio.PlaySound3DAtTransformAndForget("LimbAttach", pAttachPoint.transform);

        this.gameObject.transform.position = pAttachPoint.transform.position;
        this.gameObject.transform.rotation = pAttachPoint.transform.rotation;

        this.gameObject.transform.position += offsetOnPlace;

        constraintSource.sourceTransform = pAttachPoint.transform;
        constraintSource.weight = 1;
        this.gameObject.GetComponent<ParentConstraint>().AddSource(constraintSource);

        //this.gameObject.transform.SetParent(pAttachPoint.transform);

        this.gameObject.GetComponent<Interactable>().enabled = false;
        Destroy(this.gameObject.GetComponent<Throwable>());
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        leftHand.DetachObject(this.gameObject);
        rightHand.DetachObject(this.gameObject);

        pAttachPoint.SetActive(false);

    }

    public void HammerItIn()
    {
        this.gameObject.transform.position += offsetOnHit;
        if (InteractCounter >= 3)
        {
            isAttached = true;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            MasterAudio.PlaySound3DAtTransformAndForget("Zombie1", this.gameObject.transform);
            this.gameObject.GetComponent<ParentConstraint>().constraintActive = true;
            GameControlScript.LimbsAttached++;
        }
    }


    private void OnDestroy()
    {

    }
}

