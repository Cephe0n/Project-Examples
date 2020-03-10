using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRTK;

namespace Mechalon
{
    public class GameControl : MonoBehaviour
    {
        public static GameControl Instance;
        private AudioSource audioSource;
        private GameObject[] enemiesOnField;
        public List<GameObject> rankingList = new List<GameObject> { };
        private int EnemiesLeft;
        [SerializeField] GameObject Player;
        [SerializeField] GameObject VictoryScreen;
        Text victoryText;
        Text killsText;
        Text deathText;
        [SerializeField] GameObject leftController;
        [SerializeField] GameObject rightController;
        [SerializeField] GameObject crosshair;
        [SerializeField] GameObject crosshairImg;
        [SerializeField] GameObject infoPanel;
        [SerializeField] GameObject gameOverPanel;
        [SerializeField] GameObject heatOverloadPanel;
        [SerializeField] GameObject startupPanel;
        [SerializeField] GameObject HUD;
       // [SerializeField] GameObject window;
       // [SerializeField] Material windowTP;
      //  [SerializeField] Material windowOP;
        [SerializeField] AudioClip victorySound;
        [SerializeField] AudioClip successSound;
        [SerializeField] AudioClip killSound;
        [SerializeField] AudioClip matchStartSound;

      //  Material[] matsOP = new Material[2];
      //  Material[] matsTP = new Material[2];
        [SerializeField] Texture bronzeScreen;
        [SerializeField] Texture silverScreen;
        public bool victoryOrDeath;
        public bool heatOverload;

        // 1 = novice, 2 = intermediate, 3 = elite
        public int ChosenLeague;

        Upgrades upg;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {

            ChosenLeague = PlayerPrefs.GetInt("ChosenLeague");
            audioSource = gameObject.GetComponent<AudioSource>();
            upg = Player.GetComponent<Upgrades>();

          //  matsOP[0] = windowOP;
          //  matsOP[1] = windowOP;
          //  matsTP[0] = windowTP;
          //  matsTP[1] = windowTP;

            victoryOrDeath = false;
            heatOverload = false;
            enemiesOnField = GameObject.FindGameObjectsWithTag("Enemy");
            EnemiesLeft = enemiesOnField.Length;
            victoryText = VictoryScreen.transform.GetChild(0).GetComponent<Text>();
            killsText = VictoryScreen.transform.GetChild(1).GetComponent<Text>();
            deathText = gameOverPanel.transform.GetChild(0).GetComponent<Text>();

            StartCoroutine(MatchStartup());

        }

        void Victory()
        {
            victoryOrDeath = true;
            Time.timeScale = 0f;
            audioSource.PlayOneShot(victorySound);
            heatOverloadPanel.SetActive(false);
           // window.GetComponent<Renderer>().materials = matsOP;
            VictoryScreen.SetActive(true);
            HUD.SetActive(false);
            DisableControls();
            rankingList.Add(Player);
            rankingList.Reverse();

            // Give rewards based on league, if enough kills, unlock next league
            switch (ChosenLeague)
            {
                case 0:
                    upg.playerScript.MoneyEarned += 303000;
                    if (Player.GetComponent<ThreatScript>().Kills > 2)
                    {
                        if (PlayerPrefs.GetInt("UnlockedLeague") < 2)
                            PlayerPrefs.SetInt("UnlockedLeague", 1);
                    }
                    break;
                case 1:
                    upg.playerScript.MoneyEarned += 1010000;
                    if (Player.GetComponent<ThreatScript>().Kills > 2)
                    {
                        PlayerPrefs.SetInt("UnlockedLeague", 2);
                    }
                    break;
                case 2:
                    upg.playerScript.MoneyEarned += 10100000;
                    break;
            }

            upg.playerScript.Money += upg.playerScript.MoneyEarned;
            PlayerPrefs.SetInt("Money", upg.playerScript.Money);

            upg.Earnings += upg.playerScript.MoneyEarned;
            upg.GoldMedals++;
            PlayerPrefs.SetInt("Earnings", upg.Earnings);
            PlayerPrefs.SetInt("GoldMedals", upg.GoldMedals);

            for (int i = 0; i < rankingList.Count; i++)
            {
                victoryText.text += string.Format((i + 1).ToString()) + ".  " + rankingList[i].GetComponent<ThreatScript>().Name + "\n";
                killsText.text += rankingList[i].GetComponent<ThreatScript>().Kills + "\n";
            }

            victoryText.text += "\n Enemies destroyed: " + Player.GetComponent<ThreatScript>().Kills
                                + "\n Money earned: $" + upg.playerScript.MoneyEarned;

            // Change trigger presses to return to garage

            leftController.GetComponent<VRTK_ControllerEvents>().TriggerPressed += OnLeftTriggerPressed;
            rightController.GetComponent<VRTK_ControllerEvents>().TriggerPressed += OnRightTriggerPressed;
        }

        public void EnemyKilled()
        {
            EnemiesLeft -= 1;

            audioSource.PlayOneShot(killSound);

            upg.AllKills++;
            PlayerPrefs.SetInt("AllKills", upg.AllKills);

            if (EnemiesLeft <= 1)
            {
                Invoke("Victory", 3f);
            }
        }

        public void Death(GameObject pKiller)
        {

            victoryOrDeath = true;

            upg.playerScript.Dead = true;

            Time.timeScale = 0f;

            HUD.SetActive(false);

          //  window.GetComponent<Renderer>().materials = matsOP;

            rankingList.Add(Player);

            if (rankingList.Count < 8)
            {
                for (int i = 8; i > rankingList.Count;)
                rankingList.Add(leftController);
            }

            rankingList.Reverse();

            string num;
            switch (rankingList.IndexOf(Player))
            {
                case 0:
                    num = "st";
                    break;

                case 1:
                    num = "nd";
                    gameOverPanel.GetComponent<RawImage>().texture = silverScreen;
                    audioSource.PlayOneShot(successSound);
                    if (ChosenLeague == 0)
                    {
                        upg.playerScript.MoneyEarned += 202000;
                    }
                    else if (ChosenLeague == 1)
                    {
                        upg.playerScript.MoneyEarned += 505000;
                    }
                    else if (ChosenLeague == 2)
                    {
                        upg.playerScript.MoneyEarned += 3330000;
                    }

                    upg.SilverMedals++;
                    PlayerPrefs.SetInt("SilverMedals", upg.SilverMedals);
                    break;

                case 2:
                    num = "rd";
                    gameOverPanel.GetComponent<RawImage>().texture = bronzeScreen;
                    audioSource.PlayOneShot(successSound);

                    if (ChosenLeague == 0)
                    {
                        upg.playerScript.MoneyEarned += 101000;
                    }
                    else if (ChosenLeague == 1)
                    {
                        upg.playerScript.MoneyEarned += 303000;
                    }
                    else if (ChosenLeague == 2)
                    {
                        upg.playerScript.MoneyEarned += 1010000;
                    }
                    upg.BronzeMedals++;
                    PlayerPrefs.SetInt("BronzeMedals", upg.BronzeMedals);
                    break;

                default:
                    num = "th";
                    break;
            }

            upg.playerScript.Money += upg.playerScript.MoneyEarned;
            upg.Earnings += upg.playerScript.MoneyEarned;
            PlayerPrefs.SetInt("Money", Player.GetComponent<PlayerMechControl>().Money);
            PlayerPrefs.SetInt("Earnings", upg.Earnings);

            deathText.text = "You were destroyed by:\n" + pKiller.GetComponent<ThreatScript>().Name + "\n\nYou ranked " + (rankingList.IndexOf(Player) + 1) + num
                                + "\nEnemies destroyed: " + Player.GetComponent<ThreatScript>().Kills
                                + "\nMoney earned: $" + upg.playerScript.MoneyEarned;


            heatOverloadPanel.SetActive(false);
            gameOverPanel.SetActive(true);
            DisableControls();

            // Change trigger presses to reload scene
            leftController.GetComponent<VRTK_ControllerEvents>().TriggerPressed += OnLeftTriggerPressed;
            rightController.GetComponent<VRTK_ControllerEvents>().TriggerPressed += OnRightTriggerPressed;
        }

        public IEnumerator HeatOverload()
        {
            heatOverload = true;
            DisableControls();
            HUD.SetActive(false);
          //  window.GetComponent<Renderer>().materials = matsOP;
            upg.playerScript.audioSource.PlayOneShot(upg.playerScript.HeatShutDown);
            heatOverloadPanel.SetActive(true);

            yield return new WaitForSeconds(5f);

            heatOverload = false;
            EnableControls();
          //  window.GetComponent<Renderer>().materials = matsTP;
            HUD.SetActive(true);
            upg.playerScript.audioSource.PlayOneShot(upg.playerScript.HeatStartup);
            heatOverloadPanel.SetActive(false);

        }

        IEnumerator MatchStartup()
        {
            victoryOrDeath = true;
            Time.timeScale = 0f;
            DisableControls();
            //window.GetComponent<Renderer>().materials = matsOP;
            startupPanel.SetActive(true);
            HUD.SetActive(false);
            upg.playerScript.audioSource.PlayOneShot(upg.playerScript.MatchStartup);

            yield return new WaitForSecondsRealtime(5f);

            victoryOrDeath = false;
            Time.timeScale = 1;
            EnableControls();
           // leftController.GetComponent<VRTK_ControllerEvents>().StartMenuPressed += OnMenuPressed;
           // window.GetComponent<Renderer>().materials = matsTP;
            startupPanel.SetActive(false);
            HUD.SetActive(true);
            audioSource.PlayOneShot(matchStartSound);

            upg.Matches++;
            PlayerPrefs.SetInt("Matches", upg.Matches);
        }

        public void DisableControls()
        {
            crosshair.SetActive(false);

            rightController.GetComponent<VRTK_TouchpadControl>().enabled = false;
            leftController.GetComponent<VRTK_TouchpadControl>().enabled = false;
            rightController.GetComponent<VRTK_ControllerEvents>().enabled = false;
        }

        public void EnableControls()
        {
            crosshair.SetActive(true);

            rightController.GetComponent<VRTK_TouchpadControl>().enabled = true;
            leftController.GetComponent<VRTK_TouchpadControl>().enabled = true;
            rightController.GetComponent<VRTK_ControllerEvents>().enabled = true;
        }

        void OnLeftTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            SceneManager.LoadScene("garageTest");
        }
        void OnRightTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            SceneManager.LoadScene("garageTest");
        }
        void OnLeftGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            SceneManager.LoadScene("garageTest");
        }


        public void OnMenuPressed()
        {
            if (!victoryOrDeath)
            {
                if (infoPanel.activeInHierarchy == false)
                {
                    DisableControls();
                    Time.timeScale = 0;
                    infoPanel.SetActive(true);
                    HUD.SetActive(false);
                    leftController.GetComponent<VRTK_ControllerEvents>().GripPressed += OnLeftGripPressed;
                }
                else
                {
                    leftController.GetComponent<VRTK_ControllerEvents>().GripPressed -= OnLeftGripPressed;
                    if (!heatOverload)
                        EnableControls();

                    Time.timeScale = 1;
                    infoPanel.SetActive(false);
                    HUD.SetActive(true);
                }
            }
        }
    }
}