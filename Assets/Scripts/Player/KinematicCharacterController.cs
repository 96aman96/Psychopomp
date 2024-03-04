using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KinematicCharacterController : MonoBehaviour{
    // ===== MOVEMENT ROTATION =====
    [Header("Movement Rotation")]
    public float rotationStrength = 1f;
    private Vector3 movementVector = Vector3.zero;

    // ===== MOVEMENT ACCELERATION =====\
    [Header("Movement Acceleration")]
    public float maxSpeed = 100f;
    public float maxPassiveSpeed = 40f;
    public float minSpeed = 10f;
    public float acceleration = 10f;
    public float deceleration = 40f;
    public float passiveAcceleration = 10f;
    public float passiveAirAcceleration = 10f;
    public float passiveDeceleration = 10f;
    public Vector3 currentVelocity = Vector3.zero;
    private float velMagnitude = 0;
    private const float stasisTreshold = 0.01f;

    // ===== FALLING =====
    [Header("Falling")]
    public float gravity = 10f;
    public float fallGravityMultiplier = 1.5f; 
    public float maxFallSpeed = 100f;
    private bool isGrounded = false;

    // ===== JUMPING =====
    [Header("Jumping")]
    public float jumpVelocity = 1f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.1f;
    private bool isJumping = false;
    private float currentCoyote = 0;
    private float currentJumpBuffer = 0;

    // ===== GLIDING =====
    [Header("Gliding")]
    public float updraftForce = 5f;
    [Range(0f,1f)] public float percentageUpdraftUpwards = .6f;
    public float glideGravityFactor = 0.1f;
    public float glideMaxFallSpeed = 10f;
    private bool isGliding = false;

    // ===== COMPONENT REFERENCES =====
    private ColliderUtil colUtil;
    private CharacterState state;

    void Start(){
        colUtil = GetComponent<ColliderUtil>();
        state = GetComponent<CharacterState>();
        movementVector = transform.forward;
    }

    void Update(){
        // Get input from WASD and mouse and calculate velocity from it (using acceleration and shit) 
        // Axis acceleration is considered passive while mouse is active. Axis deceleration is active, while pressing nothing is passive.
        Vector2 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        bool isAccelerating = Input.GetMouseButton(0);
        transform.Rotate(Vector3.up * input.x * rotationStrength);
        velMagnitude = CalculateCurrentVelocityMagnitude(input.y, isAccelerating);
        Vector3 newVelocity = transform.forward * velMagnitude;

        // Check if grounded, and keeping track of coyote time timer to allow for delayed jumps
        isGrounded = CheckGrounded();
        if(isGrounded){
            isJumping=false;
            isGliding=false;
            currentCoyote = coyoteTime;
        } else currentCoyote -= Time.deltaTime;

        // Check for jump to buffer inputs. Allows for antecipated jumps, buffering a jump before the player actually hits the floor
        if(Input.GetMouseButtonUp(0)){
            currentJumpBuffer = jumpBufferTime;
        } else currentJumpBuffer -= Time.deltaTime;

        // Jumping from the ground
        if(currentCoyote > 0 && currentJumpBuffer > 0){
            isJumping = true;
            currentCoyote = 0;
            currentJumpBuffer = 0;
            newVelocity += CalculateJumpVelocity();
        }

        // Check for gliding, apply updraft force
        if(!isGrounded && !isGliding && Input.GetKey("left shift")){
            isGliding = true;
            newVelocity += CalculateUpdraftVelocity();
        } else if(Input.GetKeyUp("left shift")){
            isGliding = false;
        }

        // Check to see if is grounded. If not, fall
        if(!isGrounded){
            Vector3 newFallVelocity = CalculateFallingVelocity();
            newVelocity += newFallVelocity;
        }

        // Set state, for animation/visual purposes
        SetState(isAccelerating);

        // Check for collision and slides across the collided surface if it happens
        Vector3 attemptedMovement = ((newVelocity+currentVelocity)/2) * Time.deltaTime;
        Vector3 newMovement = colUtil.CollideAndSlide(attemptedMovement, transform.position, 1);

        // Doing the character translation
        transform.position += newMovement;
        currentVelocity =  newMovement/Time.deltaTime;
    }

    private float CalculateCurrentVelocityMagnitude(float input, bool isAccelerating){
        float mag = new Vector3(currentVelocity.x,0,currentVelocity.z).magnitude;
        float acc = -passiveDeceleration;
        float max = maxPassiveSpeed;

        if(!isGrounded){
            max = maxSpeed;
            acc = 0;
        } else if(isAccelerating && isGrounded){
            max = maxSpeed;
            acc = acceleration;
        } else if(input>0){
            acc = passiveAcceleration;
        } else if(input<0) acc = -deceleration;

        // float cap = Mathf.Clamp(velMagnitude+(acc*Time.deltaTime), minSpeed, max);
        // float vel = Mathf.Max(cap, velMagnitude);
        // float cap = Mathf.Clamp(mag+(acc*Time.deltaTime), minSpeed, max);
        // float vel = Mathf.Max(cap, mag);

        float vel = Mathf.Clamp(velMagnitude+(acc*Time.deltaTime), minSpeed, max);

        return vel;
    }

    private Vector3 CalculateFallingVelocity(){
        Vector3 verticalVelocity = Vector3.zero;
        float gravFactor = 1;
        float max = maxFallSpeed;

        if(currentVelocity.y<0) gravFactor *= fallGravityMultiplier;
        if(isGliding){
            gravFactor = glideGravityFactor;
            max = glideMaxFallSpeed;
        }
        
        verticalVelocity.y = Mathf.Clamp(currentVelocity.y - (gravity * gravFactor * Time.deltaTime), -max, Mathf.Infinity);
        
        return verticalVelocity;
    }

    private Vector3 CalculateJumpVelocity(){
        Vector3 verticalVelocity = Vector3.zero;
        verticalVelocity.y = jumpVelocity;
        // the bigger the y velocity the higher the jump

        return verticalVelocity;
    }

    private Vector3 CalculateUpdraftVelocity(){
        Vector3 vel = Vector3.zero;
        vel.y = updraftForce * percentageUpdraftUpwards;
        vel += (1-percentageUpdraftUpwards) * updraftForce * transform.forward;
        return vel;
    }

    private bool CheckGrounded(){
        return colUtil.IsGroundedCast(transform.position);
    }

    private void SetState(bool isAccelerating){
        if(isGliding){
            state.ChangeState(State.Glide);
        } else if(isJumping){
            state.ChangeState(State.Jump);
        } else if(isAccelerating){
            state.ChangeState(State.Accelerated);
        } else {
            state.ChangeState(State.Idle);
        }
    }

    private Vector3 CalculateDirection(Vector3 input){
        if(input == Vector3.zero) return input;

        movementVector = (movementVector + (input*rotationStrength)).normalized;
        
        return movementVector;
    }

    private Vector3 AbsoluteToCameraDirection(Vector3 dir){
        Vector3 fwd = Camera.main.transform.forward;
        Vector3 rgt = Camera.main.transform.right;

        // Project on xz plane
        fwd.y = 0f;
        rgt.y = 0f;
        fwd.Normalize();
        rgt.Normalize();

        return rgt * dir.x + fwd * dir.z;
    }

    private Vector3 AbsoluteToPlayerDirection(Vector3 dir){
        Vector3 fwd = transform.forward;
        Vector3 rgt = transform.right;

        // Project on xz plane
        fwd.y = 0f;
        rgt.y = 0f;
        fwd.Normalize();
        rgt.Normalize();

        return rgt * dir.x + fwd * dir.z;
    }
}
