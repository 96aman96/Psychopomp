using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour{
    // ======= CAMERA REFS ======
    [Header("Cameras")]
    public CinemachineVirtualCamera[] cameras;
    
    public CinemachineVirtualCamera nearCamera;
    public CinemachineVirtualCamera farCamera;
    public CinemachineVirtualCamera glideCamera;

    public CinemachineVirtualCamera initialCamera;
    private CinemachineVirtualCamera current;

    // ======= CAMERA VARIABLES ======
    [Header("Camera Variables")]
    public float switchToNearTimer = 1f;

    // ======= KCC VARIABLES ======
    [Header("Components")]
    public KinematicCharacterController kcc;
    private bool isGliding = false;
    private bool isGrounded = false;
    private float speed = 0;
    private int currenTier = 0;

    void Start(){
        current = initialCamera;
        ActivateCamera(current);
    }

    void LateUpdate(){
        UpdateVariables();
    }

    public void UpdateTier(int _tier){
        if(currenTier < _tier){
            SwitchToCamera(farCamera);
            StartCoroutine(TimedSwitchToNear());
        }

        currenTier = _tier;
    }

    public void SwitchToCamera(CinemachineVirtualCamera cam){
        if(cam == current) return;

        current = cam;
        ActivateCamera(current);
    }

    private void ActivateCamera(CinemachineVirtualCamera cam){
        for (int i = 0; i < cameras.Length; i++){
            if (cameras[i] == cam){
                cameras[i].Priority = 20;
            } else cameras[i].Priority = 10;
        }
    }

    private void UpdateVariables(){
        isGrounded = kcc.GetIsGrounded();
        speed = kcc.GetSpeed();

        bool newGlide = kcc.GetIsGliding();
        if(newGlide != isGliding){
            if(!isGliding){
                StartGlide();
            } else StopGlide();
        }
        
        isGliding = newGlide;
    }

    private IEnumerator TimedSwitchToNear(){
        yield return new WaitForSeconds(switchToNearTimer);
        SwitchToCamera(nearCamera);
    }

    private void StartGlide(){
        SwitchToCamera(glideCamera);
    }

    private void StopGlide(){
        SwitchToCamera(nearCamera);
    }
}
