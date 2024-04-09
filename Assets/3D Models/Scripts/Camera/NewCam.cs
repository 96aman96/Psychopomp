using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCam : MonoBehaviour{
    public Transform nearFollow;
    public Transform farFollow;
    public Transform lookAt;

    public float moveSpeed = 10f;
    public float lookSpeed = 5f;

    private Transform currentFollow;

    private void Start(){
        currentFollow = nearFollow;
    }

    private void Update(){
        transform.position = Vector3.Lerp(transform.position, currentFollow.transform.position, moveSpeed * Time.deltaTime);
        
        transform.LookAt(lookAt);
        // Vector3 lookdir = lookAt.position - transform.position;
        // Quaternion rot = Quaternion.LookRotation(lookdir, Vector3.up);
        // transform.rotation = Quaternion.Lerp(transform.rotation, rot, lookSpeed * Time.deltaTime);

    }

    private void GoNear(){
        currentFollow = nearFollow;
    }

    private void GoFar(){
        currentFollow = farFollow;
    }
    
}
