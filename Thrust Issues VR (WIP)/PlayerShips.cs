using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using DarkTonic.MasterAudio;
using DG.Tweening;

public class PlayerShips : MonoBehaviour
{
    // Objects with the SteamVR linear mapping script (for the levers movement)
    public GameObject LeftLinearMap, RightLinearMap;

    //The throttle objects for spawning into your hand
    public GameObject LeftThrottleObjects, RightThrottleObjects, LeftThrottle, RightThrottle;
    public GameObject CollidersWhenRunning, CollidersWhenShutdown;
    public Image BoostBar, LThrustBar, RThrustBar, LStrafeBar, RStrafeBar;
    public Text SpeedText;

    public Transform Ray1Pos, Ray2Pos, LeftHandGrabPos, RightHandGrabPos, RightEnginePos, LeftEnginePos, LeftStraferPos, RightStraferPos;
    // The current amount of power (0 - 1) given to each thruster, or both combined. Based on throttle position
    protected float leftThrustCurrent, rightThrustCurrent, bothThrustCurrent;

    // CUrrent amount of power (0 - 1) given for turning the ship. Based on throttle positions relative to each other
    protected float torqueCurrent;

    public float MaxSpeed;

    [Tooltip("How much max speed is multiplied while boosted")]
    public float BoostSpeedMulti;

    [Tooltip("How much acceleration is multiplied while boosted")]
    public float BoostAccelMulti;

    [Tooltip("The current amount of boost fuel")]
    public float BoostAmount;

    [Tooltip("The maximum amount of boost fuel")]
    public float BoostAmountMax;

    [Tooltip("How fast boost fuel degens in use")]
    public float BoostUsageSpeed;
    [Tooltip("How fast boost fuel regens when not in use")]
    public float BoostRegenSpeed;

    [Tooltip("The ships acceleration speed")]
    public float AccelMultip;

    [Tooltip("The ships turning speed (use small numbers)")]
    public float TorqueMultip;

    [Tooltip("The ships strafe speed")]
    public float StrafeMultip;

    public Rigidbody playerShipRB;


    [Tooltip("How high above the floor the ship floats")]
    public float HoverHeight;

    [Tooltip("The amount of force applied to keep the ship afloat")]
    public float HoverForce;

    [Tooltip("The amount of force pushing the ship down when in the air")]
    public float DownForce;

    public float CurrentGear;

    // False if player is in the air
    protected bool grounded;

    // Used to align the ship to the ground normal
    Quaternion rotToNormal;

    // The starting values for reset purposes
    [HideInInspector]
    public float startingMaxSpeed, startingAccelMulti, startingStrafeMulti, startingTorqueMulti;

    float currentSpeed, acceleration;
    float horizontalInputKB, horizontalInputJS, L2state, R2state, VRTriggerInputLeft, VRTriggerInputRight = 0;
    bool boostPressed, boostAdded, boostPadHit, startupSoundPlayed,
         shutdownSoundPlayed = false;
    public SteamVR_Action_Boolean BoostInput;
    public SteamVR_Action_Single StrafeLeftInput, StrafeRightInput;
    public SteamVR_Action_Vibration throttleHaptic;
    public Hand RightHand, LeftHand;
    public GameControl GameControlScript;
    private BoostPad BoostPadScript;


    protected virtual void Start()
    {

        GameControlScript = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl>();
        startingMaxSpeed = MaxSpeed;
        startingAccelMulti = AccelMultip;
        startingTorqueMulti = TorqueMultip;
        startingStrafeMulti = StrafeMultip;
        leftThrustCurrent = 0;
        rightThrustCurrent = 0;
        grounded = true;
        playerShipRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        SpeedText.color = Color.green;
        MasterAudio.PlaySound3DFollowTransformAndForget("JumpJetLoopL", LeftEnginePos);
        MasterAudio.PlaySound3DFollowTransformAndForget("JumpJetLoopR", RightEnginePos);
        MasterAudio.PlaySound3DFollowTransformAndForget("strafeloopL", RightStraferPos);
        MasterAudio.PlaySound3DFollowTransformAndForget("strafeloopR", LeftStraferPos);
    }

    protected virtual void Update()
    {

        if (GameControlScript.ShipRunning && !GameControlScript.Paused)
        {
            CheckInputs();
            Boost();
        }

        HUDinstruments();

        if (grounded)
            Debug.Log("on the ground");
        else
            Debug.Log("in the air");

    }

    protected virtual void FixedUpdate()
    {
        if (GameControlScript.ShipRunning)
        {
            if (CurrentGear != 0)
            {
                Thrust();
                Strafe();
            }
            Hover();

        }

        if (grounded)
        {
            playerShipRB.constraints |= RigidbodyConstraints.FreezeRotationX;
            playerShipRB.constraints |= RigidbodyConstraints.FreezeRotationZ;
            playerShipRB.constraints &= RigidbodyConstraints.FreezeRotationY;
        }
        else
        {
            playerShipRB.constraints |= RigidbodyConstraints.FreezeRotationX;
            playerShipRB.constraints |= RigidbodyConstraints.FreezeRotationZ;
            playerShipRB.constraints |= RigidbodyConstraints.FreezeRotationY;
        }
    }



    void CheckInputs()
    {

        VRTriggerInputLeft = StrafeLeftInput.GetAxis(LeftHand.handType);

        VRTriggerInputRight = StrafeRightInput.GetAxis(RightHand.handType);

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Joystick1Button1)
            || BoostInput.GetStateDown(RightHand.handType) && leftThrustCurrent > 0)
            boostPressed = true;

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Joystick1Button1)
            || BoostInput.GetStateUp(RightHand.handType))
            boostPressed = false;

        horizontalInputKB = Input.GetAxis("Horizontal");

        if (Input.GetJoystickNames().Length > 2)
        {
            horizontalInputJS = Input.GetAxis("HorizontalStick");

            if (Input.GetAxis("L2") == -1f)
            {
                L2state = 0f;
            }
            else if (Input.GetAxis("L2") < 0)
            {
                L2state = (1f - Mathf.Abs(Input.GetAxis("L2"))) / 2;
            }
            else
            {
                L2state = (1f + Input.GetAxis("L2")) / 2;
            }

            if (Input.GetAxis("R2") == -1f)
            {
                R2state = 0f;
            }
            else if (Input.GetAxis("R2") < 0)
            {
                R2state = (1f - Mathf.Abs(Input.GetAxis("R2"))) / 2;
            }
            else
            {
                R2state = (1f + Input.GetAxis("R2")) / 2;
            }
        }
    }


    public void Ignition()
    {

        if (!GameControlScript.ShipRunning && !startupSoundPlayed && GameControlScript.IgnitionLeverValue == 1)
        {
            MasterAudio.PlaySound3DFollowTransformAndForget("WarmupSequence", this.gameObject.transform);
            MasterAudio.PlaySound3DFollowTransformAndForget("EngineLoop", this.gameObject.transform);

            startupSoundPlayed = true;
            shutdownSoundPlayed = false;
            GameControlScript.ActivateShip();
            CollidersWhenShutdown.SetActive(false);
            CollidersWhenRunning.SetActive(true);
        }


        else if (GameControlScript.ShipRunning && !shutdownSoundPlayed && GameControlScript.IgnitionLeverValue == 0)
        {
            MasterAudio.PlaySound3DFollowTransformAndForget("Shutdown", this.gameObject.transform);
            MasterAudio.StopSoundGroupOfTransform(this.gameObject.transform, "EngineLoop");
            shutdownSoundPlayed = true;
            startupSoundPlayed = false;
            GameControlScript.DeactivateShip();
            CollidersWhenShutdown.SetActive(true);
            CollidersWhenRunning.SetActive(false);
        }


    }


    void Hover()
    {
        Ray hoverRay1 = new Ray(Ray1Pos.position, -Ray1Pos.up);
        Ray hoverRay2 = new Ray(Ray2Pos.position, -Ray2Pos.up);
        RaycastHit hit;

        Vector3 currentRot = transform.rotation.eulerAngles;

        Debug.DrawRay(Ray1Pos.position, -Ray1Pos.up, Color.green);
        Debug.DrawRay(Ray2Pos.position, -Ray2Pos.up, Color.green);

        if (Physics.Raycast(hoverRay1, out hit, HoverHeight) || Physics.Raycast(hoverRay2, out hit, HoverHeight))
        {

            grounded = true;

            // Get the ground normal and smoothly align the ship to it
            rotToNormal = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotToNormal, Time.deltaTime * 2);

            // Add a force below the ship to keep it afloat
            float proportionalHeight = (HoverHeight - hit.distance);
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * HoverForce;
            playerShipRB.AddRelativeForce(appliedHoverForce, ForceMode.Impulse);

        }
        else
        {
            grounded = false;

            // If in the air, add force from above to bring the ship down faster
            Vector3 appliedDownForce = Vector3.down * DownForce;
            playerShipRB.AddForce(appliedDownForce, ForceMode.Acceleration);

            // if the ship is in the air (not from a small bounce), reset x and z rotation
            if (transform.position.y > hit.distance + 10f)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, currentRot.y, 0), Time.deltaTime * 2f);
        }
    }

    void Thrust()
    {

        float leftThrottlePos;
        float rightThrottlePos;

        leftThrottlePos = LeftLinearMap.GetComponent<LinearMapping>().value;
        rightThrottlePos = RightLinearMap.GetComponent<LinearMapping>().value;

        if (LeftHand.ObjectIsAttached(LeftThrottle))
            throttleHaptic.Execute(0, 1f, 0.1f, leftThrottlePos / 3, SteamVR_Input_Sources.LeftHand);

        if (RightHand.ObjectIsAttached(RightThrottle))
            throttleHaptic.Execute(0, 1f, 0.1f, rightThrottlePos / 3, SteamVR_Input_Sources.RightHand);

        // Get the throttle amounts
        if (leftThrottlePos == 0 && rightThrottlePos == 0)
        {
            leftThrustCurrent = Input.GetKey(KeyCode.Mouse1) ? 1 : R2state;
            rightThrustCurrent = Input.GetKey(KeyCode.Mouse0) ? 1 : L2state;
        }
        else
        {
            leftThrustCurrent = leftThrottlePos;
            rightThrustCurrent = rightThrottlePos;
        }

        MasterAudio.SetGroupVolume("JumpJetLoopL", leftThrustCurrent);
        MasterAudio.SetGroupVolume("JumpJetLoopR", rightThrustCurrent);

        if (leftThrustCurrent > 0 && rightThrustCurrent > 0)
        {
            bothThrustCurrent = leftThrustCurrent + rightThrustCurrent;
        }
        else
        {
            bothThrustCurrent = 0;
        }

        // When one throttle is higher than the other, the current turn speed is the difference between them
        // Negative numbers for turning the other way 
        if (leftThrustCurrent > rightThrustCurrent)
        {
            torqueCurrent = leftThrustCurrent - rightThrustCurrent;

        }
        else if (rightThrustCurrent > leftThrustCurrent)
        {
            torqueCurrent = 0 - (rightThrustCurrent - leftThrustCurrent);
        }
        else
        {
            torqueCurrent = 0;
        }

        // Rotate the ship based on current turn speed
        Vector3 torqueVector = new Vector3(0, torqueCurrent, 0);
        this.transform.Rotate(torqueVector * TorqueMultip * GameControlScript.IgnitionLeverValue, Space.Self);


        acceleration = (bothThrustCurrent * AccelMultip * 1000) * Time.deltaTime;

        // Move ship forward by the combined amount of thrusters + multiplier (up to max speed)
        // If in the air, lower the thrust force
        if (playerShipRB.velocity.magnitude < MaxSpeed)
        {
            if (grounded)
                playerShipRB.AddRelativeForce(Vector3.forward * acceleration * GameControlScript.IgnitionLeverValue, ForceMode.Impulse);
            else
                playerShipRB.AddRelativeForce(Vector3.forward * acceleration * GameControlScript.IgnitionLeverValue * 0.8f, ForceMode.Impulse);
        }
        else
        {
            playerShipRB.velocity = playerShipRB.velocity.normalized * MaxSpeed;
        }

    }




    void Strafe()
    {

        if (VRTriggerInputLeft > 0 && LeftHand.ObjectIsAttached(LeftThrottle))
        {
            playerShipRB.AddRelativeForce(Vector3.left * StrafeMultip * VRTriggerInputLeft * GameControlScript.IgnitionLeverValue, ForceMode.Impulse);
            MasterAudio.SetGroupVolume("strafeloopL", VRTriggerInputLeft * 0.7f);
        }
        else
            MasterAudio.SetGroupVolume("strafeloopL", 0);

        if (VRTriggerInputRight > 0 && RightHand.ObjectIsAttached(RightThrottle))
        {
            playerShipRB.AddRelativeForce(Vector3.right * StrafeMultip * VRTriggerInputRight * GameControlScript.IgnitionLeverValue, ForceMode.Impulse);
            MasterAudio.SetGroupVolume("strafeloopR", VRTriggerInputRight * 0.7f);
        }
        else
            MasterAudio.SetGroupVolume("strafeloopR", 0);
    }


    void Boost()
    {

        if (boostPressed && BoostAmount > 0 && bothThrustCurrent > 0)
        {
            BoostAmount -= BoostUsageSpeed * Time.deltaTime;

            if (!MasterAudio.IsSoundGroupPlaying("HL_FireLoop"))
                MasterAudio.PlaySound3DFollowTransformAndForget("HL_FireLoop", this.gameObject.transform);

            if (!boostAdded)
            {
                TorqueMultip *= 0.5f;
                StrafeMultip *= 0.5f;
                MaxSpeed *= BoostSpeedMulti;
                AccelMultip *= BoostAccelMulti;
                SpeedText.color = Color.red;
                boostAdded = true;
            }

        }
        else
            MasterAudio.StopAllOfSound("HL_FireLoop");

        if (!boostPressed && !boostPadHit)
        {
            SpeedText.color = Color.green;
            boostAdded = false;

            switch (CurrentGear)
            {
                case 0:
                    MaxSpeed = startingMaxSpeed;
                    AccelMultip = startingAccelMulti;
                    TorqueMultip = startingTorqueMulti;
                    StrafeMultip = startingStrafeMulti;
                    break;

                case 1:
                    MaxSpeed = startingMaxSpeed * 0.7f;
                    AccelMultip = startingAccelMulti * 1.3f;
                    TorqueMultip = startingTorqueMulti + 0.4f;
                    StrafeMultip = startingStrafeMulti + 10;
                    break;

                case 2:
                    MaxSpeed = startingMaxSpeed;
                    AccelMultip = startingAccelMulti;
                    TorqueMultip = startingTorqueMulti;
                    StrafeMultip = startingStrafeMulti;
                    break;

                case 3:
                    MaxSpeed = startingMaxSpeed * 1.3f;
                    AccelMultip = startingAccelMulti * 1.3f;
                    TorqueMultip = startingTorqueMulti - 0.20f;
                    break;
            }
        }

        if (BoostAmount < 0)
        {
            BoostAmount = 0;
            boostPressed = false;
        }


        if (!boostPressed && BoostAmount < BoostAmountMax)
            BoostAmount += BoostRegenSpeed * Time.deltaTime;

        BoostBar.DOFillAmount(BoostAmount / BoostAmountMax, 0.15f);
    }

    void HUDinstruments()
    {
        currentSpeed = Mathf.Floor(playerShipRB.transform.InverseTransformDirection(playerShipRB.velocity).z);

        if (currentSpeed < 0)
            currentSpeed = 0;

        if (GameControlScript.ShipRunning)
        {
            SpeedText.text = "Speed\n" + currentSpeed;
            LThrustBar.DOFillAmount(leftThrustCurrent, 0.15f);
            RThrustBar.DOFillAmount(rightThrustCurrent, 0.15f);
        }
        else
            SpeedText.text = "";


        if (LeftHand.ObjectIsAttached(LeftThrottle))
            LStrafeBar.DOFillAmount(VRTriggerInputLeft, 0.15f);
        else if (RightHand.ObjectIsAttached(LeftThrottle))
            LStrafeBar.DOFillAmount(VRTriggerInputRight, 0.15f);
        else
            LStrafeBar.fillAmount = 0;

        if (RightHand.ObjectIsAttached(RightThrottle))
            RStrafeBar.DOFillAmount(VRTriggerInputRight, 0.15f);
        else if (LeftHand.ObjectIsAttached(RightThrottle))
            RStrafeBar.DOFillAmount(VRTriggerInputLeft, 0.15f);
        else
            RStrafeBar.fillAmount = 0;

        /*        
        if (LeftHand.ObjectIsAttached(LeftThrottle))
        {
            LStrafeText.text = "StrafeL\n" + Mathf.Floor(VRTriggerInputLeft * 100);
        }
        else
        {
            LStrafeText.text = "StrafeL\n" + 0;
        }

        if (RightHand.ObjectIsAttached(RightThrottle))
        {
            RStrafeText.text = "StrafeR\n" + Mathf.Floor(VRTriggerInputRight * 100);
        }
        else
        {
            RStrafeText.text = "StrafeR\n" + 0;
        }

        LThrustText.text = "ThrustL\n" + Mathf.Floor(leftThrustCurrent * 100);
        RThrustText.text = "ThrustR\n" + Mathf.Floor(rightThrustCurrent * 100);
        */
    }



    IEnumerator AddPadBoost(float pBoostTime, float pBoostStrength)
    {
        boostPadHit = true;
        AccelMultip *= pBoostStrength;
        playerShipRB.AddRelativeForce(Vector3.forward, ForceMode.Impulse);

        SpeedText.color = Color.red;

        MasterAudio.PlaySound3DFollowTransformAndForget("EH_Hit2", this.gameObject.transform);

        yield return new WaitForSeconds(pBoostTime);
        boostPadHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("BoostPickup"))
        {
            if (BoostAmount < BoostAmountMax)
            {
                MasterAudio.PlaySoundAndForget("MenuCancel");
                BoostAmount += other.gameObject.GetComponent<BoostPickups>().BoostToAdd;

                if (BoostAmount > BoostAmountMax)
                    BoostAmount = BoostAmountMax;

                Destroy(other.gameObject);
            }

        }
        */

        if (other.CompareTag("BoostPad"))
        {
            BoostPadScript = other.gameObject.GetComponent<BoostPad>();
            StartCoroutine(AddPadBoost(BoostPadScript.BoostTime, BoostPadScript.BoostStrength));
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
            MasterAudio.PlaySound3DFollowTransformAndForget("01-Collision_Demo", this.gameObject.transform);
    }
}
