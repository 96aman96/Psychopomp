using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CamMovement : MonoBehaviour{
    public Transform target;

    public float camSensitivityX = 1f;
    public float camSensitivityY = 1f;
    public Vector2 camClampX = new Vector2(-50,50);
    public Vector2 camClampY = new Vector2(-60,60);
    private Vector2 rotation = Vector2.zero;
    private Vector3 current = Vector3.zero;

    public float smoothDampTimeRotation = 0.1f;
    private Vector3 smoothDampVelocity = Vector3.zero;

    public float camSensitivityZoom = 1f;
    public float camDistance = 5f;
    public Vector2 camDistanceRange = new Vector2(1,10);

    
    void Update(){
        if(Input.GetMouseButton(1)){
            // Getting mouse input to rotate camera
            float horizontal = Input.GetAxis("Mouse X") * camSensitivityX;
            float vertical = Input.GetAxis("Mouse Y") * -camSensitivityY;
            
            // Adding rotation and clamping so camera doesn't move too far vertically
            rotation += new Vector2(vertical, horizontal);
            rotation.x = Mathf.Clamp(rotation.x, camClampX.x, camClampX.y);
            // rotation.y = Mathf.Clamp(rotation.y, camClampY.x, camClampY.y);
        }
        
        // Smoothing the movement for good feel
        current = Vector3.SmoothDamp(current, new Vector3(rotation.x, rotation.y, 0), ref smoothDampVelocity, smoothDampTimeRotation);
        transform.localEulerAngles = current;

        // Make the camera look at the character
        // float distance = camDistance + (Input.mouseScrollDelta.y * camSensitivityZoom);
        // distance = Mathf.Clamp(distance, camDistanceRange.x, camDistanceRange.y);
        // camDistance = Mathf.SmoothDamp(camDistance, distance, ref smoothDampVelocityZoom, smoothDampTimeZoom);
        camDistance-=Input.mouseScrollDelta.y * camSensitivityZoom;
        transform.position = target.position - transform.forward * camDistance;

    }
}
