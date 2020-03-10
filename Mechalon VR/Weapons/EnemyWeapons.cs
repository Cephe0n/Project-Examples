using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mechalon
{
    public class EnemyWeapons : MonoBehaviour
    {

        protected GameObject projectile;
        protected ParticleSystem muzzleFlash;

        protected Transform barrel;

        // "Projectile" for laser weapons
        protected LineRenderer laser;

        // Effect that plays when weapon hits enemy
        protected GameObject hitEffect;

        // Who the gun is attached to
        protected GameObject shooter;
        protected float Damage;
        protected float HeatPerShot;
        protected float Range;

        // How much increased/decreased damage dealt to limbs versus torso
        protected float LimbDmgMultiplier;

        // Time between shots
        protected float CooldownTime;

        // Duration of shot (effects etc.)
        protected float ShotDuration;

        protected int BallisticAmmo;

        // Compare this against time + weapon cooldown to set fire rate
        protected float readyToFire = 0;

        float ready = 0;

        protected RaycastHit hit;
        protected Vector3 deviation;
        protected AudioSource weaponAudioSource;
        protected TakeDamage targetTakeDamage;
        protected TakeDamage selfTakeDamage;
        protected EnemyMechs enemyMechs;
        protected EnemyAiming aimingScript;
        private ThreatScript threatScript;
        System.Random rand = new System.Random();

        protected virtual void Start()
        {
            enemyMechs = gameObject.GetComponentInParent<EnemyMechs>();
            shooter = transform.root.gameObject;
            selfTakeDamage = gameObject.GetComponentInParent<TakeDamage>();
            threatScript = gameObject.GetComponentInParent<ThreatScript>();
            aimingScript = transform.root.Find("hips/Torso/AimPoint").GetComponent<EnemyAiming>();
            weaponAudioSource = gameObject.GetComponent<AudioSource>();

        }

        protected int CheckHitLocation(RaycastHit pHit)

        {
            if (pHit.collider.gameObject.CompareTag("LeftArm"))
            {
                threatScript.Threat += 1;
                return 0;
            }
            else if (pHit.collider.gameObject.CompareTag("RightArm"))
            {
                threatScript.Threat += 1;
                return 1;
            }
            else if (pHit.collider.gameObject.CompareTag("LeftLeg"))
            {
                threatScript.Threat += 2;
                return 2;
            }
            else if (pHit.collider.gameObject.CompareTag("RightLeg"))
            {
                threatScript.Threat += 2;
                return 3;
            }
            else if (pHit.collider.gameObject.CompareTag("Torso"))
            {
                threatScript.Threat += 2;
                return 4;
            }
            else
            {
                return 5;
            }

        }

        protected Vector3 DeviateShots()
        {

            int x = rand.Next(-5, enemyMechs.ShotDeviation);
            int y = rand.Next(-5, enemyMechs.ShotDeviation);
            int z = rand.Next(-5, enemyMechs.ShotDeviation);
            deviation = new Vector3(x, y, z);

            return deviation;

        }

        protected Vector2 DeviateShotgun(int pPlusDeviation, int pMinusDeviation)
        {

            int x = rand.Next(pMinusDeviation, pPlusDeviation);
            int y = rand.Next(pMinusDeviation, pPlusDeviation);

            Vector3 deviation = new Vector3(x, y, 0);

            return deviation;

        }

        protected bool TimeBetweenShots()
        {

            float nextFire = Random.Range(enemyMechs.minTimeToFire, enemyMechs.maxTimeToFire);

            if (Time.time > ready)
            {
                ready = Time.time + nextFire;
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}