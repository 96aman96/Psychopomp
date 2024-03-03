using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderUtil : MonoBehaviour{
    public CapsuleCollider capsule; 
    public LayerMask[] groundMask;

    public float xzSkinWidth = 0.015f;
    public float ySkinWidth = 0.015f;
    public float groundDistance = 0.05f;
    public int maxBounces = 4;

    private float radius = 0;

    void Start(){
        if(capsule){
            radius = capsule.radius;
        }
    }

    public Vector3 YCollideAndSlide(Vector3 vel, Vector3 pos){
        float mag = vel.magnitude;
        float dist = mag + ySkinWidth;
        List<Vector3> points = GetPointsFromPosition(pos);

        RaycastHit hit;
        if(Physics.CapsuleCast(points[0], points[1], radius, vel.normalized, out hit, dist)){
            Vector3 velPostCollision = vel/mag * (hit.distance - ySkinWidth);
            Vector3 leftover = vel - velPostCollision;

            if(velPostCollision.magnitude <= ySkinWidth) velPostCollision = Vector3.zero;

            float leftoverMagnitude = leftover.magnitude;
            leftover = Vector3.ProjectOnPlane(leftover, hit.normal).normalized;
            leftover *= leftoverMagnitude;

            return velPostCollision;
        }

        return vel;
    }

    public Vector3 XZCollideAndSlide(Vector3 vel, Vector3 pos, int depth){
        if(depth >= maxBounces) return Vector3.zero;

        float mag = vel.magnitude;
        float dist = mag + xzSkinWidth;
        List<Vector3> points = GetPointsFromPosition(pos);

        RaycastHit hit;
        if(Physics.CapsuleCast(points[0], points[1], radius, vel.normalized, out hit, dist)){
            Vector3 velPostCollision = vel/mag * (hit.distance - xzSkinWidth);
            Vector3 leftover = vel - velPostCollision;

            if(velPostCollision.magnitude <= xzSkinWidth) velPostCollision = Vector3.zero;

            float leftoverMagnitude = leftover.magnitude;
            leftover = Vector3.ProjectOnPlane(leftover, hit.normal).normalized;
            leftover *= leftoverMagnitude;

            return velPostCollision + XZCollideAndSlide(leftover, pos + velPostCollision, depth+1);
        }

        return vel;
    }

    public bool IsGroundedCast(Vector3 pos){
        List<Vector3> points = GetPointsFromPosition(pos);
        return Physics.CapsuleCast(points[0], points[1], radius, -capsule.transform.up, groundDistance);
    }

    private List<Vector3> GetPointsFromPosition(Vector3 pos){
        List<Vector3> points = new List<Vector3>{
            (pos + capsule.center) + new Vector3(0, capsule.height / 2 - capsule.radius, 0),
            (pos + capsule.center) + new Vector3(0, -capsule.height / 2 + capsule.radius, 0)
        };
        return points;
    }
}
