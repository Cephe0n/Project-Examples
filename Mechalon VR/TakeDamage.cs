using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Mechalon
{
    public class TakeDamage : MonoBehaviour
    {
        // Array of the objects children (limbs and torso)       
        // 0 = left arm, 1 = right arm, 2 = left leg, 3 = right leg, 4 = torso
        private GameObject[] mechParts;

        // Health values of each mech part.
        public float[] partHealths;
        public float LeftArmHealth;
        public float RightArmHealth;
        public float LeftLegHealth;
        public float RightLegHealth;
        public float TorsoHealth;
        AudioSource audioSource;
        public AudioClip deathSound;
        public AudioClip hitSound;
        public bool Dead;

        GameObject explosion;
        GameObject deathsplosion;
        
        Vector3 offset;


        VRTK_SlideObjectControlAction[] slidespeeds;

        // Use this for initialization
        void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            mechParts = new GameObject[] {transform.Find("hips/Torso/LeftArm").gameObject, transform.Find("hips/Torso/RightArm").gameObject, transform.Find("hips/leftLegUp").gameObject,
                    transform.Find("hips/rightLegUp").gameObject, transform.Find("hips/Torso").gameObject};
            partHealths = new float[] { LeftArmHealth, RightArmHealth, LeftLegHealth, RightLegHealth, TorsoHealth };

            explosion = Resources.Load("onHitEffects/Explosion") as GameObject;
            deathsplosion = Resources.Load("onHitEffects/Mechsplosion") as GameObject;

            offset = new Vector3(0, -7, 0);

            slidespeeds = GameObject.Find("PlayerMech").GetComponents<VRTK_SlideObjectControlAction>();
        }


        public void EnemyDamage(float pDmg, float pLimbMulti, int pPartHit, GameObject pHitEffect, GameObject pShooter, RaycastHit pHit)
        {

            // Weapon script sends hit part, compare that number to arrays
            if (pPartHit == 5)
            {
                // No hit
                return;
            }
            else if (pPartHit == 4)
            {
                // Torso hit
                partHealths[pPartHit] -= pDmg;

                if (pDmg > 0)
                    StartCoroutine(ShowHit(pPartHit, pHitEffect, pHit));

                audioSource.PlayOneShot(hitSound);
            }
            else if (pPartHit < 4)
            {
                // limbs hit, add limb dmg multiplier
                partHealths[pPartHit] -= pDmg * pLimbMulti;
                StartCoroutine(ShowHit(pPartHit, pHitEffect, pHit));

                audioSource.PlayOneShot(hitSound);
            }

            if (pPartHit == 3 || pPartHit == 2)
            {
                // Legs hit, if the most damaged one, add portion of damage to speed 
                if (Mathf.Min(partHealths[3], partHealths[2]) == partHealths[pPartHit])
                {
                    gameObject.GetComponent<EnemyMechs>().MoveSpeed -= pDmg / 8;

                    if (gameObject.GetComponent<EnemyMechs>().MoveSpeed < 5)
                        gameObject.GetComponent<EnemyMechs>().MoveSpeed = 5;
                }
            }

            // If part health 0, destroy it
            if (partHealths[pPartHit] <= 0)
            {
                mechParts[pPartHit].SetActive(false);

                // If torso or legs destroyed, kill mech
                if (pPartHit == 4 || pPartHit == 3 || pPartHit == 2)
                {
                    pShooter.GetComponent<ThreatScript>().Kills += 1;

                    if (pShooter.name == "PlayerMech")
                    {

                        if (GameControl.Instance.ChosenLeague == 0)
                            PlayerMechControl.Instance.MoneyEarned += 101000;

                        else if (GameControl.Instance.ChosenLeague == 1)
                            PlayerMechControl.Instance.MoneyEarned += 303000;

                        else if (GameControl.Instance.ChosenLeague == 2)
                            PlayerMechControl.Instance.MoneyEarned += 1010000;


                        Upgrades.Instance.AllKills += 1;
                        PlayerPrefs.SetInt("AllKills", Upgrades.Instance.AllKills);

                    }

                    GameControl.Instance.rankingList.Add(gameObject);
                    Instantiate(deathsplosion, mechParts[pPartHit].transform.position + offset, mechParts[pPartHit].transform.rotation);
                    deathsplosion.GetComponent<ParticleSystem>().Play(true);
                    Die();
                }
                else
                {
                    Instantiate(explosion, mechParts[pPartHit].transform.position, mechParts[pPartHit].transform.rotation);
                    explosion.GetComponent<ParticleSystem>().Play(true);
                    mechParts[pPartHit].SetActive(false);
                }
            }

        }

        void Die()
        {

            Dead = true;
            audioSource.PlayOneShot(deathSound);
            GameControl.Instance.EnemyKilled();
            gameObject.SetActive(false);
        }

        public void PlayerDamage(float pDmg, float pLimbMulti, int pPartHit, GameObject pHitEffect, GameObject pShooter)
        {

            // Weapon script sends hit part, compare that number to arrays
            if (pPartHit == 5)
            {
                // No hit
                return;
            }
            else if (pPartHit == 4)
            {
                partHealths[pPartHit] -= pDmg;
                StartCoroutine(ShowPlayerHit(pPartHit, pHitEffect));

                audioSource.PlayOneShot(hitSound);
            }
            else if (pPartHit < 4)
            {
                // If limbs hit, add limb dmg multiplier
                partHealths[pPartHit] -= pDmg * pLimbMulti;
                StartCoroutine(ShowPlayerHit(pPartHit, pHitEffect));

                audioSource.PlayOneShot(hitSound);
            }

            if (pPartHit == 3 || pPartHit == 2)
            {
                if (Mathf.Min(partHealths[3], partHealths[2]) == partHealths[pPartHit])
                {
                    for (int i = 0; i < slidespeeds.Length; i++)
                    {
                        slidespeeds[i].maximumSpeed -= pDmg / 8;

                        if (slidespeeds[i].maximumSpeed < 5)
                            slidespeeds[i].maximumSpeed = 5;
                    }
                }
            }
            // If part health 0, destroy it
            if (partHealths[pPartHit] <= 0)
            {

                // If torso or legs destroyed, kill mech
                if (pPartHit == 4 || pPartHit == 3 || pPartHit == 2)
                {
                    audioSource.PlayOneShot(deathSound);

                    GameControl.Instance.Death(pShooter);

                    pShooter.GetComponent<ThreatScript>().Kills += 1;
                }

                else

                {
                    Instantiate(explosion, mechParts[pPartHit].transform.position, mechParts[pPartHit].transform.rotation);
                    explosion.GetComponent<ParticleSystem>().Play(true);
                    mechParts[pPartHit].SetActive(false);
                }

            }
        }

        IEnumerator ShowHit(int pPartHit, GameObject pHitEffect, RaycastHit pHit)

        // Show hit effects & visual damage
        {
            ParticleSystem hitParticles = pHitEffect.GetComponent<ParticleSystem>();

            Instantiate(pHitEffect, pHit.point, Quaternion.identity);

            pHitEffect.transform.position = pHit.point;

            pHitEffect.SetActive(true);

            hitParticles.Play();

            // mechParts[pPartHit].GetComponent<Renderer>().material.color = Color.red;

            yield return new WaitForSeconds(0.2f);

            hitParticles.Stop();

            // mechParts[pPartHit].GetComponent<Renderer>().material.color = Color.gray;

            pHitEffect.SetActive(false);
        }
        IEnumerator ShowPlayerHit(int pPartHit, GameObject pHitEffect)

        // Show hit effects & visual damage
        {
            ParticleSystem hitParticles = pHitEffect.GetComponent<ParticleSystem>();

            Instantiate(pHitEffect, mechParts[pPartHit].transform);

            pHitEffect.transform.position = mechParts[pPartHit].transform.position;

            pHitEffect.SetActive(true);

            hitParticles.Play();

            // mechParts[pPartHit].GetComponent<Renderer>().material.color = Color.red;

            yield return new WaitForSeconds(0.2f);

            hitParticles.Stop();

            // mechParts[pPartHit].GetComponent<Renderer>().material.color = Color.gray;

            pHitEffect.SetActive(false);
        }

    }
}