using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
using UnityEngine.Events;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class GameControl : MonoBehaviour
{

    public GameObject LeftArm, RightArm, LeftLeg, RightLeg, Head, Torso, CurrentSpawnedZombie, LimbTub, ClockHand;
    public Transform LimbSpawnPoint, ZombieSpawnPoint, ZombieDeSpawnPoint, SlabStopPoint;
    public bool limbReadyToSpawn, zombieReadyToSpawn = true;

    public int LimbsAttached, Score = 0;

    public float Timer;

    public Text ScoreText, RatingText;
    bool gameOver, gameStarted;

    public Animator Clock;

    // Start is called before the first frame update
    void Start()
    {
        Timer = 180f;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameStarted)
        Timer -= Time.deltaTime;

        if (Timer <= 0 && !gameOver)
        {
            MasterAudio.PlaySoundAndForget("GameOver");
            RatingText.text = "Shift over!\nFinal  Rating: ";
            gameOver = true;
            MasterAudio.StopAllPlaylists();
        }
    }


    public void SpawnLimbs()
    {
        if (limbReadyToSpawn && !gameOver && gameStarted)
        {
            StartCoroutine(LimbSpawn());

        }
    }

    public void SpawnNewZombie()
    {
        if (!gameOver || !gameStarted)
        {
            gameStarted = true;
            MasterAudio.PlaySoundAndForget("ReadyButton");
            Clock.Play("clock");

            if (zombieReadyToSpawn)
            {
                LimbsAttached = 0;
                Instantiate(Torso, ZombieSpawnPoint.position, Quaternion.identity);
                CurrentSpawnedZombie = GameObject.FindGameObjectWithTag("Torso");
                CurrentSpawnedZombie.transform.rotation = Quaternion.Euler(90, 0, 180);
                //CurrentSpawnedZombie.transform.position = Vector3.MoveTowards(transform.position, SlabStopPoint.position, 1f * Time.deltaTime);
                CurrentSpawnedZombie.transform.DOMove(SlabStopPoint.position, 5f);
                StartCoroutine(PlayLoop(5f));
                zombieReadyToSpawn = false;

            }
            else if (!zombieReadyToSpawn && (Vector3.Distance(CurrentSpawnedZombie.transform.position, SlabStopPoint.position) < 0.2f))
            {
                if (CurrentSpawnedZombie != null)
                {
                    StartCoroutine(PlayLoop(7f));
                    CurrentSpawnedZombie.transform.DOMove(ZombieDeSpawnPoint.position, 7f);
                    var limbs = FindObjectsOfType<LimbScript>();

                    for (int i = 0; i < limbs.Length; i++)
                    {
                        if (!limbs[i].isAttached)
                        limbs[i].GetComponent<Rigidbody>().isKinematic = false;
                    }
                }
            }
        }
        else
        {
            SceneManager.LoadScene(0);
        }

    }


    public void RepairSuccess()
    {
        Score += 1;
        ScoreText.text = "" + Score;
        MasterAudio.PlaySoundAndForget("RepairCorrect");
    }

    public void RepairFail()
    {
        Score -= 1;
        ScoreText.text = "" + Score;
        MasterAudio.PlaySoundAndForget("RepairWrong");
    }

    IEnumerator LimbSpawn()
    {
        MasterAudio.PlaySoundAndForget("LimbButton");

        limbReadyToSpawn = false;
        Instantiate(LeftArm, LimbSpawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        MasterAudio.PlaySound3DAtTransformAndForget("LimbDrop", LimbTub.transform);
        Instantiate(RightArm, LimbSpawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(LeftLeg, LimbSpawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(RightLeg, LimbSpawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(Head, LimbSpawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        limbReadyToSpawn = true;
    }

    IEnumerator PlayLoop(float pSecsToWait)
    {
        MasterAudio.PlaySound3DAtTransformAndForget("ConveyorMoveLoop", CurrentSpawnedZombie.transform);
        yield return new WaitForSeconds(pSecsToWait);
        MasterAudio.StopAllOfSound("ConveyorMoveLoop");
    }

}
