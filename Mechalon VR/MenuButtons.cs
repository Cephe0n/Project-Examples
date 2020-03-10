using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRTK;

namespace Mechalon
{
    public class MenuButtons : MonoBehaviour
    {
        Upgrades upg;


        public Text earningsText, killsText, matchesText, goldText, silverText, bronzeText, moneyText,
                                wpnTitleText, DPSText, dmgText, heatText, CDText, ammoText,
                                dmgTextNew, heatTextNew, CDTextNew, ammoTextNew, wpnCostText,
                                 TATitleText, TAText, AATitleText, AAText, LATitleText, LAText, TATextNew, AATextNew, LATextNew, TACostText, AACostText, LACostText,
                                 speedTitleText, speedText, heatTitleText, heatDiffText, speedTextNew, heatDiffTextNew, speedCost, heatDiffCost,
                                 statsACText, statsPLText, statsEHText, statsSGText, statsSpdText, statsHeatText, statsTAText, statsAAText, statsLAText,
                                 wpnBtnText;


        [SerializeField] GameObject Player, ammoUpgArrow, leftController, rightController, infoPanel;

        public GameObject WpnUpgBtn, AAUpgBtn, LAUpgBtn, TAUpgBtn, SpdUpgBtn, HeatUpgBtn, noviceBtn, interBtn, eliteBtn;

        bool ChosenAC, ChosenPL, ChosenEH, ChosenSG;

        public AudioSource menuAudio;

        public AudioClip Cancel, Confirm, Upgraded;

        private void Start()
        {
            upg = Player.GetComponent<Upgrades>();

            earningsText.text = upg.Earnings.ToString();
            killsText.text = upg.AllKills.ToString();
            matchesText.text = upg.Matches.ToString();
            goldText.text = upg.GoldMedals.ToString();
            silverText.text = upg.SilverMedals.ToString();
            bronzeText.text = upg.BronzeMedals.ToString();

            moneyText.text = PlayerPrefs.GetInt("Money").ToString();

            if (upg.UnlockedLeague == 1)
            {
                interBtn.SetActive(true);
                eliteBtn.SetActive(false);
            }
            else if (upg.UnlockedLeague == 2)
            {
                interBtn.SetActive(true);
                eliteBtn.SetActive(true);
            }
            else
            {
                interBtn.SetActive(false);
                eliteBtn.SetActive(false);
            }
        }


        public void PlayNovice()
        {
            menuAudio.PlayOneShot(Confirm);
            SceneManager.LoadScene("noviceMap");

            PlayerPrefs.SetInt("ChosenLeague", 0);
        }
        public void PlayIntermediate()
        {

            menuAudio.PlayOneShot(Confirm);
            SceneManager.LoadScene("interMap");

            PlayerPrefs.SetInt("ChosenLeague", 1);

        }
        public void PlayElite()
        {
            if (upg.UnlockedLeague == 2)
            {
                menuAudio.PlayOneShot(Confirm);
                SceneManager.LoadScene("eliteMap");

                PlayerPrefs.SetInt("ChosenLeague", 2);
            }
        }


        public void ChooseAC()
        {
            ChosenAC = true;
            ChosenPL = false;
            ChosenEH = false;
            ChosenSG = false;

            menuAudio.PlayOneShot(Confirm);

            WpnUpgBtn.SetActive(true);

            wpnBtnText.color = Color.white;
            wpnCostText.color = Color.white;
            WpnUpgBtn.GetComponent<Button>().interactable = true;

            wpnTitleText.text = "Autocannon Lvl " + upg.autoCannonLevel.ToString();

            // DPSText.text = "DPS: " + (((4.45 + 0.20 * upg.autoCannonLevel) * (0.28 - 0.02 * upg.autoCannonLevel)));

            dmgText.text = "Damage: " + (4.45 + 0.20 * upg.autoCannonLevel).ToString();
            dmgTextNew.text = "Damage: " + (4.45 + 0.20 * (upg.autoCannonLevel + 1)).ToString();

            heatText.text = "Heat: " + (1.1 - 0.05 * upg.autoCannonLevel).ToString();
            heatTextNew.text = "Heat: " + (1.1 - 0.05 * (upg.autoCannonLevel + 1)).ToString();

            CDText.text = "Cooldown: " + (0.28 - 0.02 * upg.autoCannonLevel).ToString();
            CDTextNew.text = "Cooldown: " + (0.28 - 0.02 * (upg.autoCannonLevel + 1)).ToString();

            ammoText.text = "Ammo: " + (50 + 3 * upg.autoCannonLevel).ToString();
            ammoTextNew.text = "Ammo: " + (50 + 3 * (upg.autoCannonLevel + 1)).ToString();
            ammoUpgArrow.SetActive(true);

            wpnCostText.text = "Cost: " + (100000 + 25000 * upg.autoCannonLevel).ToString();

            if (upg.autoCannonLevel == 10)
            {
                WpnUpgBtn.SetActive(false);
                wpnTitleText.text = "Autocannon Lvl Max";

            }
        }
        public void ChoosePL()
        {
            ChosenAC = false;
            ChosenPL = true;
            ChosenEH = false;
            ChosenSG = false;

            menuAudio.PlayOneShot(Confirm);

            WpnUpgBtn.SetActive(true);

            wpnBtnText.color = Color.white;
            wpnCostText.color = Color.white;
            WpnUpgBtn.GetComponent<Button>().interactable = true;

            wpnTitleText.text = "Pulse Laser Lvl " + upg.pulseLaserLevel.ToString();
            dmgText.text = "Damage: " + (4.50 + 0.30 * upg.pulseLaserLevel).ToString();
            dmgTextNew.text = "Damage: " + (4.50 + 0.30 * (upg.pulseLaserLevel + 1)).ToString();

            heatText.text = "Heat: " + (2.1 + 0.02 * upg.pulseLaserLevel).ToString();
            heatTextNew.text = "Heat: " + (2.1 + 0.05 * (upg.pulseLaserLevel + 1)).ToString();

            CDText.text = "Cooldown: " + (0.22 - 0.01 * upg.pulseLaserLevel).ToString();
            CDTextNew.text = "Cooldown: " + (0.22 - 0.01 * (upg.pulseLaserLevel + 1)).ToString();

            ammoText.text = "";
            ammoTextNew.text = "";
            ammoUpgArrow.SetActive(false);

            wpnCostText.text = "Cost: " + (100000 + 25000 * upg.pulseLaserLevel).ToString();

            if (upg.pulseLaserLevel == 10)
            {
                WpnUpgBtn.SetActive(false);
                wpnTitleText.text = "Pulse Laser Lvl Max";
            }
        }
        public void ChooseEH()
        {
            ChosenAC = false;
            ChosenPL = false;
            ChosenEH = true;
            ChosenSG = false;

            menuAudio.PlayOneShot(Confirm);

            WpnUpgBtn.SetActive(true);

            wpnBtnText.color = Color.white;
            wpnCostText.color = Color.white;
            WpnUpgBtn.GetComponent<Button>().interactable = true;

            wpnTitleText.text = "Electrohedron Lvl " + upg.electroHedronLevel.ToString();
            dmgText.text = "Damage: " + (44.50 + 2.5 * upg.electroHedronLevel).ToString();
            dmgTextNew.text = "Damage: " + (44.50 + 2.5 * (upg.electroHedronLevel + 1)).ToString();

            heatText.text = "Heat: " + (9.8 + 0.3 * upg.electroHedronLevel).ToString();
            heatTextNew.text = "Heat: " + (9.8 + 0.3 * (upg.electroHedronLevel + 1)).ToString();

            CDText.text = "Cooldown: " + (6 - 0.3 * upg.electroHedronLevel).ToString();
            CDTextNew.text = "Cooldown: " + (6 - 0.3 * (upg.electroHedronLevel + 1)).ToString();

            ammoText.text = "";
            ammoTextNew.text = "";
            ammoUpgArrow.SetActive(false);

            wpnCostText.text = "Cost: " + (100000 + 25000 * upg.electroHedronLevel).ToString();

            if (upg.electroHedronLevel == 10)
            {
                WpnUpgBtn.SetActive(false);
                wpnTitleText.text = "Electrohedron Lvl Max";
            }
        }
        public void ChooseSG()
        {
            ChosenAC = false;
            ChosenPL = false;
            ChosenEH = false;
            ChosenSG = true;

            menuAudio.PlayOneShot(Confirm);

            WpnUpgBtn.SetActive(true);

            wpnBtnText.color = Color.white;
            wpnCostText.color = Color.white;
            WpnUpgBtn.GetComponent<Button>().interactable = true;

            wpnTitleText.text = "Storm Gun Lvl " + upg.stormGunLevel.ToString();
            dmgText.text = "Damage: " + (8 + 0.3 * upg.stormGunLevel).ToString();
            dmgTextNew.text = "Damage: " + (8 + 0.3 * (upg.stormGunLevel + 1)).ToString();

            heatText.text = "Heat: " + (5 - 0.2 * upg.stormGunLevel).ToString();
            heatTextNew.text = "Heat: " + (5 - 0.2 * (upg.stormGunLevel + 1)).ToString();

            CDText.text = "Cooldown: " + (4 - 0.2 * upg.stormGunLevel).ToString();
            CDTextNew.text = "Cooldown: " + (4 - 0.2 * (upg.stormGunLevel + 1)).ToString();

            ammoText.text = "Ammo: " + (6 + 1 * upg.stormGunLevel).ToString();
            ammoTextNew.text = "Ammo: " + (6 + 1 * (upg.stormGunLevel + 1)).ToString();
            ammoUpgArrow.SetActive(true);

            wpnCostText.text = "Cost: " + (100000 + 25000 * upg.stormGunLevel).ToString();

            if (upg.stormGunLevel == 10)
            {
                WpnUpgBtn.SetActive(false);
                wpnTitleText.text = "Storm Gun Lvl Max";
            }
        }

        public void UpdateArmorStatsText()
        {
            AAUpgBtn.SetActive(true);
            TAUpgBtn.SetActive(true);
            LAUpgBtn.SetActive(true);

            TATitleText.text = "Torso lvl " + upg.torsoLevel.ToString();
            TAText.text = (75 + 10 * upg.torsoLevel).ToString();
            TATextNew.text = (75 + 10 * (upg.torsoLevel + 1)).ToString();

            TACostText.text = "Cost: " + (150000 + 25000 * upg.torsoLevel).ToString();
            AACostText.text = "Cost: " + (150000 + 25000 * upg.armsLevel).ToString();
            LACostText.text = "Cost: " + (150000 + 25000 * upg.legsLevel).ToString();

            AATitleText.text = "Arms lvl " + upg.armsLevel.ToString();
            AAText.text = (64 + 9 * upg.torsoLevel).ToString();
            AATextNew.text = (64 + 9 * (upg.torsoLevel + 1)).ToString();

            LATitleText.text = "Legs lvl " + upg.legsLevel.ToString();
            LAText.text = (68 + 9 * upg.torsoLevel).ToString();
            LATextNew.text = (68 + 9 * (upg.torsoLevel + 1)).ToString();

            if (upg.torsoLevel == 10)
            {
                TAUpgBtn.SetActive(false);
                TATitleText.text = "Torso lvl Max";
            }

            if (upg.armsLevel == 10)
            {
                AAUpgBtn.SetActive(false);
                AATitleText.text = "Arms lvl Max";
            }

            if (upg.legsLevel == 10)
            {
                LAUpgBtn.SetActive(false);
                LATitleText.text = "Legs lvl Max";
            }
        }

        public void UpdateGeneralStatsText()
        {
            SpdUpgBtn.SetActive(true);
            speedTitleText.text = "Top Speed Lvl " + upg.speedLevel.ToString();
            speedText.text = (20 + 2 * upg.speedLevel).ToString();
            speedTextNew.text = (20 + 2 * (upg.speedLevel + 1)).ToString();

            speedCost.text = "Cost: " + (100000 + 25000 * upg.speedLevel).ToString();

            HeatUpgBtn.SetActive(true);
            heatTitleText.text = "Heat Diffusion Lvl " + upg.heatLevel.ToString();
            heatDiffText.text = (5 + upg.heatLevel).ToString();
            heatDiffTextNew.text = (5 + (upg.heatLevel + 1)).ToString();

            heatDiffCost.text = "Cost: " + (100000 + 25000 * upg.heatLevel).ToString();

            if (upg.speedLevel == 10)
            {
                SpdUpgBtn.SetActive(false);
                speedTitleText.text = "Top Speed Lvl Max";

            }

            if (upg.heatLevel == 10)
            {
                SpdUpgBtn.SetActive(false);
                heatTitleText.text = "Heat Diffusion Lvl Max";
            }
        }

        public void UpdateStatScreen()
        {
            statsACText.text = "Autocannon - Lvl: " + upg.autoCannonLevel;
            statsPLText.text = "Pulse Laser - Lvl: " + upg.pulseLaserLevel;
            statsEHText.text = "Electrohedron - Lvl: " + upg.electroHedronLevel;
            statsSGText.text = "Storm Gun - Lvl: " + upg.stormGunLevel;

            statsTAText.text = "Torso Armor - Lvl: " + upg.torsoLevel;
            statsAAText.text = "Arm Armor - Lvl: " + upg.armsLevel;
            statsLAText.text = "Leg Armor - Lvl: " + upg.legsLevel;

            statsSpdText.text = "Top Speed - Lvl: " + upg.speedLevel;
            statsHeatText.text = "Heat Diffusion - Lvl: " + upg.heatLevel;
        }


        public void UpgradeWeapon()
        {
            if (ChosenAC)
                upg.UpgradeAC();
            else if (ChosenPL)
                upg.UpgradePL();
            else if (ChosenEH)
                upg.UpgradeEH();
            else if (ChosenSG)
                upg.UpgradeSG();
        }

        public void PlayCancelSound()
        {
            menuAudio.PlayOneShot(Cancel);
        }

        public void Pause()
        {

            if (infoPanel.activeInHierarchy == false)
            {
                DisableControls();
                Time.timeScale = 0;
                infoPanel.SetActive(true);
                rightController.GetComponent<VRTK_ControllerEvents>().GripPressed += OnRightGripPressed;
                leftController.GetComponent<VRTK_ControllerEvents>().GripPressed += OnLeftGripPressed;
            }
            else
            {
                rightController.GetComponent<VRTK_ControllerEvents>().GripPressed -= OnRightGripPressed;
                leftController.GetComponent<VRTK_ControllerEvents>().GripPressed -= OnLeftGripPressed;
                EnableControls();
                Time.timeScale = 1;
                infoPanel.SetActive(false);
            }
        }

        void OnLeftGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (SceneManager.GetActiveScene().name != "mainMenu")
                SceneManager.LoadScene("mainMenu");
        }

        void OnRightGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            Application.Quit();
        }

        private void DisableControls()
        {

            rightController.GetComponent<VRTK_TouchpadControl>().enabled = false;
            leftController.GetComponent<VRTK_TouchpadControl>().enabled = false;
            rightController.GetComponent<VRTK_ControllerEvents>().enabled = false;
        }

        private void EnableControls()
        {

            rightController.GetComponent<VRTK_TouchpadControl>().enabled = true;
            leftController.GetComponent<VRTK_TouchpadControl>().enabled = true;
            rightController.GetComponent<VRTK_ControllerEvents>().enabled = true;
        }
    }
}
