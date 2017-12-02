using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EnkiBye.Maths.Shapes
{
    using EnkiBye.Maths;

    [System.Serializable]
    public class Polygon
    {
        public edge[] segments;
        public Vector2[] points;

        public Vector2 barycenter
        {
            get
            {
                return points.getBarycenter();
            }
        }


        protected Polygon() { }

        public Polygon(Vector2[] _points)
        {
            _points = _points.reorderPoints();
            points = new Vector2[_points.Length];
            segments = new edge[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                points[i] = _points[i];
                if(i != _points.Length - 1)
                {
                    segments[i] = new edge(_points[i], _points[i + 1], this);
                }
                else
                {
                    segments[i] = new edge(_points[i], _points[0], this);
                }
            }
        }



        public float angleAtSegment(int segmentindex)
        {
            return MathsTools.SignedAngle(
                points[(int)Mathf.Repeat(segmentindex - 1, segments.Length)], 
                points[(int)Mathf.Repeat(segmentindex, segments.Length)], 
                points[(int)Mathf.Repeat(segmentindex + 1, segments.Length)]
                );
        }

        /// <summary>
        /// is the point contained inside of the polygon?
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool ContainPoint(Vector2 point)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].seePoint(point)) return false;
            }
            return true;
        }

        public virtual bool HasSegment(Segment segment, bool canBeOposite = true)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].isEqual(segment, canBeOposite)) return true;
            }
            return false;
        }

        public virtual bool isPointOnEdge(Vector2 point)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (point == points[i]) return true;
            }
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].containPoint(point)) return true;
            }
            return false;
        }

        public virtual void Draw()
        {
            #if !UNITY_EDITOR
                return; 
            #endif
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].Draw();
            }
        }

        /// <summary>
        /// is the point one of the extremity of the polygon
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool HasVertice(Vector2 point)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == point) return true;
            }
            return false;
        }

        public virtual int[] ShareVertice(Polygon otherPoly)
        {
            List<int> shardedPoints = new List<int>();
            for (int i = 0; i < points.Length; i++)
            {
                if (otherPoly.points.Contains(points[i])) shardedPoints.Add(i);
            }
            return shardedPoints.ToArray();
        }

        public virtual int[] ShareEdge(Polygon otherPoly)
        {
            int[] sharedVertice = ShareVertice(otherPoly);
            List<int> segmentsShared = new List<int>();

            if (sharedVertice.Length == 0) segmentsShared.ToArray();


            for (int i = 0; i < segments.Length; i++)
            {
                for (int j = 0; j < otherPoly.segments.Length; j++)
                {
                    if (segments[i].isEqual(otherPoly.segments[j])) segmentsShared.Add(i);
                }
            }
            return segmentsShared.ToArray();
        }

    }

    [System.Serializable]
    public class Triangle : Polygon
    {

        public Vector2 a;
        public Vector2 b;
        public Vector2 c;

        public edge ab;
        public edge bc;
        public edge ca;


        /// <summary>
        /// Get the hortocenter of a triangle, its the center of the circumscribed circle, that pass on all the 3 points of the triangle
        /// </summary>
        public Vector2 hortocenter
        {
            get
            {
                return points.getHortocenter();
            }
        }


        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            points = (new Vector2[3] { p1, p2, p3 }).reorderPoints();
            a = points[0];
            b = points[1];
            c = points[2];

            ab = new edge(a, b, this);
            bc = new edge(b, c, this);
            ca = new edge(c, a, this);

            segments = new edge[3] { ab, bc, ca };

        }//Triangle()

        public bool isinCircumscribed(Vector2 point)
        {
            return point.isInCircumscribedCircle(this);
        }

        /// <summary>
        /// if the two triangles share a segment, return the vector (i, j), where i is the index of the segment int the first triangle and j is the index in the second triangle
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Vector2 isAdjacent(Triangle other)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                for (int j = 0; j < other.segments.Length; j++)
                {
                    if (segments[i].isEqual(other.segments[j])) return new Vector2(i, j);
                }
            }
            return new Vector2(-1, -1);
        }

    }//Triangle



    [System.Serializable]
    public class Segment
    {
        public Vector2[] points;
        public Vector2 a;
        public Vector2 b;

        public Vector2 middle
        {
            get
            {
                return points.getMiddle();
            }
        }

        public Segment oposite
        {
            get
            {
                return new Segment(b, a);
            }
        }

        /// <summary>
        /// The normal of the segment, pointing to the exterior of the triangle
        /// </summary>
        public Vector2 normal
        {
            get
            {
                return MathsTools.normal(a, b);
            }
        }

        public Segment(Vector2 p1, Vector2 p2)
        {
            points = (new Vector2[2] { p1, p2 }).reorderPoints();
            a = points[0];
            b = points[1];
        }//Segment()

        public bool containPoint(Vector2 point)
        {
            if ((b-a).normalized == (point-a).normalized && (a-b).normalized == (point-b).normalized) return true;
            return false;
        }

        public bool isEqual(Segment other, bool canBeOposite = true)
        {
            return ((a == other.a && b == other.b) || (a == other.b && b == other.a && canBeOposite));
        }

        public virtual void Draw()
        {
            #if !UNITY_EDITOR
                return; 
            #endif
            Gizmos.DrawLine(a, b);
        }
        public virtual new string ToString()
        {
            return "{" + a + ", " + b + "}";
        }

    }//Segment

    /// <summary>
    /// the segment side of a triangle
    /// </summary>
    [System.Serializable]
    public class edge : Segment
    {
        [HideInInspector]
        public Polygon polygon;

        public new Vector2 normal
        {
            get
            {
                Vector2 _normal = MathsTools.normal(a, b);
                if (Vector2.Dot(_normal, middle - polygon.barycenter) < 0)
                    _normal *= -1;
                return _normal;
            }
        }//normal

        public edge(Vector2 p1, Vector2 p2, Polygon _triangle) : base(p1, p2)
        {
            polygon = _triangle;
        }//edge()

        public bool seePoint(Vector2 point)
        {
            return Vector2.Dot(normal, point - middle) > 0;
        }

        public override void Draw()
        {
            base.Draw();

            #if !UNITY_EDITOR
                return; 
            #endif

            Color c = Gizmos.color;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(middle, middle + normal);
            Gizmos.color = c;
        }
    }//edge 








    [System.Serializable]
    public class Triangulation
    {
        public List<Triangle> triangles;
        public List<Vector2> points;

        List<edge> edges;

        Polygon convexHull;
        private void recalculateHull()
        {
            convexHull = Jarvis.GetConvexHull(points);
        }
        public Polygon _convexHull
        {
            get
            {
                if (convexHull == null) recalculateHull();
                return convexHull;
            }
        }

        public Triangulation()
        {

        }
        public Triangulation(Vector2[] _points) { new Triangulation(_points.ToList()); }

        public Triangulation (List<Vector2> _points)
        {
            List<Vector2> tempPoints = new List<Vector2>(_points);
            points = new List<Vector2>();
            tempPoints.sortX();

            triangles = new List<Triangle>();
            triangles.Add(new Triangle(tempPoints[0], tempPoints[1], tempPoints[2]));
            points.Add(tempPoints[0]);
            points.Add(tempPoints[1]);
            points.Add(tempPoints[2]);

            recalculateHull();
            if(Application.isPlaying && false)
            {
                //PointsManager.get().StartCoroutine(addPointProcessDelayed(tempPoints));
            }
            else
            {
                for (int i = 3; i < tempPoints.Count; i++)
                {
                    addPoint(tempPoints[i]);
                }
            }



        }


        public IEnumerator addPointProcessDelayed(List<Vector2> tempPoints, bool wait = false)
        {

            for (int i = 3; i < tempPoints.Count; i++)
            {
                if (wait)
                {
                    while (!Input.GetMouseButtonUp(0)) yield return null;
                    yield return null; 
                }

                //yield return PointsManager.get().StartCoroutine(addPoint(tempPoints[i], true, true));
                addPoint(tempPoints[i], true, true);
                PointsManager.get().createMeshes();
            }
        }

        public void addPoint(Vector2 point, bool addToList = true, bool wait = false)
        {
            if(addToList)
                points.Add(point);

            edges = new List<edge>();


            if (_convexHull.ContainPoint(point))
            {
                for (int i = 0; i < triangles.Count; i++)
                {
                    if (triangles[i].ContainPoint(point))
                    {
                        for (int j = 0; j < triangles[i].segments.Length; j++)
                        {
                            edges.Add(triangles[i].segments[j]);
                        }
                        break;
                    }
                }//end for
            }
            else
            {
                for (int i = 0; i < _convexHull.segments.Length; i++)
                {
                    if (_convexHull.segments[i].seePoint(point)) edges.Add(_convexHull.segments[i]);
                }
            }



            int safelock = 0;
            while (edges.Count > 0)
            {
                edge current = edges[0];
                edges.RemoveAt(0);

                bool intriangle = false;
                for (int i = 0; i < triangles.Count; i++)
                {
                    if (!triangles[i].HasSegment(current)) continue;
                    if(triangles[i].isinCircumscribed(point) && !triangles[i].isPointOnEdge(point)) // TO DO ne pas add les triangles créé avant la fin de la fonction
                    {
                        intriangle = true;
                        for (int j = 0; j < triangles[i].segments.Length; j++)
                        {
                            if (!triangles[i].segments[j].isEqual(current)) edges.Add(triangles[i].segments[j]);
                        }
                        triangles.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                    }
                }

                if(!intriangle)
                    triangles.Add(new Triangle(current.a, current.b, point));

                if(safelock++ > 100)
                {
                    Debug.LogError("infinite loop");
                    break;
                }

                //if (wait)
                //{
                //    Debug.Log("number of edges in the list = " + edges.Count);
                //    PointsManager.get().createMeshes();
                //    while (!Input.GetMouseButtonUp(0)) yield return null;
                //    yield return null;
                //    
                //}
                recalculateHull();

            }

            
        }//addPoint


        public void Draw()
        {
            #if !UNITY_EDITOR
                return; 
            #endif
            if (triangles == null) return;
            for (int i = 0; i < triangles.Count; i++)
            {
                triangles[i].Draw();
            }
        }//Draw

    }//Triangulation

    [System.Serializable]
    public class Voronoi
    {
        public Triangulation delaunay;

        public List<Polygon> polygons = new List<Polygon>();

        private List<Triangle> trianglesTemp;

        public List<Vector2> points;

        public List<Segment> segments;

        public Voronoi(Triangulation triangulation)
        {
            secondVersion(triangulation);
            //return;

            delaunay = triangulation;
            trianglesTemp = new List<Triangle>(delaunay.triangles);
            points = new List<Vector2>();
            segments = new List<Segment>();

            bool[] freeEdge = new bool[3] { false, false, false };

            for (int i = 0; i < trianglesTemp.Count; i++)
            {
                freeEdge = new bool[3] { false, false, false };

                for (int j = 0; j < trianglesTemp.Count; j++)
                {
                    if (i == j) continue;

                    Vector2 adjacentReturn = trianglesTemp[i].isAdjacent(trianglesTemp[j]);
                    Segment S = new Segment(trianglesTemp[i].hortocenter, trianglesTemp[j].hortocenter);
                    if (adjacentReturn.x != -1) //the two triangles share a segment, so we link it
                    {
                        if (!hasSegment(S)) segments.Add(S);
                        freeEdge[(int)adjacentReturn.x] = true;
                    }
                }

                for (int j = 0; j < freeEdge.Length; j++)
                {
                    if (!freeEdge[j])
                    {
                        segments.Add(new Segment(trianglesTemp[i].hortocenter, trianglesTemp[i].segments[j].middle + trianglesTemp[i].segments[j].normal * 1000));
                        points.Add(trianglesTemp[i].segments[j].middle + trianglesTemp[i].segments[j].normal * 1000);
                    }
                }
            }

            for (int i = 0; i < delaunay.triangles.Count; i++)
            {
                points.Add(delaunay.triangles[i].hortocenter);
            }



        }//voronoi()


        public void secondVersion(Triangulation triangulation)
        {
            delaunay = triangulation;
            trianglesTemp = new List<Triangle>(delaunay.triangles);
            points = new List<Vector2>();
            segments = new List<Segment>();

            for (int i = 0; i < delaunay.points.Count; i++)
            {
                //pour chaque point de la triangulation, trouver quels points sont reliés à lui.
                Vector2 point = delaunay.points[i];
                List<Vector2> otherPoints = new List<Vector2>();
                List<Triangle> adjacentTriangles = new List<Triangle>();
                List<Vector2> polygonPoints = new List<Vector2>();

                for (int j = 0; j < delaunay.triangles.Count; j++) // on trouve les autres points
                {
                    if(delaunay.triangles[j].points.Contains(point))
                    {
                        Triangle currentTriangle = delaunay.triangles[j];
                        adjacentTriangles.Add(currentTriangle);
                        polygonPoints.Add(currentTriangle.hortocenter);
                        int[] exterEdges = currentTriangle.ShareEdge(delaunay._convexHull);
                        for (int e = 0; e < exterEdges.Length; e++)
                        {
                            if (currentTriangle.segments[exterEdges[e]].points.Contains(point)) //si un des edges externe du triangle contient le point actuel, on ajoutte un point externe loin
                                polygonPoints.Add(currentTriangle.hortocenter + currentTriangle.segments[exterEdges[e]].normal * 1000);
                        }
                    }
                }

                polygons.Add(new Polygon(polygonPoints.ToArray()));
            }


        }

        bool hasSegment(Segment S)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                if(segments[i].isEqual(S))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void Draw()
        {

            #if !UNITY_EDITOR
                return; 
            #endif
            //
            
            for (int i = 0; i < polygons.Count; i++)
            {
                polygons[i].Draw();
            }
        }


    }//Voronoï



}