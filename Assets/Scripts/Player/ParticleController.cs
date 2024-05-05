using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleController : MonoBehaviour{
    public KinematicCharacterController kcc;
    
    // TIER VARIABLES
    public int thisTier = 0;
    private int currentTier = 0;
    private bool isEmitting = false;

    // KCC VARIABLES
    private bool onWater = true;
    private bool isGrounded = false;
    private float speed = 0;
    private bool isGliding = false;

    // WATER RIPPLE AND SPLATTER
    [Header("Water Ripple and Splatter")]
    public ParticleSystem ripple;
    public ParticleSystem splatter;
    public float rippleInterval = 0.5f;
    private float currentRipple = 0;
    
    // WATER FOAM
    [Header("Water Foam")]
    public VisualEffect foam;
    private bool isFoaming = false;

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

    // WIND
    [Header("Wind")]
    public ParticleSystem wind;

    private void LateUpdate(){
        UpdateVariables();
        TriggerParticles();
    }

    private void UpdateVariables(){
        isGrounded = kcc.GetIsGrounded();
        speed = kcc.GetSpeed();
        isGliding = kcc.GetIsGliding();
        onWater = kcc.GetIsOnWater();
    }


    public void UpdateTier(int _newTier){        
        currentTier = _newTier;
        if(thisTier == _newTier){
            StartEmitting();
        } else StopEmitting();
    }

    private void StartEmitting(){
        isEmitting = true;
        StartWind();
    }

    private void StopEmitting(){
        isEmitting = false;
        StopFoam();
        StopGlideTrail();
        StopWind();
    }

    private void TriggerParticles(){
        if(!isEmitting) return;

        if(onWater && isGrounded){
            TriggerRipple();
            StartFoam();
        } else if (isGrounded){
            TriggerDust();
            StopFoam();
        } else {
            StopFoam();
        }
        
        if (isGliding){
            TriggerGlideTrail();
        } else {
            StopGlideTrail();
        }
    }

    private void TriggerRipple(){
        currentRipple += Time.deltaTime;
        if(currentRipple>rippleInterval){
            currentRipple = 0;
            ripple.Play();
        }
    }

    private void StartFoam(){
        if(foam == null) return;

        if(!isFoaming){
            isFoaming = true;
            foam.enabled = true;
            foam.SendEvent("Start");
        }
    }

    private void StopFoam(){
        if(foam == null) return;

        isFoaming = false;
        foam.enabled = false;
        foam.SendEvent("Stop");
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
    }

    private void TriggerFeathers(){
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

    private void StartWind(){
        if(!wind) return;
        wind.Play();
    }

    private void StopWind(){
        if(!wind) return;
        wind.Stop();
    }

}
