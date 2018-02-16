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
        public List<Vector3> points = new List<Vector3>();

        public bool planar = true;

        public Polygon3D(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            setFaces(v1, v2, v3);
        }
        public Polygon3D(Point p1, Point p2, Point p3)
        {
            setFaces(p1, p2, p3);
        }
        public Polygon3D(List<Vector3> _points)
        {
            if (_points.Count < 3)
            {
                Debug.LogError("not enought points to create a polygon");
                return;
            }

            faces = new Face[1] { new Face(_points[0], _points[1], _points[2]) };

            points.Add(_points[0]);
            points.Add(_points[1]);
            points.Add(_points[2]);

            for (int i = 3; i < _points.Count; i++)
            {
                //if (i == 4) return;

                addPoint(_points[i]);
            }

        }

        private void setFaces(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            setFaces(new Point(v1), new Point(v2), new Point(v3));
        }
        private void setFaces(Point p1, Point p2, Point p3)
        {
            setFaces(new Face[] { new Face(p1, p2, p3) });
        }
        private void setFaces(Face[] _faces)
        {
            for (int i = 0; i < _faces.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!points.Contains(_faces[i][j])) points.Add(_faces[i][j]);
                }
            }
            faces = _faces;
            checkFacesOrientation();
        }

        public void addPoint(Vector3 point)
        {
            if (!points.Contains(point))
                points.Add(point);

            if(planar && !faces[0].isCoplanar(point))
            {
                faces[0].flip();
                checkFacesOrientation();
            }

            List<Face> facesToReplace = new List<Face>();
            List<Face> newFaces = new List<Face>();
            if(faces.Length == 1)
            {
                facesToReplace.Add(faces[0]);
                newFaces.Add(faces[0]);
            }
            else
            {
                for (int i = 0; i < faces.Length; i++)
                {
                    if(planar)
                    {
                        facesToReplace.Add(faces[i]);
                        newFaces.Add(faces[i]);
                        continue;
                    }

                    if (faces[i].seePoint(point))
                        facesToReplace.Add(faces[i]);
                    else
                        newFaces.Add(faces[i]);
                }
            }


            for (int i = 0; i < facesToReplace.Count; i++)
            {
                if (facesToReplace[i].isCoplanar(point)) // if it is coplanar
                {
                    List<Point[]> linked = facesToReplace[i].GetLinkedPoints(new Point(point));
                    //if (faces.Length != 1 && linked.Count != 3) newFaces.Add(facesToReplace[i]);
                    bool skip = false;
                    for (int j = 0; j < linked.Count; j++)
                    {
                        for (int k = 0; k < faces.Length; k++)
                        {
                            if (faces[k] != facesToReplace[i]) //Si le point est coplanaire avec une autre face et que cette face a aussi les deux points
                            {
                                if (faces[k].isCoplanar(point))
                                {
                                    if (faces[k].containPoints(linked[j][0], linked[j][1]))
                                    {
                                        skip = true;
                                        break;
                                    }
                                }
                            }
                        }//for  
                        if (skip)
                        {
                            skip = false;
                            continue;
                        }
                        newFaces.Add(new Face(linked[j][0], linked[j][1], point));
                    }
                    continue;
                } // if coplanar
                planar = false;
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
            //Debug.Log("number of faces = " + faces.Length);

            if (planar) // Si le mesh est un plan, 
            {
                for (int i = 0; i < faces.Length; i++)
                {
                    if (Vector3.Dot(faces[i].normal, faces[0].normal) < 1 - Face.coplanarTolerance)
                        faces[i].flip();
                }
                return;
            }


            //flip the faces to make sure no one is looking at the barycenter (the polygon is convex)
            Vector3 bary = barycenter;
            if(faces.Length == 0)
            {
                //Debug.LogError("fuck");
                return;
            }

            Vector3 normal = faces[0].normal;

            for (int i = 0; i < faces.Length; i++)
            {
                if (faces[i].seePoint(bary))
                {
                    faces[i].flip();
                    if (faces[i].seePoint(bary))
                    {
                        Debug.LogError("NOPE : bary = " + bary);
                    }
                }
                    


                //if (i != 0 && faces[i].isCoplanar(faces[0]) && faces[i].normal == -normal)
                //    faces[i].flip();

            }
            

            //Debug.Log("number of faces = " + faces.Length);
        }


        public void Draw()
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < faces.Length; i++)
            {
                faces[i].Draw();
            }

            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.DrawSphere(points[i], 0.025f);
            }
        }

    }//Polygon3D



    public class Face
    {
        public const float coplanarTolerance = 0.001f;

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
        public Plane plane;

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
            plane = new Plane(p1, p2, p3);
            if (plane.normal != normal) plane.Flip();
        }
        public Face(Point p1, Point p2, Point p3)
        {
            points = new Point[3] { p1, p2, p3 };
            plane = new Plane(p1, p2, p3);
            if (plane.normal != normal) plane.Flip();
        }


        //functions
        public void flip()
        {
            Point temp = b;
            b = c;
            c = temp;
            if (plane.normal != normal) plane.Flip();
        }

        public bool seePoint(Vector3 point, bool coplanar = false)
        {
            float dot = Vector3.Dot((point - a).normalized, normal);
            float dist = plane.GetDistanceToPoint(point);
            if (coplanar) return plane.GetDistanceToPoint(point) > -coplanarTolerance;
            return plane.GetDistanceToPoint(point) > coplanarTolerance;


        }

        public bool isCoplanar(Vector3 point)
        {
            return  Mathf.Abs(plane.GetDistanceToPoint(point)) < coplanarTolerance;
            //float dot = Vector3.Dot(point - a, normal);
            //return dot <= coplanarTolerance && dot >= -coplanarTolerance;
        }

        public bool isCoplanar(Face other, bool sameOrientation = false)
        {
            // si les deux normals ne sont pas identiques, ils ne sont pas coplanar
            if (Vector3.Dot(plane.normal, other.plane.normal) > 1 - coplanarTolerance) return true;
            if (sameOrientation && Vector3.Dot(plane.normal, other.plane.normal) > -1 + coplanarTolerance) return true;

            if (plane.GetDistanceToPoint(other.a) == 0) return true;
            return false;
        }

        /// <summary>
        /// only use this function if the point and the plane are coplanar
        /// </summary>
        public List<Point[]> GetLinkedPoints(Point point)
        {

            List<Point[]> linked = new List<Point[]>();

            Vector3 cross_ab_cb = Vector3.Cross(b - a, c - b).normalized;
            Vector3 cross_ab_bP = Vector3.Cross(b - a, point - b).normalized;

            Vector3 cross_bc_ca = Vector3.Cross(c - b, a - c).normalized;
            Vector3 cross_bc_cP = Vector3.Cross(c - b, point - c).normalized;

            Vector3 cross_ca_ab = Vector3.Cross(a - c, b - a).normalized;
            Vector3 cross_ca_aP = Vector3.Cross(a - c, point - a).normalized;

            float dot1 = Vector3.Dot(cross_ab_cb, cross_ab_bP);
            float dot2 = Vector3.Dot(cross_bc_ca, cross_bc_cP);
            float dot3 = Vector3.Dot(cross_ca_ab, cross_ca_aP);

            if (dot1 < 1 - coplanarTolerance  
                && !Point.aligned(a, b, point))
                linked.Add(new Point[2] { a, b });
            if (dot2 < 1 - coplanarTolerance
                && !Point.aligned(b, c, point))
                linked.Add(new Point[2] { b, c });
            if (dot3 < 1 - coplanarTolerance
                && !Point.aligned(c, a, point))
                linked.Add(new Point[2] { c, a });

            if (linked.Count == 0)
            {
                linked.Add(new Point[2] { a, b });
                linked.Add(new Point[2] { b, c });
                linked.Add(new Point[2] { c, a });
            }

            return linked;
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
        
        public void addToList(List<Vector3> list)
        {
            list.Add(a);
            list.Add(b);
            list.Add(c);
        }

        public void Draw()
        {
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, a);
            //a.Draw();
            //b.Draw();
            //c.Draw();
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

        public static bool aligned(Point a, Point b, Point c)
        {
            float dot = Vector3.Dot((b-a).normalized, (c-b).normalized);
            return Mathf.Approximately(dot, 1) || Mathf.Approximately(dot, -1);
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

        public static Vector3 operator +(Point p1, Point p2)
        {
            return p1.position + p2.position;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.position == p2.position;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p1.position != p2.position;
        }

        public void Draw()
        {
            Gizmos.DrawSphere(position, 0.05f);
        }

    }//Point



}


