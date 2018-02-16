using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnkiBye.Maths;
using EnkiBye.Maths.Shapes;
using System.Linq;

public static class Jarvis
{

    public static Polygon GetConvexHull(Vector2[] _points)
    {
        return GetConvexHull(_points.ToList());
    }

    public static Polygon GetConvexHull(List<Vector2> _points)
    {
        List<Vector2> points = new List<Vector2>(_points);
        if (points.Count <= 1) return null;
        if (points.Count <= 3) return new Polygon(points.ToArray());

        points.sortX();

        List<Vector2> hullPoints = new List<Vector2>();
        hullPoints.Add(points[0]);
        //points.RemoveAt(0);

        Vector2 currentPoint = hullPoints[0];
        Vector2 previousPoint = currentPoint + Vector2.up;

        float lastAngle = 361;
        int index = -1;

        int safelock = 0;
        int initialNumberOFPoints = points.Count;
        do
        {
            
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] == currentPoint || points[i] == previousPoint) continue;
                float angle = MathsTools.SignedAngle(previousPoint, currentPoint, points[i]);
                if (angle < lastAngle)
                {
                    lastAngle = lastAngle = angle;
                    index = i;
                }
            }

            //if (points[index] == hullPoints[0]) break;
            hullPoints.Add(points[index]);
            previousPoint = currentPoint;
            currentPoint = points[index];
            points.RemoveAt(index);
            lastAngle = 361;

            if (safelock++ > initialNumberOFPoints)
            {
                Debug.Log("infinite loop");
                break;
            }

        } while (currentPoint != hullPoints[0]);



        return new Polygon(hullPoints.ToArray());
    }




}
