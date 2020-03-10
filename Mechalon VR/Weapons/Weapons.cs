using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

namespace Mechalon
{
    public class Weapons : MonoBehaviour
    {
        public Text AmmoText;
        protected GameObject crosshair;

        protected Component enemyMechs;

        //Layermask for ignoring player parts
        public static LayerMask playerMechLayer;
        protected GameObject leftController;
        protected GameObject rightController;

        // Object to get shot effect components from
        protected GameObject projectile;

        // Who the gun is attached to
        public GameObject shooter;
        protected ParticleSystem muzzleFlash;

        protected Transform barrel;

        // "Projectile" for laser weapons
        protected LineRenderer laser;

        protected GameObject laserSight;

        protected LineRenderer laserSightLine;

        // Effect that plays when weapon hits enemy
        public GameObject enemyHitEffect;
        public float Damage;
        public float HeatPerShot;
        public float Range;

        // How much increased/decreased damage dealt to limbs versus torso
        public float LimbDmgMultiplier;

        // Time between shots
        public float CooldownTime;

        // Duration of shot (effects etc.)
        protected float ShotDuration;

        // Which hardpoint gun attaches to (Left low 1, left high 2, right low 3, right high 4)

        public int BallisticAmmo;

        // Compare this against time + weapon cooldown to set fire rate
        protected float readyToFire = 0;

        System.Random rand = new System.Random();

        protected RaycastHit hit;
        protected AudioSource weaponAudioSource;
        public TakeDamage takeDamage;
        protected Upgrades upg;

        protected Color laserReady;
        protected Color laserNotReady;


        protected virtual void Start()
        {
            upg = Upgrades.Instance;

            crosshair = GameObject.FindGameObjectWithTag("Crosshair");

            playerMechLayer = LayerMask.NameToLayer("PlayerMech");

            leftController = GameObject.FindGameObjectWithTag("LeftController");

            rightController = GameObject.FindGameObjectWithTag("RightController");

            weaponAudioSource = gameObject.GetComponent<AudioSource>();

            laserSight = transform.GetChild(1).gameObject;

            laserSightLine = laserSight.GetComponent<LineRenderer>();

            shooter = transform.root.gameObject;

            laserReady = new Color(0, 1, 0, 0.6f);
            laserNotReady = new Color(1, 0, 0, 0.6f);

            leftController.GetComponent<VRTK_ControllerEvents>().TriggerPressed += OnLeftTriggerPressed;

            leftController.GetComponent<VRTK_ControllerEvents>().TriggerHairlineStart += OnLeftTriggerHairline;

            leftController.GetComponent<VRTK_ControllerEvents>().TriggerReleased += OnLeftTriggerReleased;

            leftController.GetComponent<VRTK_ControllerEvents>().GripPressed += OnLeftGripPressed;

            leftController.GetComponent<VRTK_ControllerEvents>().GripReleased += OnLeftGripReleased;

            rightController.GetComponent<VRTK_ControllerEvents>().TriggerPressed += OnRightTriggerPressed;

            rightController.GetComponent<VRTK_ControllerEvents>().TriggerHairlineEnd += OnRightTriggerReleased;

            rightController.GetComponent<VRTK_ControllerEvents>().GripPressed += OnRightGripPressed;

            rightController.GetComponent<VRTK_ControllerEvents>().GripReleased += OnRightGripReleased;
        }

        protected virtual void OnLeftTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {

        }

        protected virtual void OnLeftTriggerHairline(object sender, ControllerInteractionEventArgs e)
        {

        }

        protected virtual void OnLeftTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {

        }

        protected virtual void OnLeftGripPressed(object sender, ControllerInteractionEventArgs e)
        {

        }

        protected virtual void OnLeftGripReleased(object sender, ControllerInteractionEventArgs e)
        {

        }

        protected virtual void OnRightTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {

        }

        protected virtual void OnRightTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {

        }

        protected virtual void OnRightGripPressed(object sender, ControllerInteractionEventArgs e)
        {

        }

        protected virtual void OnRightGripReleased(object sender, ControllerInteractionEventArgs e)
        {

        }

        protected void AddGunHeat()
        {
            PlayerMechControl.Instance.AddHeat(HeatPerShot);
        }

        protected int CheckHitLocation(RaycastHit pHit)
        {
            if (pHit.collider.gameObject.CompareTag("LeftArm"))
            {
                return 0;
            }
            else if (pHit.collider.gameObject.CompareTag("RightArm"))
            {
                return 1;
            }
            else if (pHit.collider.gameObject.CompareTag("LeftLeg"))
            {
                return 2;
            }
            else if (pHit.collider.gameObject.CompareTag("RightLeg"))
            {
                return 3;
            }
            else if (pHit.collider.gameObject.CompareTag("Torso"))
            {
                return 4;
            }
            else
            {
                return 5;
            }

        }


        // Add the returned vector3 from this to the guns raycast direction to add deviation
        protected Vector3 DeviateShots(int pPlusDeviation, int pMinusDeviation)
        {

            int x = rand.Next(pMinusDeviation, pPlusDeviation);
            int y = rand.Next(pMinusDeviation, pPlusDeviation);

            Vector3 deviation = new Vector3(x, y, 0);

            return deviation;

        }


    }


}