using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRTK;

namespace Mechalon
{
    public class Upgrades : MonoBehaviour
    {
        public static Upgrades Instance;

        public int autoCannonLevel, pulseLaserLevel, electroHedronLevel, stormGunLevel,
            torsoLevel, armsLevel, legsLevel, speedLevel, heatLevel;

        int[] levels;

        string[] labels = {"autoCannonLevel", "pulseLaserLevel", "electroHedronLevel", "stormGunLevel",
            "torsoLevel", "armsLevel", "legsLevel", "speedLevel", "heatLevel"};

        [SerializeField] GameObject Player;
        [SerializeField] MenuButtons menu;
        public PlayerMechControl playerScript;

        public int GoldMedals, SilverMedals, BronzeMedals, AllKills, Earnings, Matches, UnlockedLeague;

        int[] stats;
        string[] statLabels = { "GoldMedals", "SilverMedals", "BronzeMedals", "AllKills", "Earnings", "Matches" };
        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            playerScript = Player.GetComponent<PlayerMechControl>();

            stats = new int[] { GoldMedals, SilverMedals, BronzeMedals, AllKills, Earnings, Matches };

            for (int i = 0; i < statLabels.Length; i++)
            {
                if (!PlayerPrefs.HasKey(statLabels[i]))
                    PlayerPrefs.SetInt(statLabels[i], 0);
            }

            for (int i = 0; i < labels.Length; i++)
            {
                if (!PlayerPrefs.HasKey(labels[i]))
                    PlayerPrefs.SetInt(labels[i], 0);
            }

            autoCannonLevel = PlayerPrefs.GetInt("autoCannonLevel");
            pulseLaserLevel = PlayerPrefs.GetInt("pulseLaserLevel");
            electroHedronLevel = PlayerPrefs.GetInt("electroHedronLevel");
            stormGunLevel = PlayerPrefs.GetInt("stormGunLevel");
            torsoLevel = PlayerPrefs.GetInt("torsoLevel");
            armsLevel = PlayerPrefs.GetInt("armsLevel");
            legsLevel = PlayerPrefs.GetInt("legsLevel");
            speedLevel = PlayerPrefs.GetInt("speedLevel");
            heatLevel = PlayerPrefs.GetInt("heatLevel");

            GoldMedals = PlayerPrefs.GetInt("GoldMedals");
            SilverMedals = PlayerPrefs.GetInt("SilverMedals");
            BronzeMedals = PlayerPrefs.GetInt("BronzeMedals");
            AllKills = PlayerPrefs.GetInt("AllKills");
            Earnings = PlayerPrefs.GetInt("Earnings");
            Matches = PlayerPrefs.GetInt("Matches");
            UnlockedLeague = PlayerPrefs.GetInt("UnlockedLeague");

        }

        public void UpgradeAC()
        {
            if (playerScript.Money >= 100000 + (25000 * autoCannonLevel) && autoCannonLevel < 10)
            {
                menu.menuAudio.PlayOneShot(menu.Upgraded);
                playerScript.Money -= 100000 + (25000 * autoCannonLevel);

                autoCannonLevel++;

                menu.wpnTitleText.text = "Autocannon Lvl " + autoCannonLevel.ToString();
                menu.dmgText.text = "Damage: " + (4.45 + 0.20 * autoCannonLevel).ToString();
                menu.dmgTextNew.text = "Damage: " + (4.45 + 0.20 * (autoCannonLevel + 1)).ToString();

                menu.heatText.text = "Heat: " + (1.1 - 0.05 * autoCannonLevel).ToString();
                menu.heatTextNew.text = "Heat: " + (1.1 - 0.05 * (autoCannonLevel + 1)).ToString();

                menu.CDText.text = "Cooldown: " + (0.28 - 0.02 * autoCannonLevel).ToString();
                menu.CDTextNew.text = "Cooldown: " + (0.28 - 0.02 * (autoCannonLevel + 1)).ToString();

                menu.ammoText.text = "Ammo: " + (50 + 3 * autoCannonLevel).ToString();
                menu.ammoTextNew.text = "Ammo: " + (50 + 3 * (autoCannonLevel + 1)).ToString();

                menu.wpnCostText.text = "Cost: " + (100000 + 25000 * autoCannonLevel).ToString();

                SaveChanges();
            }
            else
            {
                menu.wpnBtnText.color = Color.red;
                menu.wpnCostText.color = Color.red;
                menu.WpnUpgBtn.GetComponent<Button>().interactable = false;

                if (autoCannonLevel == 10)
                {
                    menu.WpnUpgBtn.SetActive(false);
                    menu.wpnTitleText.text = "Autocannon Lvl Max";
                    menu.wpnCostText.text = "";
                }
            }
        }
        public void UpgradeEH()
        {
            if (playerScript.Money >= 100000 + (25000 * electroHedronLevel) && electroHedronLevel < 10)
            {
                menu.menuAudio.PlayOneShot(menu.Upgraded);
                playerScript.Money -= 100000 + (25000 * electroHedronLevel);

                electroHedronLevel++;

                menu.wpnTitleText.text = "Electrohedron Lvl " + electroHedronLevel.ToString();
                menu.dmgText.text = "Damage: " + (44.50 + 2.5 * electroHedronLevel).ToString();
                menu.dmgTextNew.text = "Damage: " + (44.50 + 2.5 * (electroHedronLevel + 1)).ToString();

                menu.heatText.text = "Heat: " + (9.8 + 0.3 * electroHedronLevel).ToString();
                menu.heatTextNew.text = "Heat: " + (9.8 + 0.3 * (electroHedronLevel + 1)).ToString();

                menu.CDText.text = "Cooldown: " + (6 - 0.3 * electroHedronLevel).ToString();
                menu.CDTextNew.text = "Cooldown: " + (6 - 0.3 * (electroHedronLevel + 1)).ToString();

                menu.ammoText.text = "";
                menu.ammoTextNew.text = "";

                menu.wpnCostText.text = "Cost: " + (100000 + 25000 * electroHedronLevel).ToString();
                SaveChanges();
            }
            else
            {
                menu.wpnBtnText.color = Color.red;
                menu.wpnCostText.color = Color.red;
                menu.WpnUpgBtn.GetComponent<Button>().interactable = false;

                if (electroHedronLevel == 10)
                {
                    menu.WpnUpgBtn.SetActive(false);
                    menu.wpnTitleText.text = "Electrohedron Lvl Max";
                    menu.wpnCostText.text = "";
                }
            }
        }
        public void UpgradePL()
        {
            if (playerScript.Money >= 100000 + (25000 * pulseLaserLevel) && pulseLaserLevel < 10)
            {

                menu.menuAudio.PlayOneShot(menu.Upgraded);
                playerScript.Money -= 100000 + (25000 * pulseLaserLevel);

                pulseLaserLevel++;

                menu.wpnTitleText.text = "Pulse Laser Lvl " + pulseLaserLevel.ToString();
                menu.dmgText.text = "Damage: " + (4.50 + 0.30 * pulseLaserLevel).ToString();
                menu.dmgTextNew.text = "Damage: " + (4.50 + 0.30 * (pulseLaserLevel + 1)).ToString();

                menu.heatText.text = "Heat: " + (2.1 + 0.02 * pulseLaserLevel).ToString();
                menu.heatTextNew.text = "Heat: " + (2.1 + 0.05 * (pulseLaserLevel + 1)).ToString();

                menu.CDText.text = "Cooldown: " + (0.22 - 0.01 * pulseLaserLevel).ToString();
                menu.CDTextNew.text = "Cooldown: " + (0.22 - 0.01 * (pulseLaserLevel + 1)).ToString();

                menu.ammoText.text = "";
                menu.ammoTextNew.text = "";

                menu.wpnCostText.text = "Cost: " + (100000 + 25000 * pulseLaserLevel).ToString();
                SaveChanges();
            }
            else
            {
                menu.wpnBtnText.color = Color.red;
                menu.wpnCostText.color = Color.red;
                menu.WpnUpgBtn.GetComponent<Button>().interactable = false;

                if (pulseLaserLevel == 10)
                {
                    menu.WpnUpgBtn.SetActive(false);
                    menu.wpnTitleText.text = "Pulse Laser Lvl Max";
                    menu.wpnCostText.text = "";
                }
            }
        }
        public void UpgradeSG()
        {
            if (playerScript.Money >= 100000 + (25000 * stormGunLevel) && stormGunLevel < 10)
            {
                menu.menuAudio.PlayOneShot(menu.Upgraded);
                playerScript.Money -= 100000 + (25000 * stormGunLevel);

                stormGunLevel++;

                menu.wpnTitleText.text = "Storm Gun Lvl " + stormGunLevel.ToString();
                menu.dmgText.text = "Damage: " + (8 + 0.3 * stormGunLevel).ToString();
                menu.dmgTextNew.text = "Damage: " + (8 + 0.3 * (stormGunLevel + 1)).ToString();

                menu.heatText.text = "Heat: " + (5 - 0.2 * stormGunLevel).ToString();
                menu.heatTextNew.text = "Heat: " + (5 - 0.2 * (stormGunLevel + 1)).ToString();

                menu.CDText.text = "Cooldown: " + (4 - 0.2 * stormGunLevel).ToString();
                menu.CDTextNew.text = "Cooldown: " + (4 - 0.2 * (stormGunLevel + 1)).ToString();

                menu.ammoText.text = "Ammo: " + (6 + 1 * stormGunLevel).ToString();
                menu.ammoTextNew.text = "Ammo: " + (6 + 1 * (stormGunLevel + 1)).ToString();

                menu.wpnCostText.text = "Cost: " + (100000 + 25000 * stormGunLevel).ToString();

                SaveChanges();
            }
            else
            {
                menu.wpnBtnText.color = Color.red;
                menu.wpnCostText.color = Color.red;
                menu.WpnUpgBtn.GetComponent<Button>().interactable = false;

                if (stormGunLevel == 10)
                {
                    menu.WpnUpgBtn.SetActive(false);
                    menu.wpnTitleText.text = "Storm Gun Lvl Max";
                    menu.wpnCostText.text = "";
                }
            }
        }
        public void UpgradeArms()
        {
            if (playerScript.Money >= 150000 + (25000 * armsLevel) && armsLevel < 10)
            {
                menu.menuAudio.PlayOneShot(menu.Upgraded);
                playerScript.Money -= 150000 + (25000 * armsLevel);

                armsLevel++;

                menu.AATitleText.text = "Arms lvl " + armsLevel.ToString();
                menu.AAText.text = (75 + 10 * torsoLevel).ToString();
                menu.AATextNew.text = (75 + 10 * (torsoLevel + 1)).ToString();
                menu.AACostText.text = "Cost: " + (150000 + 25000 * armsLevel).ToString();


                SaveChanges();
            }
            else
            {
                menu.AAUpgBtn.transform.GetChild(0).GetComponent<Text>().color = Color.red;
                menu.AACostText.color = Color.red;
                menu.AAUpgBtn.GetComponent<Button>().interactable = false;

                if (armsLevel == 10)
                {
                    menu.AAUpgBtn.SetActive(false);
                    menu.AATitleText.text = "Arms Lvl Max";
                    menu.AACostText.text = "";
                }
            }

        }
        
        public void UpgradeLegs()
        {
            if (playerScript.Money >= 150000 + (25000 * legsLevel) && legsLevel < 10)
            {
                menu.menuAudio.PlayOneShot(menu.Upgraded);
                playerScript.Money -= 150000 + (25000 * legsLevel);

                legsLevel++;

                menu.LATitleText.text = "Legs lvl " + legsLevel.ToString();
                menu.LAText.text = (75 + 10 * torsoLevel).ToString();
                menu.LATextNew.text = (75 + 10 * (torsoLevel + 1)).ToString();
                menu.LACostText.text = "Cost: " + (150000 + 25000 * legsLevel).ToString();

                SaveChanges();
            }
            else
            {
                menu.LAUpgBtn.transform.GetChild(0).GetComponent<Text>().color = Color.red;
                menu.LACostText.color = Color.red;
                menu.LAUpgBtn.GetComponent<Button>().interactable = false;

                if (legsLevel == 10)
                {
                    menu.LAUpgBtn.SetActive(false);
                    menu.LATitleText.text = "Legs Lvl Max";
                    menu.LACostText.text = "";
                }
            }
        }
        public void UpgradeTorso()
        {
            if (playerScript.Money >= 150000 + (25000 * torsoLevel) && torsoLevel < 10)
            {
                menu.menuAudio.PlayOneShot(menu.Upgraded);
                playerScript.Money -= 150000 + (25000 * torsoLevel);

                torsoLevel++;

                menu.TATitleText.text = "Torso lvl " + torsoLevel.ToString();
                menu.TAText.text = (75 + 10 * torsoLevel).ToString();
                menu.TATextNew.text = (75 + 10 * (torsoLevel + 1)).ToString();
                menu.TACostText.text = "Cost: " + (150000 + 25000 * torsoLevel).ToString();

                SaveChanges();
            }
            else
            {
                menu.TAUpgBtn.transform.GetChild(0).GetComponent<Text>().color = Color.red;
                menu.TACostText.color = Color.red;
                menu.TAUpgBtn.GetComponent<Button>().interactable = false;

                if (torsoLevel == 10)
                {
                    menu.TAUpgBtn.SetActive(false);
                    menu.TATitleText.text = "Torso Lvl Max";
                    menu.TACostText.text = "";
                }
            }
        }
        public void UpgradeSpeed()
        {
            if (playerScript.Money >= 100000 + (25000 * speedLevel) && speedLevel < 10)
            {
                menu.menuAudio.PlayOneShot(menu.Upgraded);
                playerScript.Money -= 100000 + (25000 * speedLevel);

                speedLevel++;

                menu.speedTitleText.text = "Top Speed Lvl " + speedLevel.ToString();
                menu.speedText.text = (5 + speedLevel).ToString();
                menu.speedTextNew.text = (5 + (speedLevel + 1)).ToString();
                menu.speedCost.text = "Cost: " + (150000 + 25000 * speedLevel).ToString();

                SaveChanges();
            }
            else
            {
                menu.SpdUpgBtn.transform.GetChild(0).GetComponent<Text>().color = Color.red;
                menu.speedCost.color = Color.red;
                menu.SpdUpgBtn.GetComponent<Button>().interactable = false;

                if (speedLevel == 10)
                {
                    menu.SpdUpgBtn.SetActive(false);
                    menu.speedTitleText.text = "Top Speed Lvl Max";
                    menu.speedCost.text = "";
                }
            }

        }
        public void UpgradeHeat()
        {
            if (playerScript.Money >= 100000 + (25000 * heatLevel) && heatLevel < 10)
            {
                menu.menuAudio.PlayOneShot(menu.Upgraded);
                playerScript.Money -= 100000 + (25000 * heatLevel);

                heatLevel++;

                menu.heatTitleText.text = "Heat Diffusion Lvl " + heatLevel.ToString();
                menu.heatDiffText.text = (5 + heatLevel).ToString();
                menu.heatDiffTextNew.text = (5 + (heatLevel + 1)).ToString();
                menu.heatDiffCost.text = "Cost: " + (100000 + 25000 * heatLevel).ToString();


                SaveChanges();
            }
            else
            {
                menu.HeatUpgBtn.transform.GetChild(0).GetComponent<Text>().color = Color.red;
                menu.heatDiffCost.color = Color.red;
                menu.HeatUpgBtn.GetComponent<Button>().interactable = false;

                if (heatLevel == 10)
                {
                    menu.HeatUpgBtn.SetActive(false);
                    menu.heatDiffText.text = "Heat Diffusion Lvl Max";
                    menu.heatDiffCost.text = "";
                }
            }
        }

        public void SaveChanges()
        {
            levels = new int[]{autoCannonLevel, pulseLaserLevel, electroHedronLevel, stormGunLevel,
            torsoLevel, armsLevel, legsLevel, speedLevel, heatLevel};


            for (int i = 0; i < labels.Length; i++)
            {
                PlayerPrefs.SetInt(labels[i], levels[i]);
            }

            menu.moneyText.text = playerScript.Money.ToString();

            PlayerPrefs.SetInt("Money", playerScript.Money);

        }

        public void ResetProgress()
        {
            levels = new int[]{autoCannonLevel, pulseLaserLevel, electroHedronLevel, stormGunLevel,
            torsoLevel, armsLevel, legsLevel, speedLevel, heatLevel};

            for (int i = 0; i < labels.Length; i++)
            {
                levels[i] = 0;

                PlayerPrefs.SetInt(labels[i], levels[i]);
            }

            for (int i = 0; i < statLabels.Length; i++)
            {
                stats[i] = 0;

                PlayerPrefs.SetInt(statLabels[i], stats[i]);
            }

            playerScript.Money = 0;

            PlayerPrefs.SetInt("UnlockedLeague", 0);

            PlayerPrefs.SetInt("Money", 0);
        }

        public void StartGame()
        {
            SceneManager.LoadScene("garageTest");
        }


    }
}