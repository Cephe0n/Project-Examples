using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using DG.Tweening;

namespace Mechalon
{
    public class Railgun : Weapons

    {
        public Text ChargeText;

        public Image ammoBar;
        public Image chargeBar;
        public AudioClip ChargeSound;
        public AudioClip FireSound;

        float chargeRate;
        float charge;

        float chargeHeat;

        protected override void Start()
        {

            base.Start();

            Damage = 30f;
            HeatPerShot = 10;
            Range = 10000f;
            LimbDmgMultiplier = 0.7f;
            CooldownTime = 1f;
            ShotDuration = 1f;
            BallisticAmmo = 5;

            chargeRate = 250f;
            charge = 0;
            chargeHeat = 0.7f;

            projectile = Instantiate(Resources.Load("projectiles/TestLaser") as GameObject);

            enemyHitEffect = Instantiate(Resources.Load("onHitEffects/AutoCannonHit") as GameObject);

            laser = projectile.GetComponent<LineRenderer>();

            barrel = transform.GetChild(0).transform;

            laser.transform.position = new Vector3(0, 0, 0);

            AmmoText.text = "RG Ammo: " + BallisticAmmo;

        }

        private void Update()
        {
            ChargeText.text = "Railgun charge: " + Mathf.Floor(charge);

            float chargeFillAmount = charge / 100f;
            float ammoFloat = BallisticAmmo;
            float ammoFillAmountRG = (ammoFloat / 5f);

            ammoBar.DOFillAmount(ammoFillAmountRG, 0.05f);
            chargeBar.DOFillAmount(chargeFillAmount, 0.05f);
        }

        protected override void OnRightGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (BallisticAmmo > 0 && !PlayerMechControl.Instance.Dead && gameObject.activeInHierarchy)
                InvokeRepeating("Charge", 0, 0.1f);
            else
            {
                // Sound effect for not ready to fire
            }

        }
        protected override void OnRightGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            weaponAudioSource.Stop();

            CancelInvoke();

            if (charge >= 95)
            {
                ShootRailgun();
            }

            charge = 0;


        }

        private void AddChargeHeat()
        {
            PlayerMechControl.Instance.AddHeat(chargeHeat);
        }


        void Charge()
        {
            weaponAudioSource.clip = ChargeSound;

            weaponAudioSource.Play();

            charge += chargeRate * Time.deltaTime;

            AddChargeHeat();

            if (charge > 100)
                charge = 100;
        }

        public void ShootRailgun()
        {

            // Check that crosshair is not inside cockpit
            if (CrossHairScript.Instance.inFiringArea && Time.time > readyToFire)
            {

                // Linecast from gun barrel onto crosshair's ray hit point
                if (Physics.Linecast(barrel.position, CrossHairScript.Instance.hitLocation, out hit))
                {

                    AddGunHeat();

                    BallisticAmmo -= 1;
                    AmmoText.text = "RG Ammo: " + BallisticAmmo;

                    float ammoFillAmount = this.BallisticAmmo / 5;

                    ammoBar.fillAmount = ammoFillAmount;

                    int hitLocation = CheckHitLocation(hit);

                    // Set linerender "laser" between gun and linecast hit point
                    laser.SetPosition(0, barrel.position);

                    laser.SetPosition(1, hit.point);

                    // Make laser visible for the shots duration
                    StartCoroutine(LaserEffects(ShotDuration, laser, hit));

                    // If it hits enemy mech part, get the enemymech script from it to call the damage function
                    if (hitLocation != 5)
                    {

                        takeDamage = hit.transform.GetComponentInParent<TakeDamage>();

                        takeDamage.EnemyDamage(Damage, LimbDmgMultiplier, CheckHitLocation(hit), enemyHitEffect, shooter, hit);
                    }
                    readyToFire = Time.time + CooldownTime;
                }

            }


        }


        IEnumerator LaserEffects(float pShotDuration, LineRenderer pLaserVisual, RaycastHit pHit)
        {

            pLaserVisual.enabled = true;

            weaponAudioSource.PlayOneShot(FireSound);

            yield return new WaitForSeconds(pShotDuration);

            pLaserVisual.enabled = false;
        }


    }
}