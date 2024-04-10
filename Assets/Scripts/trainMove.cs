using System;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed;
    public bool canMove = false;
    void Update()
    {
        if (canMove == true)
        {
            // Move the object forward based on the speed and deltaTime
            transform.Translate(new Vector3(-1,0,0) * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Track")
        {
            other.transform.GetComponent<Animator>().Play("Spawn");
        }
    }
}