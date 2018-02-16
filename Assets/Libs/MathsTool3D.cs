using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnkiBye.Maths.Shapes;


namespace EnkiBye.Maths
{

    public static class MathsTool3D
    {


        public static Polygon3D getConvexHull(List<Vector3> points)
        {
            return new Polygon3D(points);
        }


        public static Vector3 projectPointOnPlane(Ray ray, Plane plane, out bool projectionFailed)
        {
            float dist = 0;
            if (!plane.Raycast(ray, out dist))
            {
                ray.direction *= -1;
                if (!plane.Raycast(ray, out dist))
                {
                    Debug.DrawRay(ray.origin, ray.direction, Color.white);
                    projectionFailed = true;
                    return Vector3.zero;
                }
            }
            projectionFailed = false;
            Vector3 result = ray.origin + ray.direction.normalized * dist;
            return result;
        }




        public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Quaternion pointRotation)
        {
            //To rotate an object around an arbitrary point, translate first (the arbitrary offset) then rotate as opposed to rotate-scale - translate

            Vector3 offset = point - pivot;
            //Vector3 rotationOffset = pivotRotation.eulerAngles - pointRotation.eulerAngles;
            offset = pointRotation * offset;
            return offset + pivot; // calculate rotated point
        }

    }

}