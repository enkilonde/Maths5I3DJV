using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EnkiBye.Maths.Shapes;

namespace EnkiBye.Maths
{

    public static class MathsTools
    {
        /// <summary>
        /// Get the signed angle between three points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static float SignedAngle(Vector2 p1, Vector2 p2, Vector2 p3)
        {

            float ang = Vector2.SignedAngle(p1 - p2, p3 - p2);

            if (ang < 0)
                ang += 360;

            return ang;
        }//SignedAngle

        /// <summary>
        /// The area of the shape
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static float areaSize(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }//sign



        /// <summary>
        /// Get the middle point of two vectors
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <returns></returns>
        public static Vector3 getMiddle(Vector3 p1, Vector3 p2)
        {
            return p1 + (p2 - p1) / 2;
        }//getMiddle
        /// <summary>
        /// Get the middle point of two vectors
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <returns></returns>
        public static Vector2 getMiddle(Vector2 p1, Vector2 p2)
        {
            return p1 + (p2 - p1) / 2;
        }//getMiddle
        /// <summary>
        /// Get the middle point of two vectors
        /// </summary>
        /// <param name="points">array of points</param>
        /// <returns></returns>
        public static Vector2 getMiddle(this Vector2[] points)
        {
            if (points.Length < 2) return Vector2.zero;
            return getMiddle(points[0], points[1]);
        }//getMiddle


        /// <summary>
        /// Gets the coordinates of the intersection point of two lines.
        /// </summary>
        /// <param name="A1">A point on the first line.</param>
        /// <param name="A2">Another point on the first line.</param>
        /// <param name="B1">A point on the second line.</param>
        /// <param name="B2">Another point on the second line.</param>
        /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
        public static Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
        {
            bool osef;
            Vector2 output = GetIntersectionPointCoordinates(A1, A2, B1, B2, out osef);
            return output;
        }//GetIntersectionPointCoordinates
        /// <summary>
        /// Gets the coordinates of the intersection point of two lines.
        /// </summary>
        /// <param name="A1">A point on the first line.</param>
        /// <param name="A2">Another point on the first line.</param>
        /// <param name="B1">A point on the second line.</param>
        /// <param name="B2">Another point on the second line.</param>
        /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
        /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
        public static Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
        {
            float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

            if (tmp == 0)
            {
                // No solution!
                found = false;
                return Vector2.zero;
            }

            float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

            found = true;

            return new Vector2(
                B1.x + (B2.x - B1.x) * mu,
                B1.y + (B2.y - B1.y) * mu
            );
        }//GetIntersectionPointCoordinates



        /// <summary>
        /// Get the hortocenter of a triangle, its the center of the circumscribed circle, that pass on all the 3 points of the triangle
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <param name="p3">point 3</param>
        /// <returns></returns>
        public static Vector2 getHortocenter(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return GetIntersectionPointCoordinates(
                getMiddle(p1, p2), getMiddle(p1, p2) + normal(p1 - p2),
                getMiddle(p2, p3), getMiddle(p2, p3) + normal(p2 - p3));
        }//getHortocenter
        public static Vector2 getHortocenter(this Vector2[] points)
        {
            if (points.Length != 3) return Vector2.zero;
            return getHortocenter(points[0], points[1], points[2]);
        }//getHortocenter



        /// <summary>
        /// get the barycenter of the points
        /// </summary>
        /// <param name="points">the array of points</param>
        /// <returns></returns>
        public static Vector2 getBarycenter(this Vector2[] points)
        {
            Vector2 output = Vector2.zero;
            for (int i = 0; i < points.Length; i++)
            {
                output += points[i];
            }
            return output / points.Length;
        }//getBarycenter



        /// <summary>
        /// order the points in the trigonometrical order
        /// </summary>
        /// <param name="points">the point list</param>
        /// <returns></returns>
        public static Vector2[] reorderPoints(this Vector2[] points)
        {
            return points.reorderPoints(points.getBarycenter());
        }//reorderPoints
        /// <summary>
        /// order the points in the trigonometrical order
        /// </summary>
        /// <param name="points">the point list</param>
        /// <returns></returns>
        public static Vector2[] reorderPoints(this Vector2[] points, Vector2 center)
        {
            List<Vector2> listTemp = new List<Vector2>(points);
            Vector2 centerRight = center + Vector2.right;
            listTemp.Sort((a, b) => SignedAngle(centerRight, center, a).CompareTo(SignedAngle(centerRight, center, b)));
            //listTemp.Reverse();
            return listTemp.ToArray(); ;
        }//reorderPoints


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// give the normal of a vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 normal(this Vector2 vector)
        {
            return new Vector2(vector.y, -vector.x).normalized;
        }//normal
         /// <summary>
         /// get the normal of 
         /// </summary>
         /// <param name="point1"></param>
         /// <param name="point2"></param>
         /// <returns></returns>
        public static Vector2 normal(Vector2 point1, Vector2 point2)
        {
            return (point2 - point1).normal();
        }//normal


        /// <summary>
        /// check if the point is inside the triangle
        /// </summary>
        /// <param name="pt">Point to check</param>
        /// <param name="p1">first point of the triangle</param>
        /// <param name="p2">second point of the triangle</param>
        /// <param name="p3">third point of the triangle</param>
        /// <returns></returns>
        public static bool isInTriangle(this Vector2 pt, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            bool b1, b2, b3;

            b1 = areaSize(pt, p1, p2) < 0.0f;
            b2 = areaSize(pt, p2, p3) < 0.0f;
            b3 = areaSize(pt, p3, p1) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }//isInTriangle
         /// <summary>
         /// check if the point is inside the triangle
         /// </summary>
         /// <param name="pt">Point to check</param>
         /// <param name="triangle">the triangle</param>
         /// <returns></returns>
        public static bool isInTriangle(this Vector2 pt, Triangle triangle)
        {
            return pt.isInTriangle(triangle.a, triangle.b, triangle.c);
        }//isInTriangle
         /// <summary>
         /// check if the point is inside the triangle
         /// </summary>
         /// <param name="pt">Point to check</param>
         /// <param name="p1">first point of the triangle</param>
         /// <param name="p2">second point of the triangle</param>
         /// <param name="p3">third point of the triangle</param>
         /// <returns></returns>
        public static bool isInTriangle(this Vector3 pt, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return ((Vector2)pt).isInTriangle(p1, p2, p3);
        }//isInTriangle



        /// <summary>
        /// check if the point is inside the triangle
        /// </summary>
        /// <param name="pt">Point to check</param>
        /// <param name="v1">first point of the triangle</param>
        /// <param name="v2">second point of the triangle</param>
        /// <param name="v3">third point of the triangle</param>
        /// <returns></returns>
        public static bool isInCircumscribedCircle(this Vector2 pt, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 centerOfCircle = getHortocenter(p1, p2, p3);
            return (Vector2.Distance(centerOfCircle, pt) <= Vector2.Distance(centerOfCircle, p1));
        }//isInCircumscribedCircle
         /// <summary>
         /// check if the point is inside the triangle
         /// </summary>
         /// <param name="pt">Point to check</param>
         /// <param name="v1">the triangle</param>
         /// <returns></returns>
        public static bool isInCircumscribedCircle(this Vector2 pt, Triangle triangle)
        {
            return pt.isInCircumscribedCircle(triangle.a, triangle.b, triangle.c);
        }//isInCircumscribedCircle
        /// <summary>
        /// check if the point is inside the triangle
        /// </summary>
        /// <param name="pt">Point to check</param>
        /// <param name="v1">first point of the triangle</param>
        /// <param name="v2">second point of the triangle</param>
        /// <param name="v3">third point of the triangle</param>
        /// <returns></returns>
        public static bool isInCircumscribedCircle(this Vector3 pt, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return pt.isInCircumscribedCircle(p1, p2, p3);
        }//isInCircumscribedCircle


    }//MathsTools

}

