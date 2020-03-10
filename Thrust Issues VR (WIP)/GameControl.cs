using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Valve.VR;
using Valve.VR.InteractionSystem;
using DarkTonic.MasterAudio;

public class GameControl : MonoBehaviour
{
    public bool ShipRunning, WODActive, Paused, StartupFinished, StartupResetPressed;
    public GameObject GameOverScreen, PauseScreen, HudElements;
    public Text PauseText;
    public GameObject WallOfDeath;
    public SteamVR_Action_Boolean PauseBtn, RestartBtn;
    public Hand LeftHand, RightHand;
    public GameObject PlayerShip, LeftThrottle, RightThrottle, Ignition, StartupPanel, IgnitionLever, WoDBtn, GearStick;
    public float WodActivateTime;
    public float IgnitionLeverValue = 0;
    public bool Dead;
    GearScript GearScript;


    private void Start()
    {
        GearScript = GearStick.GetComponent<GearScript>();

        Dead = false;
        UnPause();
    }

    public void Pause()
    {

        Ignition.GetComponent<BoxCollider>().enabled = false;
        GearStick.GetComponent<BoxCollider>().enabled = false;
        Time.timeScale = 0;
        PauseScreen.SetActive(true);
        Paused = true;
    }

    public void UnPause()
    {

        Ignition.GetComponent<BoxCollider>().enabled = true;

        GearStick.GetComponent<BoxCollider>().enabled = true;

        PauseScreen.SetActive(false);
        Time.timeScale = 1;
        Paused = false;
    }

    public void Death()
    {
        MasterAudio.PlaySoundAndForget("Death_screen");
        MasterAudio.StopAllPlaylists();
        MasterAudio.StopAllOfSound("EngineLoop");
        MasterAudio.StopAllOfSound("JumpJetLoopR");
        MasterAudio.StopAllOfSound("JumpJetLoopL");
        MasterAudio.StopAllOfSound("strafeloopL");
        MasterAudio.StopAllOfSound("strafeloopR");
        MasterAudio.StopAllOfSound("HL_FireLoop");
        HudElements.SetActive(false);
        Dead = true;
        ShipRunning = false;

        Time.timeScale = 0;
        GameOverScreen.SetActive(true);
    }


    public void ActivateShip()
    {
        HudElements.SetActive(true);

        ShipRunning = true;
        Ignition.GetComponent<BoxCollider>().enabled = false;

        GearScript.GearText.text = "Gear: " + GearScript.CurrentGear;

        if (GearScript.CurrentGear == 0)
        GearScript.GearText.text = "Gear: N";
    }

    public void DemoWoDButton()
    {
        if (ShipRunning && !Paused && !WODActive && !LeftHand.ObjectIsAttached(LeftThrottle))
        {
            MasterAudio.StartPlaylist("TestLevelPlayList");
            Invoke("ActivateWOD", WodActivateTime);
            WoDBtn.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
            WoDBtn.GetComponent<Interactable>().enabled = false;
        }
    }

    public void DeactivateShip()
    {
        ShipRunning = false;
        HudElements.SetActive(false);

        GearScript.GearText.text = "";
    }

    void ActivateWOD()
    {
        WallOfDeath.SetActive(true);
        WODActive = true;
    }

    void Update()
    {

        IgnitionLeverValue = IgnitionLever.GetComponent<LinearMapping>().value;

        if (!RightHand.ObjectIsAttached(RightThrottle))
        {
            Ignition.GetComponent<BoxCollider>().enabled = true;
        }

        if (ShipRunning && IgnitionLeverValue > 0)
        {
            Ignition.GetComponent<Interactable>().enabled = false;
        }
        else
        {
            Ignition.GetComponent<Interactable>().enabled = true;
        }


        if (PauseBtn.GetStateDown(LeftHand.handType))
        {
            if (!Paused && !Dead)
                Pause();
            else if (Paused && !Dead)
                UnPause();
        }

        if (RestartBtn.GetStateDown(RightHand.handType))
        {
            if (Paused || Dead)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator StartupSequence()
    {
        yield return new WaitForSecondsRealtime(3f);
        Pause();

        if (StartupResetPressed)
        {
            StartupPanel.SetActive(false);
        }

    }
}
