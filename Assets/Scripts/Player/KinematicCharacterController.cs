using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class KinematicCharacterController : MonoBehaviour{
    // ===== MOVEMENT ROTATION =====
    [Header("Movement Rotation")]
    public float rotationStrength = 1f;
    public float alignmentSpeed = 10f;

    // ==== GENERAL ======
    private Vector2 input = Vector2.zero;
    private Vector3 currentNormal = Vector3.zero;
    private string currentGroundTag = null; 

    // ===== MOVEMENT ACCELERATION =====
    [Header("Movement Acceleration")]
    public float maxSpeed = 100f;
    public float absoluteMaxSpeed = 250f;
    public float minSpeed = 10f;
    public float acceleration = 10f;
    public float deceleration = 40f;
    public float passiveAirAcceleration = 10f;
    public float passiveDeceleration = 10f;
    public float rampMagnitudeMultiplier = 1.05f;
    private const float stasisTreshold = 0.01f;

    // ===== MOVEMENT VELOCITY VARIABLES =====
    [Header("Current Velocity")]
    public Vector3 currentVelocity = Vector3.zero;
    public float velMagnitude = 0;

    // ===== FALLING =====
    [Header("Falling")]
    public float gravity = 10f;
    public float fallGravityMultiplier = 1.5f; 
    public float acceleratedFallMultiplier = 5f;
    public float maxFallSpeed = 100f;
    private bool isGrounded = false;
    public float fallingVelocity = 0;

    // ===== JUMPING =====
    [Header("Jumping")]
    public float jumpVelocity = 1f;
    public int maxJumpCount = 2;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.1f;
    private bool isJumping = false;
    private float currentCoyote = 0;
    private float currentJumpBuffer = 0;
    private int currentJumpCount = 0;

    // ===== GLIDING =====
    [Header("Gliding")]
    public float updraftForce = 5f;
    public float updraftThreshold = 120f;
    public float momentumChangeThreshold = 60f;
    [Range(0f,1f)] public float percentageMomentumMaintained = .6f;
    public float glideGravityFactor = 0.1f;
    public float glideMaxFallSpeed = 10f;
    public float freezeFrameDuration = 0.1f;
    private bool isGliding = false;
    private bool isFrozen = false;

    // ===== COMPONENT REFERENCES =====
    public AudioManager audioManager;
    private ColliderUtil colUtil;
    private CharacterVFX vfx;

    void Start(){
        colUtil = GetComponent<ColliderUtil>();
        vfx = GetComponent<CharacterVFX>();
    }

    void Update(){
        if(isFrozen) return;

        // Get input from WASD and mouse and calculate velocity from it (using acceleration and shit) 
        // Axis acceleration is considered passive while mouse is active. Axis deceleration is active, while pressing nothing is passive.
        input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        bool isAccelerating = GetAccelerationInput();
        bool isDiving = GetDivingInput();
        transform.Rotate(Vector3.up * input.x * rotationStrength * Time.deltaTime);
        velMagnitude = CalculateCurrentVelocityMagnitude(input.y);
        Vector3 newVelocity = transform.forward * velMagnitude;

        // Check if grounded, and keeping track of coyote time timer to allow for delayed jumps
        isGrounded = CheckGrounded();
        if(isGrounded){
            isJumping=false;
            isGliding=false;
            currentJumpCount = 0;
            currentCoyote = coyoteTime;
        } else currentCoyote -= Time.deltaTime;

        // Jumping from air
        if(isJumping && currentJumpCount < maxJumpCount && Input.GetButtonDown("Jump")){
            fallingVelocity += CalculateJumpVelocity();
            currentJumpBuffer = 0;
            currentJumpCount++;
        }

        // Check for jump to buffer inputs. Allows for antecipated jumps, buffering a jump before the player actually hits the floor
        if(Input.GetButtonDown("Jump")){
            currentJumpBuffer = jumpBufferTime;
        } else currentJumpBuffer -= Time.deltaTime;

        // Jumping from the ground
        if(currentCoyote > 0 && currentJumpBuffer > 0){
            isJumping = true;
            currentCoyote = 0;
            currentJumpBuffer = 0;
            currentJumpCount = 1;
            fallingVelocity += CalculateJumpVelocity();
        }

        // Check to see if is grounded. If not, fall
        if(!isGrounded){
            fallingVelocity = CalculateFallingVelocity(isDiving);
        }

        // Check for gliding, apply updraft force
        if(!isGrounded && !isGliding && Input.GetButtonDown("Glide")){
            isGliding = true;
            velMagnitude += CalculateGlidingMomentumChange(fallingVelocity);
            fallingVelocity = CalculateUpdraftVelocity(fallingVelocity);
            vfx.StartGlide();
            FreezeFrame();
        } else if(!Input.GetButton("Glide")){
            isGliding = false;
        }

        // Updating and clamping velocity
        newVelocity.y += fallingVelocity;

        // Check for collision and slides across the collided surface if it happens
        Vector3 attemptedMovement = ((newVelocity+currentVelocity)/2) * Time.deltaTime;
        Vector3 newMovement = colUtil.CollideAndSlide(attemptedMovement, transform.position, 1, GetRampMultiplier());

        // Extra check to avoid falling through ground
        // TODO: Remove -normal actualy !!!!
        if(isGrounded) newMovement = SnapToGround(newMovement);
        if(isGrounded && fallingVelocity <= 0) fallingVelocity = 0;

        // Doing the character translation
        transform.position += newMovement;
        currentVelocity =  newMovement/Time.deltaTime;
    }

    private float CalculateCurrentVelocityMagnitude(float input){
        float acc = 0;
        float dec = 0;

        if(!isGrounded){
            acc = 0;
        } else if(input>0){
            acc = acceleration;
        } else if(input<0){
            dec = deceleration;
        } else dec = passiveDeceleration;
    
        float baseVel = Mathf.Clamp(velMagnitude+(acc*Time.deltaTime), minSpeed, maxSpeed);
        float vel = Mathf.Max(baseVel, velMagnitude);
        vel -= (dec*Time.deltaTime);

        vel = Mathf.Min(vel, absoluteMaxSpeed);
        return vel;
    }

    private float CalculateFallingVelocity(bool isAccelerating){
        float verticalVelocity = 0;
        float gravFactor = 1;
        float max = maxFallSpeed;

        if(fallingVelocity<0) gravFactor = fallGravityMultiplier;
        
        if(isGliding){
            gravFactor = glideGravityFactor;
            max = glideMaxFallSpeed;
        } else if(isAccelerating){
            gravFactor = acceleratedFallMultiplier;
        }
        
        verticalVelocity = Mathf.Max(fallingVelocity - (gravity * gravFactor * Time.deltaTime), -max);
        
        return verticalVelocity;
    }

    private float CalculateJumpVelocity(){
        float verticalVelocity = 0f;
        verticalVelocity = jumpVelocity;
        // the bigger the y velocity the higher the jump

        return verticalVelocity;
    }

    private float CalculateUpdraftVelocity(float _fallMag){
        float vel = _fallMag;
        if(vel <= -updraftThreshold){
            vel = updraftForce;
        } else if (vel >= updraftThreshold){
            vel = -updraftForce;
        } else vel = 0;

        return vel;
    }

    private float CalculateGlidingMomentumChange(float _fallMag){
        if(_fallMag < -momentumChangeThreshold){
            return -_fallMag * percentageMomentumMaintained;
        }
        
        return 0;
    }

    private bool CheckGrounded(){
        Vector3 normal;
        string tag;
        bool grd = colUtil.IsGroundedCast(transform.position, out normal, out tag);
        currentNormal = normal;
        currentGroundTag = tag;
        AlignToSurface();

        return grd;
    }

    private float GetRampMultiplier(){
        float dot = Vector3.Dot(Vector3.up, currentNormal);
        
        if(dot < .95f && dot > .05f){
            return rampMagnitudeMultiplier;
        }

        return 1;
    }

    private void AlignToSurface(){
        // Quaternion rot = Quaternion.FromToRotation(transform.up, currentNormal) * transform.rotation;
        // transform.rotation = Quaternion.Lerp(transform.rotation, rot, alignmentSpeed * Time.deltaTime); 

        // Quaternion rot = Quaternion.FromToRotation(transform.up, currentNormal);
        // rot = Quaternion.Lerp(transform.rotation, rot, alignmentSpeed * Time.deltaTime);
        // transform.rotation = Quaternion.Euler(rot.eulerAngles.x, transform.eulerAngles.y, rot.eulerAngles.x);

        // Vector3 proj = Vector3.ProjectOnPlane(transform.forward, currentNormal);
        // Quaternion rot = Quaternion.LookRotation(proj, currentNormal);
        // transform.rotation = rot;
    }

    private Vector3 SnapToGround(Vector3 _mov){
        Vector3 mov = _mov;

        // Vector3 antiNormal = Vector3.Project(mov, currentNormal);
        // mov -= antiNormal;

        mov.y = Mathf.Max(0, mov.y);

        return mov;
    }


    private bool GetAccelerationInput(){        
        if(Input.GetButton("Accelerate")) return true;

        else if(Input.GetAxis("Accelerate") > 0) return true;

        return false;
    }

    private bool GetDivingInput(){        
        if(Input.GetButton("Diving")) return true;

        else if(Input.GetAxis("Diving") > 0) return true;

        return false;
    }

    private void FreezeFrame(){
        isFrozen = true;
        StartCoroutine(UnfreezeFrame());
    }

    private IEnumerator UnfreezeFrame(){
        yield return new WaitForSecondsRealtime(freezeFrameDuration);
        isFrozen = false;
    }

    public Vector2 GetInput(){
        return input;
    }
    
    public bool GetIsGrounded(){
        return isGrounded;
    }

    public bool GetIsGliding(){
        return isGliding;
    }

    public float GetSpeed(){
        return velMagnitude;
    }

    public bool GetIsOnWater(){
        if(currentGroundTag == "Water") return true;
        return false;
    }

    public void Pause(){
        isFrozen = true;
    }

    public void Unpause(){
        isFrozen = false;
    }
}
