using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.VFX;

public class CharacterVFX : MonoBehaviour{
    public Animator animator;
    public Transform model;
    public KinematicCharacterController kcc;
    
    private WwiseSoundManager wwiseSoundManager;

    public float maxSpeed = 150;
    private bool onWater = true;
    private bool isGrounded = false;
    private bool isBroadlyGrounded = false;
    private float speed = 0;
    private float previousSpeed = 0;
    private bool isGliding = false;
    private Vector2 input = Vector2.zero;
    private int tier = 0;

    // Animator Variables for Pausing
    private float prevSpeed = 0;

    // PARTICLE SYSTEMS
    [Header("Particle Systems")]
    public ParticleController[] particleSystems;

    // SHOCKWAVE SOUND BARRIER THING
    [Header("Shockwave")]
    public Material shockwave;
    public float speedToTriggerShock = 90f;
    public float shockDuration = 1f;
    private bool isShocking = false;
    private float shockStart = -5f;
    private float shockEnd = 1.5f;
    public float shockElapsed = 0;
    private bool shockSFXPlayed = true;

    // CHARACTER MODEL TILT
    [Header("Model Tilt")]
    public float maxTilt = 30f;
    public float tiltIntensity = 1f;
    public float tiltVelocityMultiplier = 1f;
    
    // SOUND FLAGS
    private bool isWindSoundPlaying = false;
    private bool isSplashPlaying = false;


    private void Start(){
        kcc = GetComponent<KinematicCharacterController>();
        wwiseSoundManager = GameObject.FindObjectOfType<WwiseSoundManager>();
        UpdateTier(0);
    }

    private void LateUpdate(){
        UpdateVariables();
        SetAnimatorState();
        UpdateModelRotation();
        UpdateShock();
    }

    private void UpdateVariables(){
        previousSpeed = speed;

        isGrounded = kcc.GetIsGrounded();
        isBroadlyGrounded = kcc.GetIsBroadlyGrounded();
        speed = kcc.GetSpeed();
        isGliding = kcc.GetIsGliding();
        onWater = kcc.GetIsOnWater();
        input = kcc.GetInput();

        if(!isGliding) StopGlide();
    }

    private void SetAnimatorState(){ 
        animator.SetBool("TouchingGround", isBroadlyGrounded);
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGliding", isGliding);
    }

    private void UpdateModelRotation(){
        float target = 0;
        if(speed>80){
            target = -input.x * maxTilt;
        }

        float lerpSpeed = tiltIntensity * Time.deltaTime;

        Quaternion tgt = Quaternion.Euler(model.localEulerAngles.x, model.localEulerAngles.y, target);
        model.localRotation = Quaternion.Lerp(model.localRotation, tgt, lerpSpeed);
    }

    public void UpdateTier(int _newTier){
        if(tier<_newTier && _newTier>1) TriggerShock();
        
        tier = _newTier;
        foreach(ParticleController pc in particleSystems){
            pc.UpdateTier(tier);
        }
    }

    public void StartGlide(){
        if(wwiseSoundManager) 
        {
            wwiseSoundManager.MusicStartGliding();
            if (!isWindSoundPlaying) {
                float pitchValue = CalculatePitch();
                float lowPassValue = CalculateLowPass();
                float highPassValue = CalculateHighPass();
                wwiseSoundManager.PlayWindSound(pitchValue, lowPassValue, highPassValue);
                isWindSoundPlaying = true;
            }
        }
    }

    public void StopGlide(){
        wwiseSoundManager.MusicStopGliding();
    }

    private void TriggerShock(){
        if(!isShocking){
            isShocking = true;
            shockSFXPlayed = false;
            shockElapsed = 0;
        }
    }

    private void UpdateShock(){
        if(!isShocking) return;
        shockElapsed += Time.deltaTime;
        shockwave.SetFloat("_Progress", Mathf.Lerp(shockStart, shockEnd, shockElapsed/shockDuration));
        
        if(!shockSFXPlayed && (shockElapsed/shockDuration)>0.7f){
            shockSFXPlayed = true;
            if(wwiseSoundManager) wwiseSoundManager.PlaySoundWave();
        }

        if(shockElapsed > shockDuration)isShocking = false;
    }


    //I suggest when player glides up, LowPass++; when player dives, low pass --, high pass ++, pitch ++
    float CalculatePitch()
    {
        return 10f; 
    }

    float CalculateHighPass()
    {
        return 50f; 
    }

    float CalculateLowPass()
    {
        return 20f; 
    }

    public void PlaySplashSound()
    {
        if (onWater && !isSplashPlaying && wwiseSoundManager != null)
        {
            wwiseSoundManager.PlaySplash(); 
            isSplashPlaying = true;
        }
    }

    public void StopSplashSound()
    {
        if (!onWater && isSplashPlaying && wwiseSoundManager != null)
        {
            wwiseSoundManager.StopSplash(); 
            isSplashPlaying = false;
        }
    }

    public void PlayFeather()
    {
        wwiseSoundManager.PlayRandomFeather();
    }

    public void Pause(){
        prevSpeed = animator.speed;
        animator.speed = 0;
    }

    public void Unpause(){
        animator.speed = prevSpeed;
    }

}

