using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum PlayerState
    {
        Idle,
        Climbing,
        Falling
    }

    public float moveSpeed, heightMax, heightMin, heightCurrent;
    public Camera freeLookCamera;
    public Animator anim;
    private Transform Player;

    private float xRotation, zRotation;

    private void Start()
    {
        Player = this.transform;
        heightCurrent = Player.position.y;
        anim = Player.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        xRotation = freeLookCamera.transform.rotation.eulerAngles.x;

        if (Input.GetKey(KeyCode.W))
        {
            MovePlayer();
        }
        else
        {
            StopMoving();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            boost();
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            anim.SetBool("didBoost", false);
        }
    }

    void MovePlayer()
    {
        Vector3 camForward = new Vector3(freeLookCamera.transform.forward.x, 0, freeLookCamera.transform.forward.z);
        transform.rotation = Quaternion.LookRotation(camForward);
        transform.Rotate(new Vector3(xRotation, 0, 0), Space.Self);

        anim.SetBool("isFlying", true);

        Vector3 forward = freeLookCamera.transform.forward;
        Vector3 flyDirection = forward.normalized;
        heightCurrent += flyDirection.y * moveSpeed * Time.deltaTime;
        heightCurrent = Mathf.Clamp(heightCurrent, heightMin, heightMax);

        transform.position += flyDirection * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, heightCurrent, transform.position.z);

    }

    void StopMoving()
    {
        anim.SetBool("isFlying", false);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void boost()
    {
        anim.SetBool("didBoost", true);


    }
}

