using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource ambientAudio, accAudio, flyMusic, themeAudio;
    public float playerSpeed;
    private KinematicCharacterController kcc;
    private void Start()
    {
        kcc = GameObject.Find("Player").GetComponent<KinematicCharacterController>();
    }

    private void Update()
    {
        playerSpeed = kcc.velMagnitude;
    }

    public void PlayShockAudio()
    {
        accAudio.PlayOneShot(accAudio.clip);
    }
}
