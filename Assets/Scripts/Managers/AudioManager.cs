using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour{
    public AudioSource ambientAudio, accAudio, flyMusic, themeAudio, playerSFX;
    public AudioClip soundBarrier;
    
    public float playerSpeed;
    
    private KinematicCharacterController kcc;
    
    private void Start(){
        kcc = GameObject.Find("Player").GetComponent<KinematicCharacterController>();
        flyMusic.Play();
        flyMusic.Pause();
    }

    private void Update(){
        playerSpeed = kcc.velMagnitude;
    }

    public void PlayShockAudio(){
        accAudio.PlayOneShot(accAudio.clip);
    }

    public void PlaySoundBarrier(){
        playerSFX.PlayOneShot(soundBarrier);
    }

    public void SwitchToFlyingMusic(){
        // ambientAudio.Stop();
        // themeAudio.Stop();
        // flyMusic.Play();
        ambientAudio.Pause();
        themeAudio.Pause();
        flyMusic.UnPause();
    }

    public void SwitchToAmbientMusic(){
        // ambientAudio.Play();
        // themeAudio.Play();
        // flyMusic.Stop();
        ambientAudio.UnPause();
        themeAudio.UnPause();
        flyMusic.Pause();
    }
}
