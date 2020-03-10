using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Mechalon
{
    public class PulseLaser : Weapons
    {

        protected override void Start()
        {
            base.Start();

            Damage = 4.50f + (0.30f * upg.pulseLaserLevel);
            HeatPerShot = 2.1f + (0.02f * upg.pulseLaserLevel);
            LimbDmgMultiplier = 1.3f;
            CooldownTime = 0.22f - (0.01f * upg.pulseLaserLevel);
            ShotDuration = 0.1f;

            projectile = Instantiate(Resources.Load("projectiles/TestLaser") as GameObject);

            enemyHitEffect = Instantiate(Resources.Load("onHitEffects/PulseLaserHit") as GameObject);

            laser = projectile.GetComponent<LineRenderer>();

            muzzleFlash = GameObject.FindGameObjectWithTag("PLVFX").GetComponent<ParticleSystem>();

            barrel = transform.GetChild(0).transform;

            laser.transform.position = new Vector3(0, 0, 0);

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

            laser.SetPosition(0, barrel.position);
        }

        protected override void OnRightTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (!PlayerMechControl.Instance.Dead && Time.timeScale != 0 && !GameControl.Instance.heatOverload && gameObject.activeInHierarchy)
                InvokeRepeating("ShootPulseLaser", 0, CooldownTime * Time.deltaTime);
        }

        protected override void OnRightTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            CancelInvoke();
        }


        public void ShootPulseLaser()
        {

            // Check if cooldown is done and crosshair is not inside cockpit
            if (Time.time > readyToFire && CrossHairScript.Instance.inFiringArea)
            {

                // Linecast from gun barrel onto crosshair's ray hit point
                if (Physics.Linecast(barrel.position, CrossHairScript.Instance.hitLocation, out hit))
                {

                    AddGunHeat();

                    Debug.DrawRay(barrel.position, CrossHairScript.Instance.hitLocation, Color.green, 5);

                    int hitLocation = CheckHitLocation(hit);

                    // Set linerender "laser" between gun and linecast hit point

                    laser.SetPosition(1, hit.point);

                    // Make laser visible for the shots duration
                    StartCoroutine(LaserEffects(ShotDuration, laser, hit));

                    // If it hits enemy mech part, get the enemymech script from it to call the damage function
                    if (hitLocation != 5 && hit.transform.gameObject.layer != 8)
                    {
                        takeDamage = hit.transform.GetComponentInParent<TakeDamage>();

                        takeDamage.EnemyDamage(Damage, LimbDmgMultiplier, hitLocation, enemyHitEffect, shooter, hit);
                    }

                    readyToFire = Time.time + CooldownTime;
                }

            }

        }


        IEnumerator LaserEffects(float pShotDuration, LineRenderer pLaserVisual, RaycastHit pHit)
        {
            muzzleFlash.Play(true);
            pLaserVisual.enabled = true;

            weaponAudioSource.Play();

            yield return new WaitForSeconds(pShotDuration);

            pLaserVisual.enabled = false;
        }

    }
}