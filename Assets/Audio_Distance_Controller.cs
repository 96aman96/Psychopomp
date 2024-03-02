using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Distance_Controller : MonoBehaviour
{
    public AudioSource radioStatic, targetMusic;
    public GameObject destinationPin;
    public float normalizedDistance;
    void Start()
    {
        radioStatic.volume = 0;
        targetMusic.volume = 0;
        radioStatic.Play();
        targetMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        normalizedDistance = Vector3.Distance(transform.position, destinationPin.transform.position);
    }

    void LookForSphereTarget(TargetType type)
    {
        RaycastHit hit;
        if (type == TargetType.PickupTrain)
        {
            Physics.SphereCast(transform.position,detect2ionRadius,transform,out hit,)
        }
    }
}
