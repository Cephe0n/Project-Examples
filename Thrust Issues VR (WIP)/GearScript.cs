using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;
using DarkTonic.MasterAudio;
using DG.Tweening;

public class GearScript : MonoBehaviour
{

    public SteamVR_Action_Vibration GearHaptic;
    public PlayerShips ShipScript;
    public float CurrentGear = 0;
    public Text GearText;


    void Update()
    {
        ShipScript.CurrentGear = CurrentGear;
    }



    private void OnTriggerEnter(Collider other)
    {

        if (ShipScript.LeftHand.ObjectIsAttached(this.gameObject) || ShipScript.RightHand.ObjectIsAttached(this.gameObject))
        {
            if (other.CompareTag("GearN") && CurrentGear != 0)
            {
                if (ShipScript.LeftHand.ObjectIsAttached(this.gameObject))
                    GearHaptic.Execute(0, 0.1f, 5, 0.3f, SteamVR_Input_Sources.LeftHand);
                else if (ShipScript.RightHand.ObjectIsAttached(this.gameObject))
                    GearHaptic.Execute(0, 0.1f, 5, 0.3f, SteamVR_Input_Sources.RightHand);

                CurrentGear = 0;

                GearText.color = Color.white;

            }
            if (other.CompareTag("Gear1") && CurrentGear != 1)
            {
                if (ShipScript.LeftHand.ObjectIsAttached(this.gameObject))
                    GearHaptic.Execute(0, 0.2f, 15, 0.6f, SteamVR_Input_Sources.LeftHand);
                else if (ShipScript.RightHand.ObjectIsAttached(this.gameObject))
                    GearHaptic.Execute(0, 0.2f, 15, 0.6f, SteamVR_Input_Sources.RightHand);

                CurrentGear = 1;

                GearText.color = Color.green;

            }
            if (other.CompareTag("Gear2") && CurrentGear != 2)
            {
                if (ShipScript.LeftHand.ObjectIsAttached(this.gameObject))
                    GearHaptic.Execute(0, 0.3f, 30, 0.9f, SteamVR_Input_Sources.LeftHand);
                else if (ShipScript.RightHand.ObjectIsAttached(this.gameObject))
                    GearHaptic.Execute(0, 0.3f, 30, 0.9f, SteamVR_Input_Sources.RightHand);

                CurrentGear = 2;

                GearText.color = Color.yellow;


            }
            if (other.CompareTag("Gear3") && CurrentGear != 3)
            {
                if (ShipScript.LeftHand.ObjectIsAttached(this.gameObject))
                    GearHaptic.Execute(0, 0.4f, 50, 1.2f, SteamVR_Input_Sources.LeftHand);
                else if (ShipScript.RightHand.ObjectIsAttached(this.gameObject))
                    GearHaptic.Execute(0, 0.4f, 50, 1.2f, SteamVR_Input_Sources.RightHand);

                CurrentGear = 3;

                GearText.color = Color.red;

            }


            if (ShipScript.GameControlScript.ShipRunning)
            ChangeGearText();
        }

    }

    void ChangeGearText()
    {
        GearText.text = "Gear: " + CurrentGear;

        if (CurrentGear == 0)
        GearText.text = "Gear: N";
    }
}
