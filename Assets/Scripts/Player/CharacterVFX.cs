using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVFX : MonoBehaviour{
    public Animator animator;
    private KinematicCharacterController kcc;

    public bool onWater = true;

    public ParticleSystem ripple;
    public float baseRippleInterval = 0.5f;
    public float rippleIntervalMultiplier = 1f;
    public float currentRipple = 0;

    public ParticleSystem feathers;

    private bool isGrounded = false;
    private float speed = 0;
    private bool isGliding = false;

    private void Start(){
        kcc = GetComponent<KinematicCharacterController>();
    }

    private void LateUpdate(){
        UpdateVariables();
        SetAnimatorState();
        TriggerParticles();
    }

    private void UpdateVariables(){
        isGrounded = kcc.GetIsGrounded();
        speed = kcc.GetSpeed();
        isGliding = kcc.GetIsGliding();
    }

    private void SetAnimatorState(){
        animator.SetBool("TouchingGround", isGrounded);
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGliding", isGliding);
    }

    private void TriggerParticles(){
        if(onWater && isGrounded && speed <= 95){
            TriggerRipple();
        }
    }

    private void TriggerRipple(){
        float rippleInterval = baseRippleInterval - rippleIntervalMultiplier * speed;

        currentRipple += Time.deltaTime;
        if(currentRipple>rippleInterval){
            currentRipple = 0;
            ripple.Play();
        }
    }

    public void TriggerFeathers(){
        feathers.Play();
    }
}
