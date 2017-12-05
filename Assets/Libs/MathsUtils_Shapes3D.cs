using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EnkiBye.Maths.Shapes
{
    public class Polygon3D
    {
        public Face[] faces;

        public Vector3 barycenter
        {
            get
            {
                Vector3 bary = Vector3.zero;
                for (int i = 0; i < faces.Length; i++)
                {
                    bary += faces[i].barycenter / faces.Length;
                }
                return bary;
            }
        }

        public Polygon3D(Face[] _faces)
        {
            faces = _faces;
            //flip the faces to make sure no one is looking at the barycenter (the polygon is convex)
            Vector3 bary = barycenter;
            for (int i = 0; i < faces.Length; i++)
            {
                if (faces[i].seePoint(bary)) faces[i].flip();
                if (faces[i].seePoint(bary)) Debug.LogError("NOPE");
            }

        }

        
    }//Polygon3D



    public class Face
    {
        public Point[] points;//has 3 points
        public Point a
        {
            get { return points[0]; }
            set { points[0] = value; }
        }
        public Point b
        {
            get { return points[1]; }
            set { points[1] = value; }
        }
        public Point c
        {
            get { return points[2]; }
            set { points[2] = value; }
        }
        public Point this[int i]
        {
            get
            {
                return points[i];
            }
            set
            {
                points[i] = value;
            }
        }


        public Vector3 barycenter
        {
            get
            {
                Vector3 bary = Vector3.zero;
                for (int i = 0; i < points.Length; i++)
                {
                    bary += points[i] / points.Length;
                }
                return bary;
            }
        }

        public Vector3 normal
        {
            get
            {
                return Vector3.Cross(b - a, c - a).normalized;
            }
        }


        //constructors
        public Face(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            points = new Point[3] { new Point(p1), new Point(p2), new Point(p3) };
        }
        public Face(Point p1, Point p2, Point p3)
        {
            points = new Point[3] { p1, p2, p3 };
        }


        //functions
        public void flip()
        {
            Point temp = b;
            b = c;
            c = temp;
        }

        public bool seePoint(Vector3 point)
        {
            return Vector3.Dot(point-a, normal) > 0;
        }

    }//Face


    [System.Serializable]
    public class Point
    {
        public Vector3 position;

        public Point(Vector3 _position)
        {
            position = _position;
        }


        public static implicit operator Vector3(Point p) { return p.position;} // implicit cast to vector3

        public static Vector3 operator /(Point p, float div)
        {
            return p.position / div;
        }

        public static Vector3 operator *(Point p, float mult)
        {
            return p.position * mult;
        }

        public static Vector3 operator -(Point p1, Point p2)
        {
            return p1.position - p2.position;
        }

    }//Point


}


