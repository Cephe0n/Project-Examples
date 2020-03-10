using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using DG.Tweening;

namespace Mechalon
{
    public class ElectroHedron : Weapons
    {
        public RaycastHit lineHit;

        protected override void Start()
        {
            base.Start();

            Damage = 44.50f + (2.5f * upg.electroHedronLevel);
            HeatPerShot = 9.8f + (0.3f * upg.electroHedronLevel);
            Range = 150f;
            LimbDmgMultiplier = 1.3f;
            CooldownTime = 6f - (0.3f * upg.electroHedronLevel);
            ShotDuration = 3f;

            barrel = transform.GetChild(0).transform;

            enemyHitEffect = Resources.Load("onHitEffects/EHHit") as GameObject;

            muzzleFlash = GameObject.FindGameObjectWithTag("EHVFX").GetComponent<ParticleSystem>();
        }

        private void LateUpdate()
        {
            if (CrossHairScript.Instance.inFiringArea)
            {
                laserSightLine.enabled = true;

                laserSightLine.SetPosition(0, laserSight.transform.position);

                laserSightLine.SetPosition(1, CrossHairScript.Instance.hitLocation);

                if (Time.time > readyToFire)
                {
                    laserSightLine.material.color = laserReady;
                    laserSightLine.material.color = laserReady;
                }
                else
                {
                    laserSightLine.material.color = laserNotReady;
                    laserSightLine.material.color = laserNotReady;
                }
            }

            else
            {
                laserSightLine.enabled = false;
            }

        }
        protected override void OnLeftGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (!PlayerMechControl.Instance.Dead && Time.timeScale != 0 && !GameControl.Instance.heatOverload && gameObject.activeInHierarchy)
                ShootHedron();
        }

        void ShootHedron()
        {
            if (Time.time > readyToFire && CrossHairScript.Instance.inFiringArea)
            {
                if (Vector3.Distance(barrel.transform.position, hit.point) > Range * 2)
                {
                    Damage *= 2f;
                }
                else if (Vector3.Distance(barrel.transform.position, hit.point) > Range * 1.5)
                {
                    Damage *= 1.5f;
                }
                else if (Vector3.Distance(barrel.transform.position, hit.point) > Range * 1.2)
                {
                    Damage *= 1.2f;
                }
                else if (Vector3.Distance(barrel.transform.position, hit.point) < Range)
                {
                    Damage *= 0.5f;
                }
                else
                {
                    Damage = 44.50f + (2.5f * upg.electroHedronLevel);
                }

                AddGunHeat();

                projectile = Resources.Load("projectiles/ElectroBullet") as GameObject;

                Physics.Linecast(barrel.position, CrossHairScript.Instance.hitLocation, out lineHit);

                Instantiate(projectile, barrel.position, transform.root.rotation);

                readyToFire = Time.time + CooldownTime;

                weaponAudioSource.Play();
            }

            else
            {
                weaponAudioSource.PlayOneShot(PlayerMechControl.Instance.unableToFire);
            }
        }

    }
}