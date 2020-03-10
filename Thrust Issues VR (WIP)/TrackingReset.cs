using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TrackingReset : MonoBehaviour
{

    public SteamVR_Action_Boolean ResetTracking;
    public Hand LeftHand;

    [Tooltip("Desired head position of player when seated")]
    public Transform DesiredHeadPosition;

    public Transform SteamCamera;
    public Transform CameraRig;
    public Transform PlayerShip;
    GameControl GameControlScript;


    void OnEnable()
    {
        //UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        //ResetTracking.AddOnChangeListener(ResetButtonPressed, LeftHand.handType);
        Valve.VR.OpenVR.Compositor.SetTrackingSpace(Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);
        Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        UnityEngine.XR.InputTracking.Recenter();
    }

    private void Start()
    {
        GameControlScript = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl>();

        if (DesiredHeadPosition != null)
        {
            ResetSeatedPos(DesiredHeadPosition);

        }

    }

    void Update()
    {
        ResetButton();
    }

    private void ResetSeatedPos(Transform desiredHeadPos)
    {
        Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        UnityEngine.XR.InputTracking.Recenter();

        if ((SteamCamera != null) && (CameraRig != null))
        {
            //ROTATION
            // Get current head heading in scene (y-only, to avoid tilting the floor)
            float rigRotation = CameraRig.localRotation.eulerAngles.y;
            float cameraRotation = SteamCamera.localRotation.eulerAngles.y;
            // Now rotate CameraRig in opposite direction to compensate
            float offset = -(rigRotation + (cameraRotation - rigRotation));
            CameraRig.localRotation = Quaternion.Euler(0, offset, 0);

            //POSITION
            // Calculate postional offset between CameraRig and Camera
            Vector3 offsetPos = SteamCamera.position - CameraRig.position;
            // Reposition CameraRig to desired position minus offset
            CameraRig.position = (desiredHeadPos.position - offsetPos);
            //steamCamera.position = (desiredHeadPos.position - offsetPos);

        }

    }

    void ResetButton()
    {
        if (GameControlScript.Paused && ResetTracking.GetStateDown(LeftHand.handType))
        {
            if (DesiredHeadPosition != null)
            {
                ResetSeatedPos(DesiredHeadPosition);
            }

            GameControlScript.StartupResetPressed = true;

        }
    }

}

