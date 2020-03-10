using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using DG.Tweening;

namespace Mechalon
{

    public class Shotgun : Weapons
    {
        public Image ammoBar;
        public int maxAmmo;

        LineRenderer SGLine;

        protected override void Start()
        {

            base.Start();

            Damage = 8 + (0.3f * upg.stormGunLevel);
            HeatPerShot = 5f - (0.2f * upg.stormGunLevel);
            Range = 100f;
            LimbDmgMultiplier = 1f;
            CooldownTime = 4f - (0.2f * upg.stormGunLevel);
            ShotDuration = 0.2f;
            BallisticAmmo = 6 + (1 * upg.stormGunLevel);
            maxAmmo = BallisticAmmo;

            barrel = transform.GetChild(0).transform;

            //projectile = Instantiate(Resources.Load("projectiles/AutoCannonShot") as GameObject);

            enemyHitEffect = Instantiate(Resources.Load("onHitEffects/SGHit") as GameObject);

            muzzleFlash = GameObject.FindGameObjectWithTag("SGVFX").GetComponent<ParticleSystem>();

            //projectile = Instantiate(Resources.Load("projectiles/SGLine") as GameObject);

        }

        private void LateUpdate()
        {
            if (CrossHairScript.Instance.inFiringArea)
            {
                laserSightLine.enabled = true;

                laserSightLine.SetPosition(0, laserSight.transform.position);

                laserSightLine.SetPosition(1, CrossHairScript.Instance.hitLocation);

                if (Time.time > readyToFire && BallisticAmmo > 0)
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
        protected override void OnRightGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (BallisticAmmo > 0 && !PlayerMechControl.Instance.Dead && Time.timeScale != 0 && !GameControl.Instance.heatOverload && gameObject.activeInHierarchy)
                ShootShotgun();
        }


        public void ShootShotgun()
        {

            //            muzzleFlash.transform.position = barrel.transform.position;

            // Check crosshair is not inside cockpit and cooldown is done
            if (Time.time > readyToFire && CrossHairScript.Instance.inFiringArea)
            {

                AddGunHeat();

                BallisticAmmo -= 1;
                UpdateShotgunAmmoUI();



                for (int i = 0; i < 8; i++)
                {
                    Vector3 deviation = DeviateShots(12, -12);

                    if (deviation.x < -7)
                        deviation.x = -7;
                    else if (deviation.x > 7)
                        deviation.x = 7;

                    if (deviation.y < -7)
                        deviation.y = -7;
                    else if (deviation.y > 7)
                        deviation.y = 7;

                    if (Physics.Linecast(barrel.position, CrossHairScript.Instance.hitLocation + deviation, out hit))
                    {
                        // Show and hide muzzle flash
                        StartCoroutine(ShotGunEffects(hit));

                        if (Vector3.Distance(barrel.transform.position, hit.point) > Range)
                        {
                            Damage *= 0.2f;
                        }
                        else if (Vector3.Distance(barrel.transform.position, hit.point) > Range * 0.7)
                        {
                            Damage *= 0.5f;
                        }
                        else if (Vector3.Distance(barrel.transform.position, hit.point) > Range * 0.4)
                        {
                            Damage *= 0.7f;
                        }
                        else
                        {
                            Damage = 6 + (0.3f * upg.stormGunLevel);
                        }

                        int hitLocation = CheckHitLocation(hit);

                        // If it hits enemy mech part, get the TakeDamage script from it to call the damage function
                        if (hitLocation != 5 && hit.transform.gameObject.layer != 8)
                        {
                            takeDamage = hit.transform.GetComponentInParent<TakeDamage>();

                            takeDamage.EnemyDamage(Damage, LimbDmgMultiplier, hitLocation, enemyHitEffect, shooter, hit);
                        }
                    }
                }

                readyToFire = Time.time + CooldownTime;

            }

            else
            {
                weaponAudioSource.PlayOneShot(PlayerMechControl.Instance.unableToFire);
            }

        }


        void UpdateShotgunAmmoUI()
        {
            float maxAmount = maxAmmo;
            float fillAmount = BallisticAmmo / maxAmount;
            float time = 0.15f;
            ammoBar.DOFillAmount(fillAmount, time);
        }

        IEnumerator ShotGunEffects(RaycastHit pHit)
        {
            muzzleFlash.Play(true);

            weaponAudioSource.Play();

            yield return new WaitForSeconds(ShotDuration);

            muzzleFlash.Stop(true);
        }

    }
}