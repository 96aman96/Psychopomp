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
    
    public CinemachineVirtualCamera[] nearCameras;
    public CinemachineVirtualCamera[] farCameras;
    public CinemachineVirtualCamera[] glideCameras;

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
    private int currentTier = 0;
    private Vector3 fwdVector = Vector3.zero;

    // Pause Unpause variables
    private bool isPaused = false;
    private float prevAmp = 0f;

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
        if(currentTier < _tier && _tier>1){
            SwitchToCamera(farCameras[_tier]);
            StartCoroutine(TimedSwitchToNear());
        }

        currentTier = _tier;
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
        SwitchToCamera(nearCameras[currentTier]);
    }

    private void StartGlide(){
        SwitchToCamera(glideCameras[currentTier]);
    }

    private void StopGlide(){
        SwitchToCamera(nearCameras[currentTier]);
    }

    private void UpdateGlidingCamera(){
        float yaxis = fwdVector.y;
        
        float freq = 0;
        float amp = 0;
        if(yaxis < 0){
            freq = glideNoiseFreqCurve.Evaluate(Mathf.Abs(yaxis));
            amp = glideNoiseAmpCurve.Evaluate(Mathf.Abs(yaxis));
        }

        UpdateCameraNoise(glideCameras[currentTier], freq, amp);
    }

    private void UpdateCameraNoise(CinemachineVirtualCamera _cam, float _freq, float _amp){
        if(isPaused){
            _freq = 0;
            _amp = 0;
        }

        _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = _freq;
        _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = _amp;

    }

    public void Pause(){
        isPaused = true;

        CinemachineBasicMultiChannelPerlin camNoise = current.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if(camNoise == null) return;

        prevAmp = camNoise.m_AmplitudeGain;
        camNoise.m_AmplitudeGain = 0f;
    }

    public void Unpause(){
        isPaused = false;

        CinemachineBasicMultiChannelPerlin camNoise = current.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if(camNoise == null) return;

        camNoise.m_AmplitudeGain = prevAmp;
    }
}
