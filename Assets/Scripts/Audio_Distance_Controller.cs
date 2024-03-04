using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
public class Audio_Distance_Controller : MonoBehaviour
{
    public AudioSource radioStatic, targetMusic;
    public GameObject destinationPin;
    public float detectionRadius;
    public LayerMask DetectionMask;
    public float DistanceToTarget, MaxDistanceAllowed;
    public float proximity, distancePercent;
    public Vector3 FoundPos;
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
     //   normalizedDistance = Vector3.Distance(transform.position, destinationPin.transform.position);
     if (destinationPin == null)
     {
         LookForSphereTarget(TargetType.PickupTrain);
     }
     else
     {
         proximity = Vector3.Distance(transform.position,FoundPos) / DistanceToTarget;
         radioStatic.volume = 1 - proximity;
         targetMusic.volume = proximity;
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
                        destinationPin = hit.transform.parent.gameObject;
                        FoundPos = transform.position;
                        DistanceToTarget = Vector3.Distance(FoundPos, destinationPin.transform.position);
                        MaxDistanceAllowed = DistanceToTarget + 5;
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; // Set the color of the gizmo
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Draw the wire sphere
    }
}
