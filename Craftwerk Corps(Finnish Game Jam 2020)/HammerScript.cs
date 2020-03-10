using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using DarkTonic.MasterAudio;

public class HammerScript : MonoBehaviour
{
    public Hand leftHand, rightHand;
    public Transform spawnPoint, playerPos;
    LimbScript limbScript;
    bool readyToHammer = true;

    // Start is called before the first frame update
    void Start()
    {
        leftHand = GameObject.FindGameObjectWithTag("LeftHand").GetComponent<Hand>();
        rightHand = GameObject.FindGameObjectWithTag("RightHand").GetComponent<Hand>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftHand.ObjectIsAttached(this.gameObject) && !rightHand.ObjectIsAttached(this.gameObject))
        {
            if (Vector3.Distance(this.gameObject.transform.position, playerPos.position) > 5f)
            {
                this.gameObject.transform.position = spawnPoint.position;
                this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!readyToHammer)
            return;

        if (other.gameObject.CompareTag("HammerHitPoint"))
        {
            MasterAudio.PlaySound3DAtTransformAndForget("HammerHit1", this.gameObject.transform);
            readyToHammer = false;

            limbScript = other.gameObject.transform.parent.GetComponent<LimbScript>();

            if (limbScript.InteractCounter < 3)
            {
                limbScript.InteractCounter++;
                limbScript.HammerItIn();
                Debug.Log(limbScript.InteractCounter);
            }

            StartCoroutine(readyHammer());
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
        this.gameObject.transform.position = spawnPoint.position;
        this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }

    IEnumerator readyHammer()
    {
        yield return new WaitForSeconds(0.3f);
        readyToHammer = true;
    }

}
