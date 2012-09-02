//Copyright (c) 2011 Evan Wallace (http://madebyevan.com/)
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

//Copyright (c) 2012 Barnaby Smith (http://mvinetwork.co.uk)
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using Microsoft.Xna.Framework;

namespace OLDE
{
    public class Plane
    {
        const float EPSILON = 1e-5f;

        Vector3 normal;
        float w;

        private Plane(Vector3 normal, float w)
        {
            this.normal = normal;
            this.w = w;
        }
        public static Plane FromPoints(Vector3 a, Vector3 b, Vector3 c)
        {
            //Vector3 n = (b - Vector3.Cross(a, c - a)).normalized;
            Vector3 n = Vector3.Cross(b - a, c - a);
            // TODO: Check that Normalize() behaves the same in Unity/XNA
            n.Normalize();

            //Debug.Log("fromPoints " + n.X + " " + n.Y + " " + n.Z);
            return new Plane(n, Vector3.Dot(n, a));
        }

        public Plane Clone()
        {
            return new Plane(normal, w);
        }

        public void Flip()
        {
            this.normal = -this.normal;
            this.w = -this.w;
        }

        // Split `polygon` by this plane if needed, then put the polygon or polygon
        // fragments in the appropriate lists. Coplanar polygons go into either
        // `coplanarFront` or `coplanarBack` depending on their orientation with
        // respect to this plane. Polygons in front or in back of this plane go into
        // either `front` or `back`.
        static int SPCounter = 0;
        public void SplitPolygon(Polygon polygon, ref List<Polygon> coplanarFront, ref List<Polygon> coplanarBack, ref List<Polygon> front, ref List<Polygon> back)
        {
//            console.log("SP" + SPCounter);
//            console.log(polygon.Vertices.Length);
            const int COPLANAR = 0;
            const int FRONT = 1;
            const int BACK = 2;
            const int SPANNING = 3;

            if (SPCounter == 118)
            {
                int foo = 10;
            }


            // Classify each point as well as the entire polygon into one of the above
            // four classes.
            int polygonType = 0;
            List<int> types = new List<int>();
            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                float t = Vector3.Dot(this.normal, polygon.Vertices[i].Position) - this.w;
//                console.log(polygon.Vertices[i].Position.X + " " + polygon.Vertices[i].Position.Y + " " + polygon.Vertices[i].Position.Z + " " + t);
                //Debug.Log("normal " + this.normal.ToString() + "pos " + polygon.Vertices[i].Position + " w " + this.w + " t " + t);
                int type = (t < -EPSILON) ? BACK : (t > EPSILON) ? FRONT : COPLANAR;
                polygonType |= type;
                types.Add(type);
            }

            //Debug.Log("polygontype " + polygonType);
            // Put the polygon in the correct list, splitting it when necessary.
            switch (polygonType)
            {
                case COPLANAR:
                    (Vector3.Dot(this.normal, polygon.Plane.normal) > 0 ? coplanarFront : coplanarBack).Add(polygon);
                    break;
                case FRONT:
                    front.Add(polygon);
                    break;
                case BACK:
                    back.Add(polygon);
                    break;
                case SPANNING:
                    List<Vertex> f = new List<Vertex>();
                    List<Vertex> b = new List<Vertex>();
                    for (int i = 0; i < polygon.Vertices.Length; i++)
                    {
                        int j = (i + 1) % polygon.Vertices.Length;
                        int ti = types[i];
                        int tj = types[j];

                        Vertex vi = polygon.Vertices[i];
                        Vertex vj = polygon.Vertices[j];
                        if (ti != BACK) f.Add(vi);
                        if (ti != FRONT) b.Add(ti != BACK ? vi.Clone() : vi);
                        if ((ti | tj) == SPANNING)
                        {
                            float t = (this.w - Vector3.Dot(this.normal, vi.Position)) / Vector3.Dot(this.normal, vj.Position - vi.Position);
                            Vertex v = vi.Interpolate(vj, t);
                            f.Add(v);
                            b.Add(v.Clone());
                        }
                    }
                    if (f.Count >= 3) front.Add(new Polygon(f.ToArray(), polygon.Shared));
                    if (b.Count >= 3) back.Add(new Polygon(b.ToArray(), polygon.Shared));
                    break;
            }

            SPCounter++;
        }
    }
}
