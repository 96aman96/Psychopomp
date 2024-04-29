using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class KinematicCharacterController : MonoBehaviour{
    // ===== MOVEMENT ROTATION =====
    [Header("Movement Rotation")]
    public float rotationStrength = 1f;
    public float yzRotationStrength = 100f;
    public float yzUpRotationSpeed = 100f;
    public float yzDownRotationSpeed = 100f;
    public float yzReturnRotationSpeed = 100f;
    public float maxYZRotationAngle = 60f;

    // ==== GENERAL ======
    private Vector2 input = Vector2.zero;
    private Vector3 currentNormal = Vector3.zero;
    private string currentGroundTag = null; 

    // ===== MOVEMENT ACCELERATION =====
    [Header("Movement Acceleration")]
    public float maxSpeed = 100f;
    public float[] speedTiers = new float[4]{50f, 100f, 150f, 200f};
    public float absoluteMaxSpeed = 250f;
    public float minSpeed = 10f;
    public float acceleration = 10f;
    public float deceleration = 40f;
    public float passiveDeceleration = 10f;
    public float rampMagnitudeMultiplier = 1.05f;

    // ===== MOVEMENT VELOCITY VARIABLES =====
    [Header("Current Velocity")]
    public Vector3 currentVelocity = Vector3.zero;
    public float velMagnitude = 0;
    public float effectiveVelMagnitude = 0;
    public int currentTier = 0;

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
    public float glideTimeLimit = 4f;
    public float glideGravityFactor = 0.1f;
    public float glideMaxFallSpeed = 10f;
    public float freezeFrameDuration = 0.1f;
    public float glideDeceleration = 20f;
    public float glideAcceleration = 20f;
    private bool isGliding = false;
    private bool isFrozen = false;
    private float glideTimer = 0f;

    // ===== COMPONENT REFERENCES =====
    public AudioManager audioManager;
    public CameraController cameraController;
    private ColliderUtil colUtil;
    private CharacterVFX vfx;

    void Start(){
        colUtil = GetComponent<ColliderUtil>();
        vfx = GetComponent<CharacterVFX>();
    }

    void Update(){
        // Checks if is paused
        if(isFrozen) return;

        // Get input from WASD and mouse
        input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        // Calculate XZ rotation for turning and YZ for gliding
        transform.Rotate(Vector3.up * input.x * rotationStrength * Time.deltaTime, Space.World);
        ApplyYZRotation(input);

        // Calculate current speed based on tiered system, applies speed forward
        velMagnitude = CalculateCurrentVelocityMagnitude(input.y);
        if(isGliding) velMagnitude = CalculateGlidingAcceleration(velMagnitude);
        currentTier = GetCurrentTier(effectiveVelMagnitude, velMagnitude);
        effectiveVelMagnitude = CalculateEffectiveVelocityMagnitude(velMagnitude, currentTier);
        Vector3 newVelocity = GetForwardVector() * effectiveVelMagnitude;

        // Check if grounded, and keeping track of coyote time timer to allow for delayed jumps
        isGrounded = CheckGrounded();
        if(isGrounded){
            isJumping=false;
            isGliding=false;
            currentJumpCount = 0;
            currentCoyote = coyoteTime;
            glideTimer = 0;
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
            fallingVelocity = CalculateFallingVelocity();
        }

        // Check for gliding, apply updraft force
        if(!isGrounded && !isGliding && Input.GetButtonDown("Glide") && glideTimer < glideTimeLimit){
            isGliding = true;
            vfx.StartGlide();
            FreezeFrame();
        } else if(!Input.GetButton("Glide")){
            isGliding = false;
        }

        if(isGliding){
            glideTimer += Time.deltaTime;
            if(glideTimer > glideTimeLimit) isGliding = false;
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
        float max = maxSpeed;

        if(!isGrounded){
            acc = 0;
        } else if(input>0){
            acc = acceleration;
        } else if(input<0){
            dec = deceleration;
        } else dec = passiveDeceleration;
    
        float baseVel = Mathf.Clamp(velMagnitude+(acc*Time.deltaTime), minSpeed, max);
        float vel = Mathf.Max(baseVel, velMagnitude);
        vel -= (dec*Time.deltaTime);

        vel = Mathf.Min(vel, absoluteMaxSpeed);
        return vel;
    }
    
    private float CalculateEffectiveVelocityMagnitude(float _vel, int _tier){
        float maxVel = speedTiers[_tier];

        // return _vel;
        return Mathf.Min(maxVel, _vel);
    }

    private int GetCurrentTier(float lastEffective, float currentVel){
        if(!isGrounded) return currentTier;
        
        int a = 0;
        int b = 0;

        for(int i=0; i<speedTiers.Length; i++){
            if(speedTiers[i] < lastEffective) a = i+1;
            if(speedTiers[i] < currentVel) b = i;  
        }

        int tier = Mathf.Max(a, b);
        tier = Mathf.Clamp(tier,0,speedTiers.Length-1);

        if(currentTier != tier){
            StartCoroutine(TriggerTierUpdate(tier));
        }

        return tier;
    }

    private IEnumerator TriggerTierUpdate(int _tier){
        yield return new WaitForSeconds(0.1f);
        vfx.UpdateTier(_tier);
        cameraController.UpdateTier(_tier);

    }

    private void ApplyYZRotation(Vector2 input){
        Quaternion target = Quaternion.Euler(new Vector3(0,transform.localEulerAngles.y,0));
        float speed = yzReturnRotationSpeed;

        if(isGliding){
            if(input.y > 0){
                speed = yzDownRotationSpeed;
            } else speed = yzUpRotationSpeed;

            float xNorm = transform.eulerAngles.x + (yzRotationStrength * input.y * Time.deltaTime);
            float xAngle = ClampAngle(xNorm, -maxYZRotationAngle, maxYZRotationAngle);
            target = Quaternion.Euler(new Vector3(xAngle, transform.localEulerAngles.y, 0f));
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, target, speed * Time.deltaTime);
    }

    private float ClampAngle(float current, float min, float max){
        float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
        float hdtAngle = dtAngle * 0.5f;
        float midAngle = min + hdtAngle;
    
        float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
        if (offset > 0)
            current = Mathf.MoveTowardsAngle(current, midAngle, offset);
        return current;
    }

    private Vector3 GetForwardVector(){
        Vector3 forward = transform.forward;
        if(isGrounded){
            forward.y = 0f;
            return forward.normalized;
        }

        return forward;
    }

    private float CalculateFallingVelocity(){
        float verticalVelocity = 0;
        float gravFactor = 1;
        float max = maxFallSpeed;

        if(fallingVelocity<0) gravFactor = fallGravityMultiplier;
        
        if(isGliding){
            gravFactor = glideGravityFactor;
            max = glideMaxFallSpeed;
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


    private float CalculateGlidingAcceleration(float _velMag){
        float ang = transform.localEulerAngles.x;
        if(ang > 270) ang -= 360;

        float angleRatio = ang / maxYZRotationAngle;
        float unclamped = 0;

        // If player is diving, else if is rising
        if(angleRatio > 0){
            unclamped = _velMag + (angleRatio * glideAcceleration * Time.deltaTime);
        } else {
            unclamped = _velMag + (angleRatio * glideDeceleration * Time.deltaTime);
        }
        
        return Mathf.Clamp(unclamped, minSpeed, absoluteMaxSpeed);
    }

    private bool CheckGrounded(){
        Vector3 normal;
        string tag;
        bool grd = colUtil.IsGroundedCast(transform.position, out normal, out tag);
        currentNormal = normal;
        currentGroundTag = tag;

        return grd;
    }

    private float GetRampMultiplier(){
        float dot = Vector3.Dot(Vector3.up, currentNormal);
        
        if(dot < .95f && dot > .05f){
            return rampMagnitudeMultiplier;
        }

        return 1;
    }

    private Vector3 SnapToGround(Vector3 _mov){
        Vector3 mov = _mov;

        // Vector3 antiNormal = Vector3.Project(mov, currentNormal);
        // mov -= antiNormal;

        mov.y = Mathf.Max(0, mov.y);

        return mov;
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
