using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KinematicCharacterController : MonoBehaviour{
    // ===== MOVEMENT DIRECTION =====
    [Header("Movement Rotation")]
    public float rotationStrength = 1f;
    private Vector3 movementVector = Vector3.zero;

    // ===== MOVEMENT ACCELERATION =====\
    [Header("Movement Acceleration")]
    public float maxSpeed = 100f;
    public float minSpeed = 10f;
    public float acceleration = 10f;
    public float deceleration = 40f;
    public float passiveDeceleration = 10f;
    private Vector3 currentVelocity = Vector3.zero;
    private float velMagnitude = 0;
    private const float stasisTreshold = 0.01f;

    // ===== FALLING =====
    [Header("Falling")]
    public float fallAcceleration = 10f;
    public float maxFallSpeed = 100f;
    public float glideFactor = 0.5f;
    private bool isGrounded = false;

    // ===== JUMPING =====
    [Header("Jumping")]
    public float jumpVelocity = 1f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.1f;
    private bool isJumping = false;
    private float currentCoyote = 0;
    private float currentJumpBuffer = 0;

    // ===== MOVEMENT ROTATION =====
    [Header("Rotation")]
    public float lookRotationSpeed = 1f;

    // ===== COLLIDER VARIABLES =====
    [Header("Collider")]
    private ColliderUtil colUtil;
    public float floorDistance = 0.1f;

    // ===== STATE CONTROLLER =====
    private CharacterState state;

    void Start(){
        colUtil = GetComponent<ColliderUtil>();
        state = GetComponent<CharacterState>();
        movementVector = transform.forward;
    }

    void Update(){
        // Get input from WASD and calculate velocity from it (using acceleration and shit)
        Vector2 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.Rotate(Vector3.up * input.x * rotationStrength);
        velMagnitude = CalculateCurrentVelocityMagnitude(input.y);
        Vector3 newVelocity = transform.forward * velMagnitude;

        // Check if grounded, and keeping track of coyote time timer to allow for delayed jumps
        isGrounded = CheckGrounded();
        if(isGrounded){
            isJumping=false;
            currentCoyote = coyoteTime;
        } else currentCoyote -= Time.deltaTime;

        // Check for jump to buffer inputs. Allows for antecipated jumps, buffering a jump before the player actually hits the floor
        if(Input.GetKeyDown("space")){
            currentJumpBuffer = jumpBufferTime;
        } else currentJumpBuffer -= Time.deltaTime;

        // Jumping from the ground
        if(currentCoyote > 0 && currentJumpBuffer > 0){
            isJumping = true;
            currentCoyote = 0;
            currentJumpBuffer = 0;
            newVelocity += CalculateJumpVelocity();
        }

        // Check to see if is grounded. If not, fall
        if(!isGrounded){
            Vector3 newFallVelocity = CalculateFallingVelocity();
            newVelocity += newFallVelocity;
        }

        // Set state, for animation/visual purposes
        SetState(input.y);

        // Check for Collision and Sliding on XZ plane and on the Y axis
        Vector3 attemptedMovement = ((newVelocity+currentVelocity)/2) * Time.deltaTime;
        Vector3 newMovement = colUtil.XZCollideAndSlide(new Vector3(attemptedMovement.x,0,attemptedMovement.z), transform.position, 1);
        newMovement += colUtil.YCollideAndSlide(new Vector3(0,attemptedMovement.y,0), transform.position);

        // Doing the character translation
        transform.position += newMovement;
        // currentVelocity = newVelocity;
        currentVelocity =  newMovement/Time.deltaTime;
    }

    private float CalculateCurrentVelocityMagnitude(float input){
        float acc = -passiveDeceleration;
        if(input > 0){
            acc = acceleration;
        } else if(input<0) acc = -deceleration;

        float vel = Mathf.Clamp(velMagnitude+(acc*Time.deltaTime), minSpeed, maxSpeed);

        return vel;
    }

    private Vector3 CalculateFallingVelocity(){
        Vector3 verticalVelocity = Vector3.zero;
        verticalVelocity.y = Mathf.Clamp(currentVelocity.y - (fallAcceleration * Time.deltaTime), -maxFallSpeed, Mathf.Infinity);
        
        // if(Input.GetKey("left shift")) verticalVelocity *= glideFactor;

        return verticalVelocity;
    }

    private Vector3 CalculateJumpVelocity(){
        Vector3 verticalVelocity = Vector3.zero;
        verticalVelocity.y = jumpVelocity;
        // the bigger the y velocity the higher the jump

        return verticalVelocity;
    }

    private bool CheckGrounded(){
        return colUtil.IsGroundedCast(transform.position);
    }

    private void SetState(float input){
        if(isJumping){
            state.ChangeState(State.Jump);
        } else if(input>0){
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
