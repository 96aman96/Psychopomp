using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using Unity.Mathematics;

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
    public AnimationCurve glideNoiseFreqCurve;
    public AnimationCurve glideNoiseAmpCurve;

    // ======= KCC VARIABLES ======
    [Header("Components")]
    public KinematicCharacterController kcc;
    private bool isGliding = false;
    private bool isGrounded = false;
    private float speed = 0;
    private int currenTier = 0;
    private Vector3 fwdVector = Vector3.zero;

    void Start(){
        current = initialCamera;
        ActivateCamera(current);
    }

    void LateUpdate(){
        UpdateVariables();
        if(isGliding){
            UpdateGlidingCamera();
        }
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
        fwdVector = kcc.GetCurrentForwardVector();

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

    private void UpdateGlidingCamera(){
        float yaxis = fwdVector.y;
        
        float freq = 0;
        float amp = 0;
        if(yaxis < 0){
            freq = glideNoiseFreqCurve.Evaluate(Mathf.Abs(yaxis));
            amp = glideNoiseAmpCurve.Evaluate(Mathf.Abs(yaxis));
        }

        UpdateCameraNoise(glideCamera, freq, amp);
    }

    private void UpdateCameraNoise(CinemachineVirtualCamera _cam, float _freq, float _amp){
        _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = _freq;
        _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = _amp;

    }
}
