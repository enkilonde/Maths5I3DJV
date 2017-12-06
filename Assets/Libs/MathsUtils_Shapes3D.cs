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

        public Polygon3D(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            setFaces(v1, v2, v3, v4);
        }
        public Polygon3D(Point p1, Point p2, Point p3, Point p4)
        {
            setFaces(p1, p2, p3, p4);
        }
        public Polygon3D(Face[] _faces)
        {
            setFaces(_faces);
        }


        private void setFaces(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            setFaces(new Point(v1), new Point(v2), new Point(v3), new Point(v4));
        }
        private void setFaces(Point p1, Point p2, Point p3, Point p4)
        {
            setFaces(new Face[] { new Face(p1, p2, p3), new Face(p2, p3, p4), new Face(p3, p4, p1), new Face(p4, p1, p2) });
        }
        private void setFaces(Face[] _faces)
        {
            faces = _faces;
            checkFacesOrientation();
        }

        public void addPoint(Vector3 point)
        {
            List<Face> facesToReplace = new List<Face>();
            List<Face> newFaces = new List<Face>();
            for (int i = 0; i < faces.Length; i++)
            {
                if (faces[i].seePoint(point)) facesToReplace.Add(faces[i]);
                else newFaces.Add(faces[i]);
            }

            for (int i = 0; i < facesToReplace.Count; i++)
            {
                for (int a = 0; a < 3; a++) // on browse les segments
                {
                    Point p1 = facesToReplace[i][a];
                    Point p2 = facesToReplace[i][(int)Mathf.Repeat(a+1, 3)];
                    bool internSegment = false;
                    for (int j = 0; j < facesToReplace.Count; j++)
                    {
                        if (i == j) continue;
                        if (facesToReplace[j].containPoints(p1, p2))
                            internSegment = true;
                    }//for j
                    if (internSegment) continue; // si c'est un segment intern, on le skip
                    newFaces.Add(new Face(p1, p2, new Point(point)));
                } // for a
            } // for i
            faces = newFaces.ToArray();
            checkFacesOrientation();
        }//addPoint

        
        public void checkFacesOrientation()
        {
            //flip the faces to make sure no one is looking at the barycenter (the polygon is convex)
            Vector3 bary = barycenter;
            for (int i = 0; i < faces.Length; i++)
            {
                if (faces[i].seePoint(bary)) faces[i].flip();
                if (faces[i].seePoint(bary)) Debug.LogError("NOPE");
            }

            //Debug.Log("number of faces = " + faces.Length);
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
            return Vector3.Dot(point - a, normal) > 0;
        }


        public bool containPoints(Point p1, Point p2)
        {
            int numberOfCorrespondances = 0;
            for (int i = 0; i < 3; i++)
            {
                if (points[i] == p1 || points[i] == p2) numberOfCorrespondances++;
            }
            return numberOfCorrespondances == 2;
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

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.position == p2.position;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p1.position != p2.position;
        }


    }//Point






}


