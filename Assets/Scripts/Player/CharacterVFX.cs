using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVFX : MonoBehaviour{
    public Animator animator;
    private KinematicCharacterController kcc;

    public float maxSpeed = 150;

    private bool onWater = true;
    private bool isGrounded = false;
    private float speed = 0;
    private float previousSpeed = 0;
    private bool isGliding = false;

    public ParticleSystem ripple;
    public float baseRippleInterval = 0.5f;
    public float rippleIntervalMultiplier = 1f;
    public float currentRipple = 0;
    public AnimationCurve rippleBurstPerSpeed;
    public AnimationCurve rippleVelocityPerSpeed;

    public ParticleSystem dust;
    public float baseDustInterval = 0.5f;
    public float dustIntervalMultiplier = 1f;
    public float currentDust = 0;

    public ParticleSystem feathers;

    public TrailRenderer glideTrailR;
    public TrailRenderer glideTrailL;

    public Material shockwave;
    public float speedToTriggerShock = 90f;
    public float shockDuration = 1f;
    private bool isShocking = false;
    private float shockStart = -5f;
    private float shockEnd = 1.5f;
    public float shockElapsed = 0;


    private void Start(){
        kcc = GetComponent<KinematicCharacterController>();
    }

    private void LateUpdate(){
        UpdateVariables();
        SetAnimatorState();
        TriggerParticles();
        UpdateShock();
    }

    private void UpdateVariables(){
        previousSpeed = speed;
        isGrounded = kcc.GetIsGrounded();
        speed = kcc.GetSpeed();
        isGliding = kcc.GetIsGliding();
        onWater = kcc.GetIsOnWater();
    }

    private void SetAnimatorState(){
        animator.SetBool("TouchingGround", isGrounded);
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGliding", isGliding);
    }

    private void TriggerParticles(){
        if(onWater && isGrounded){
            TriggerRipple();
        } else if (isGrounded){
            TriggerDust();
        }
        
        if (isGliding){
            TriggerGlideTrail();
        } else StopGlideTrail();

        TriggerShock();
    }

    private void TriggerRipple(){
        // ripple.SetCustomParticleData()


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

    public void TriggerFeathers(){
        feathers.Play();
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
        if(!isShocking && previousSpeed < speedToTriggerShock && speed > speedToTriggerShock){
            isShocking = true;
            shockElapsed = 0;
        }
    }

    private void UpdateShock(){
        if(!isShocking) return;
        shockElapsed += Time.deltaTime;
        shockwave.SetFloat("_Progress", Mathf.Lerp(shockStart, shockEnd, shockElapsed/shockDuration));
        
        if(shockElapsed > shockDuration) isShocking = false;
    }
}
