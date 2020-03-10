using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using VRTK;

namespace Mechalon
{
    public class HeavyLaser : Weapons
    {
        public AudioClip firingSound;
        protected override void Start()
        {

            base.Start();

            Damage = 1f;
            HeatPerShot = 0.9f;
            Range = 10000f;
            LimbDmgMultiplier = 1.5f;
            CooldownTime = 2.5f;
            ShotDuration = 3f;

            projectile = Instantiate(Resources.Load("projectiles/TestLaser") as GameObject);

            enemyHitEffect = Instantiate(Resources.Load("onHitEffects/AutoCannonHit") as GameObject);

            laser = projectile.GetComponent<LineRenderer>();

            barrel = transform.GetChild(0).transform;

            laser.transform.position = new Vector3(0, 0, 0);

        }

        protected override void OnLeftGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (!PlayerMechControl.Instance.Dead && gameObject.activeInHierarchy)
                InvokeRepeating("ShootHeavyLaser", 0, CooldownTime * Time.deltaTime);
        }
        protected override void OnLeftGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            CancelInvoke();
            laser.enabled = false;
            weaponAudioSource.Stop();
        }

        public void ShootHeavyLaser()
        {

            // Check if cooldown is done and crosshair is not inside cockpit
            if (CrossHairScript.Instance.inFiringArea)
            {

                // Linecast from gun barrel onto crosshair's ray hit point
                if (Physics.Linecast(barrel.position, CrossHairScript.Instance.hitLocation, out hit))
                {
                    AddGunHeat();

                    weaponAudioSource.PlayOneShot(firingSound);

                    laser.enabled = true;

                    Debug.DrawRay(barrel.position, CrossHairScript.Instance.hitLocation, Color.green, 5);

                    int hitLocation = CheckHitLocation(hit);

                    // Set linerender "laser" between gun and linecast hit point
                    laser.SetPosition(0, barrel.position);

                    laser.SetPosition(1, hit.point);

                    // Make laser visible for the shots duration
                    //StartCoroutine(LaserEffects(ShotDuration, laser, hit));

                    // If it hits enemy mech part, get the enemymech script from it to call the damage function
                    if (hitLocation != 5)
                    {
                        takeDamage = hit.transform.GetComponentInParent<TakeDamage>();

                        takeDamage.EnemyDamage(Damage, LimbDmgMultiplier, hitLocation, enemyHitEffect, shooter, hit);
                    }
                }

            }
            else
            {
                laser.enabled = false;

                // Sound effect for not ready to fire
            }

        }


        IEnumerator LaserEffects(float pShotDuration, LineRenderer pLaserVisual, RaycastHit pHit)
        {

            pLaserVisual.enabled = true;

            yield return new WaitForSeconds(pShotDuration);

            pLaserVisual.enabled = false;
        }

    }
}