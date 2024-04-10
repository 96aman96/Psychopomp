using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
public class Audio_Distance_Controller : MonoBehaviour
{
    public AudioSource radioStatic, targetMusic;
    public GameObject destinationPin;
    public float detectionRadius;
    public float DistanceToTarget, DistanceWhenFound;
    public Vector3 FoundPos;
    public float noiseVol,sigVol,proximity;
    public bool canUpdateDistance;
    public Transform TargetStub;
    void Start()
    {
        radioStatic.volume = 0;
        targetMusic.volume = 0;
        radioStatic.Play();
        targetMusic.Play();
        if (detectionRadius == 0)
        {
            destinationPin = TargetStub.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
     //   normalizedDistance = Vector3.Distance(transform.position, destinationPin.transform.position);
     if (destinationPin == null)
     {
         LookForSphereTarget(TargetType.PickupTrain);
     }
     else
     {
         CheckIfLost();
         CheckTargetAngle();
         UpdateDistanceToTarget();
         SetVolumeForSignal();
         // radioStatic.volume = noiseVol;
         // proximity = Vector3.Distance(transform.position,FoundPos) / DistanceToTarget;
         // radioStatic.volume = 1 - proximity;
         // targetMusic.volume = proximity;
     }
        
    }

    private void CheckIfLost()
    {
        if (DistanceToTarget > DistanceWhenFound)
        {   
            destinationPin = null;
            sigVol = 0;
            noiseVol = 0;
        }
}

    private void UpdateDistanceToTarget()
    {
        if (destinationPin!=null && canUpdateDistance)
        {
            DistanceToTarget = Vector3.Distance(transform.position, destinationPin.transform.position);
        }
    }

    void LookForSphereTarget(TargetType type)
    {
        if (type == TargetType.PickupTrain)
        {
            // Cast a sphere around the player's position
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionRadius, Vector3.up, 0);
            // Using Vector3.up as a dummy direction and 0 as maxDistance


            foreach (var hit in hits)
            {
                if (hit.collider != null) // Check if the hit has a collider
                {
                    if (hit.collider.tag == "ObjectiveTrigger")
                    {
                        destinationPin = hit.transform.gameObject;
                        FoundPos = transform.position;
                        DistanceToTarget = Vector3.Distance(FoundPos, destinationPin.transform.position);
                        DistanceWhenFound = DistanceToTarget;
                    }
                }
            }
        }
    }

    void CheckTargetAngle()
    {
        if (destinationPin != null && (DistanceToTarget > 20 || detectionRadius == 0))
        {
            canUpdateDistance = true;
            Vector3 directionToSphere = destinationPin.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, directionToSphere);
            float noiseVolume = angle / 180.0f;
            noiseVol = noiseVolume;
        }
        else
        {
            noiseVol = 0;
        }

        radioStatic.volume = noiseVol;

    }

    void SetVolumeForSignal()
    {
        if (DistanceToTarget < detectionRadius || detectionRadius ==0)
        {
            proximity = Mathf.Abs(1-(DistanceToTarget / DistanceWhenFound));
        }

        targetMusic.volume = Mathf.Clamp(proximity,0.05f,1.0f);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; // Set the color of the gizmo
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Draw the wire sphere
    }
}

