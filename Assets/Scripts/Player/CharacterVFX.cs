using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVFX : MonoBehaviour{
    public Animator animator;
    public Transform model;
    private GameManager gameManager;
    private KinematicCharacterController kcc;
    
    private WwiseSoundManager wwiseSoundManager;

    public float maxSpeed = 150;
    private bool onWater = true;
    private bool isGrounded = false;
    private float speed = 0;
    private float previousSpeed = 0;
    private bool isGliding = false;
    private Vector2 input = Vector2.zero;
    private int currentTier = 0;

    // WATER RIPPLE AND SPLATTER
    [Header("Water Ripple and Splatter")]
    public ParticleSystem ripple;
    public ParticleSystem splatter;
    public float baseRippleInterval = 0.5f;
    public float rippleIntervalMultiplier = 1f;
    public float currentRipple = 0;
    public AnimationCurve rippleBurstPerSpeed;
    public AnimationCurve rippleVelocityPerSpeed;

    // GROUND DUST
    [Header("Ground Dust")]
    public ParticleSystem dust;
    public float baseDustInterval = 0.5f;
    public float dustIntervalMultiplier = 1f;
    public float currentDust = 0;

    // GLIDE FEATHER
    [Header("Gliding")]
    public ParticleSystem feathers;

    // GLIDE TRAIL
    public TrailRenderer glideTrailR;
    public TrailRenderer glideTrailL;

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
        gameManager = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();
        wwiseSoundManager = GameObject.FindObjectOfType<WwiseSoundManager>();
    }

    private void LateUpdate(){
        UpdateVariables();
        SetAnimatorState();
        UpdateModelRotation();
        TriggerParticles();
        UpdateShock();
    }

    private void UpdateVariables(){
        previousSpeed = speed;
        isGrounded = kcc.GetIsGrounded();
        speed = kcc.GetSpeed();
        isGliding = kcc.GetIsGliding();
        onWater = kcc.GetIsOnWater();
        input = kcc.GetInput();
    }

    private void SetAnimatorState(){ 
        animator.SetBool("TouchingGround", isGrounded);
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
        if(currentTier < _newTier) TriggerShock();
        
        currentTier = _newTier;
    }

    private void TriggerParticles(){
        if(onWater && isGrounded){
            TriggerRipple();
            PlaySplashSound();
        } else if (isGrounded){
            TriggerDust();
        }
        else
        {
            StopSplashSound();
        }
        
        if (isGliding){
            TriggerGlideTrail();
        } else {
            StopGlideTrail();
            if(wwiseSoundManager)
            {
                wwiseSoundManager.MusicStopGliding();
                if (isWindSoundPlaying) 
                {
                    wwiseSoundManager.StopWindSound();
                    isWindSoundPlaying = false;
                }
            }
        }
    }

    private void TriggerRipple(){
        var em = splatter.emission;
        em.SetBursts(new ParticleSystem.Burst[]{
            new ParticleSystem.Burst(0, rippleBurstPerSpeed.Evaluate(speed/maxSpeed))
        });
        var main = splatter.main;
        main.startSpeed = rippleVelocityPerSpeed.Evaluate(speed/maxSpeed);

        float rippleInterval = baseRippleInterval - rippleIntervalMultiplier * speed;
        currentRipple += Time.deltaTime;
        if(currentRipple>rippleInterval){
            currentRipple = 0;
            ripple.Play();
        }
    }

    private void TriggerDust(){
        float dustInterval = baseDustInterval - dustIntervalMultiplier * speed;
        currentDust += Time.deltaTime;
        if(currentDust>dustInterval){
            currentDust=0;
            dust.Play();
        }
    }

    public void StartGlide(){
        TriggerFeathers();
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

    private void TriggerFeathers(){
        feathers.Play();
        if(wwiseSoundManager) wwiseSoundManager.PlayRandomFeather();
    }

    private void TriggerGlideTrail(){
        glideTrailR.emitting = true;
        glideTrailL.emitting = true;
    }

    private void StopGlideTrail(){
        glideTrailR.emitting = false;
        glideTrailL.emitting = false;
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

    //Sound Real-Time Parameter Setup
    //change the value to adjust pitch, HighPass, and LowPass
    //(0, 100), 0 is default value
    //you can dynamically change the value, maybe for different tier of speed you have different parameters

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

    private void PlaySplashSound()
    {
        if (onWater && !isSplashPlaying && wwiseSoundManager != null)
        {
            wwiseSoundManager.PlaySplash(); 
            isSplashPlaying = true;
        }
    }

    private void StopSplashSound(){
    if (!onWater && isSplashPlaying && wwiseSoundManager != null)
    {
        wwiseSoundManager.StopSplash(); 
        isSplashPlaying = false;
    }

}
}
