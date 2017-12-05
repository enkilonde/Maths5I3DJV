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
            


            Polygon3D poly = new Polygon3D(points[0], points[1], points[2], points[3]);
            for (int i = 4; i < points.Count; i++)
            {
                poly.addPoint(points[i]);
            }

            return poly;
        }



    }

}