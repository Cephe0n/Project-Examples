using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRTK;
using DG.Tweening;


namespace Mechalon
{
    public class PlayerMechControl : MonoBehaviour
    {
        public static PlayerMechControl Instance;
        private Vector3 flightVector;
        public float flySpeed;
        public float fuel;

        // Total money amount
        public int Money;

        // Money earned in match
        public int MoneyEarned;
        [SerializeField] Rigidbody mech;
        [SerializeField] GameObject rightController;
        //  [SerializeField] Text FuelText;
        //  [SerializeField] Text HeatText;
        [SerializeField] Text HeatWarningText;

        public Image fuelBar, heatBar, ammoBarAC, TORSO_Bar, RARM_Bar, LARM_Bar,
                    LLEG_Bar, RLEG_Bar;

        [SerializeField]
        AudioClip engineSound, jetSound, torsoSound, heatWarningSound,
                            heatCriticalSound, stepSound;

        public AudioClip HeatShutDown;
        public AudioClip HeatStartup;
        public AudioClip MatchStartup;
        public AudioClip unableToFire;
        private float heatLevel;
        public float heatSinkLevel;
        private bool Flying;
        public bool Dead;
        public AudioSource audioSource;
        bool soundPlayed = false;
        TakeDamage takeDamage;
        Upgrades upg;
        float playerTorsoHealth;
        float playerRarmHealth;
        float playerLarmHealth;
        float playerRlegHealth;
        float playerLlegHealth;
        float time;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {

            upg = Upgrades.Instance;
            MoneyEarned = 0;
            Dead = false;
            heatLevel = 0;
            heatSinkLevel = 5 + upg.heatLevel;
            audioSource = gameObject.GetComponent<AudioSource>();
            fuel = Mathf.Floor(150);
            flySpeed = 1700f;
            Time.timeScale = 1;
            takeDamage = gameObject.GetComponent<TakeDamage>();

            VRTK_SlideObjectControlAction[] slidespeeds = gameObject.GetComponents<VRTK_SlideObjectControlAction>();

            for (int i = 0; i < slidespeeds.Length; i++)
            {
                slidespeeds[i].maximumSpeed += (2 * upg.speedLevel);
            }

            takeDamage.LeftArmHealth = 64 + (9 * upg.armsLevel);
            takeDamage.RightArmHealth = 64 + (9 * upg.armsLevel);
            takeDamage.LeftLegHealth = 68 + (9 * upg.legsLevel);
            takeDamage.RightLegHealth = 68 + (9 * upg.legsLevel);
            takeDamage.TorsoHealth = 75 + (10 * upg.torsoLevel);

            if (PlayerPrefs.HasKey("Money"))
            {
                Money = PlayerPrefs.GetInt("Money");
            }
            else
            {
                Money = 0;
            }

            PlayerPrefs.SetInt("Money", Money);

            // UI stuff
            time = 0.05f;
            playerTorsoHealth = takeDamage.TorsoHealth;
            playerRarmHealth = takeDamage.RightArmHealth;
            playerLarmHealth = takeDamage.LeftArmHealth;
            playerRlegHealth = takeDamage.RightLegHealth;
            playerLlegHealth = takeDamage.LeftLegHealth;

        }

        private void Update()
        {
            CheckHeat();
            //FuelText.text = "Fuel: " + Mathf.Floor(fuel);

            float fillAmountFuel = fuel / 150;

            fuelBar.DOFillAmount(fillAmountFuel, time);


            // Health Bar calc:
            float playerTorsoHealthFA = (takeDamage.partHealths[4] / playerTorsoHealth);
            float playerRarmHealthFA = (takeDamage.partHealths[1] / playerRarmHealth);
            float playerLarmHealthFA = (takeDamage.partHealths[0] / playerLarmHealth);
            float playerRlegHealthFA = (takeDamage.partHealths[3] / playerRlegHealth);
            float playerLlegHealthFA = (takeDamage.partHealths[2] / playerLlegHealth);

            // Health indicator updates:
            TORSO_Bar.DOFillAmount(playerTorsoHealthFA, time);
            RARM_Bar.DOFillAmount(playerRarmHealthFA, time);
            LARM_Bar.DOFillAmount(playerLarmHealthFA, time);
            LLEG_Bar.DOFillAmount(playerLlegHealthFA, time);
            RLEG_Bar.DOFillAmount(playerRlegHealthFA, time);
        }


        void FixedUpdate()
        {
            Fly();
            LimitRotation();
        }


        private void LimitRotation()
        {
            // Stops the mech from tipping on inclines

            if (transform.rotation.x != 0 || transform.rotation.z != 0)
            {
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            }

        }

        public void TorsoSounds()
        {

        }


        public void Fly()
        {
            if (rightController.GetComponent<VRTK_ControllerEvents>().GetTouchpadAxis().y >= 0.8f)
            {
                Flying = true;
            }
            else
            {
                Flying = false;
            }

            // Fly up when touchpad/joystick pressed down, as long as fuel remaining
            if (Flying)
            {
                flightVector = new Vector3(0, 1, 0) * flySpeed * Time.deltaTime;

                if (fuel > 0)
                {
                    mech.AddForce(flightVector, ForceMode.Impulse);

                    fuel -= Time.deltaTime * 50;

                    if (fuel < 0)
                    {
                        fuel = 0;
                    }

                    PlaySound(jetSound, 0.2f);

                }
            }

            // When jumpjet button released, slowly regen fuel
            if (fuel < 150 && !Flying)
                fuel += Time.deltaTime * 15;
        }

        // Added to touchpad press controller event
        public void StartFlying()
        {
            Flying = true;
        }

        // Added to touchpad release controller event
        public void StopFlying()
        {
            Flying = false;
        }

        public void AddHeat(float pAddedHeat)
        {
            heatLevel += pAddedHeat;

        }

        private void CheckHeat()
        {
            float fillTime = 0.05f;
            float colorFillTime = 0.2f;

            heatLevel -= heatSinkLevel * Time.deltaTime;


            float fillAmountHeat = heatLevel / 56;

            Color normalColor = new Color(175, 175, 175, 175);

            heatBar.DOFillAmount(fillAmountHeat, fillTime);


            if (heatLevel < 0)
                heatLevel = 0;

            // HeatText.text = "Heat: " + Mathf.Floor(heatLevel);

            if (heatLevel >= 44 && heatLevel < 56)
            {
                Color criticalColor = new Color(195, 0, 0, 175);

                HeatWarningText.color = criticalColor;
                HeatWarningText.text = "//// HEAT CRITICAL ////";


                heatBar.DOColor(criticalColor, colorFillTime);

                if (!soundPlayed)
                    StartCoroutine(PlaySound(heatCriticalSound, 0.7f));

            }

            else if (heatLevel >= 31 && heatLevel < 56)
            {
                Color warningColor = new Color(175, 175, 0, 175);

                HeatWarningText.color = warningColor;
                HeatWarningText.text = "// HEAT WARNING //";


                heatBar.DOBlendableColor(warningColor, colorFillTime);

                if (!soundPlayed)
                    StartCoroutine(PlaySound(heatWarningSound, 0.7f));
            }

            else if (heatLevel < 31)
            {
                HeatWarningText.text = "";

                heatBar.DOColor(normalColor, colorFillTime);

            }

            if (heatLevel >= 56 && !GameControl.Instance.heatOverload)
            {
                StartCoroutine(GameControl.Instance.HeatOverload());
            }

        }

        IEnumerator PlaySound(AudioClip pClip, float pDuration)
        {
            audioSource.PlayOneShot(pClip);
            soundPlayed = true;
            yield return new WaitForSeconds(pDuration);
            soundPlayed = false;
        }
    }


}
