using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderUtil : MonoBehaviour{
    public CapsuleCollider capsule; 
    public LayerMask groundMask;

    public float skinWidth = 0.005f;
    public float groundDistance = 0.005f;
    public int maxBounces = 4;

    private float radius = 0;

    void Start(){
        if(capsule){
            radius = capsule.radius;
        }
    }

    public Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth, float magnitudeMult = 1){
        if(depth >= maxBounces) return Vector3.zero;

        float mag = vel.magnitude;
        float dist = mag + skinWidth;
        List<Vector3> points = GetPointsFromPosition(pos);

        RaycastHit hit;
        if(Physics.CapsuleCast(points[0], points[1], radius, vel.normalized, out hit, dist)){
            Vector3 velPostCollision = vel/mag * (hit.distance - skinWidth);
            Vector3 leftover = vel - velPostCollision;

            if(velPostCollision.magnitude <= skinWidth) velPostCollision = Vector3.zero;

            float leftoverMagnitude = leftover.magnitude * magnitudeMult;
            leftover = Vector3.ProjectOnPlane(leftover, hit.normal).normalized;
            leftover *= leftoverMagnitude;

            return velPostCollision + CollideAndSlide(leftover, pos + velPostCollision, depth+1, magnitudeMult);
        }

        return vel;
    }

    public bool IsGroundedCast(Vector3 pos, out Vector3 normal, out string tag){
        List<Vector3> points = GetPointsFromPosition(pos);
        RaycastHit hit;
        if(Physics.CapsuleCast(points[0], points[1], radius, Vector3.down, out hit, groundDistance, groundMask)){
            normal = hit.normal;
            tag = hit.transform.tag;
            return true;
        }

        normal = Vector3.up;
        tag = null;
        return false;
    }

    private List<Vector3> GetPointsFromPosition(Vector3 pos){
        List<Vector3> points = new List<Vector3>{
            (pos + capsule.center) + new Vector3(0, capsule.height / 2 - capsule.radius, 0),
            (pos + capsule.center) + new Vector3(0, -capsule.height / 2 + capsule.radius, 0)
        };
        return points;
    }
}
