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

namespace OLDE
{
    public class CSG
    {
        Polygon[] polygons;

        public Polygon[] Polygons
        {
            get { return polygons; }
        }

        public CSG()
        {
            polygons = new Polygon[0];
        }

        public static CSG FromPolygons(Polygon[] polygons)
        {
            CSG csg = new CSG();
            csg.polygons = polygons;
            return csg;
        }

        public CSG Clone()
        {
            CSG csg = new CSG();
            csg.polygons = this.polygons.Clone<Polygon>(); // TODO should clone?
            return csg;
        }

        public CSG Union(CSG csg)
        {
            Node a = new Node(this.Clone().polygons);
            Node b = new Node(csg.Clone().polygons);

            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons.ToList());

            return CSG.FromPolygons(a.AllPolygons);
        }

        public CSG Subtract(CSG csg)
        {            
            Node a = new Node(this.Clone().polygons);
            Node b = new Node(csg.Clone().polygons);

            a.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons.ToList());
            a.Invert();

            return CSG.FromPolygons(a.AllPolygons);
        }

        public CSG Intersect(CSG csg)
        {
            Node a = new Node(this.Clone().polygons);
            Node b = new Node(csg.Clone().polygons);

            a.Invert();
            b.ClipTo(a);
            b.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            a.Build(b.AllPolygons.ToList());
            a.Invert();
            return CSG.FromPolygons(a.AllPolygons);
        }

        public CSG Inverse()
        {
            CSG csg = this.Clone();
            for (int i = 0; i < csg.polygons.Length; i++)
            {
                csg.polygons[i].Flip();
            }
            return csg;
        }

        public void SetShared(object shared)
        {
            for (int i = 0; i < polygons.Length; i++)
            {
                polygons[i].Shared = shared;
            }
        }

        public class Cube
        {
            public CSG Generate(Vector3 center, Vector3 radius, Quaternion rotation)
            {
                Polygon[] polygons = new Polygon[6];

                polygons[0] = new Polygon(new Vertex[] {
                    new Vertex(new Vector3(-1, -1, -1).Multiply (radius), new Vector3(-1, 0, 0), new Vector2(radius.z*2,0)),
                    new Vertex(new Vector3(-1, -1, 1).Multiply (radius), new Vector3(-1, 0, 0), new Vector2(0,0)),
                    new Vertex(new Vector3(-1, 1, 1).Multiply (radius), new Vector3(-1, 0, 0), new Vector2(0,radius.y*2)),
                    new Vertex(new Vector3(-1, 1, -1).Multiply (radius), new Vector3(-1, 0, 0), new Vector2(radius.z*2,radius.y*2)),
                });

                polygons[1] = new Polygon(new Vertex[] {
                    new Vertex(new Vector3(1, -1, -1).Multiply (radius), new Vector3(1, 0, 0), new Vector2(radius.y*2,0)),
                    new Vertex(new Vector3(1, 1, -1).Multiply (radius), new Vector3(1, 0, 0), new Vector2(0,0)),
                    new Vertex(new Vector3(1, 1, 1).Multiply (radius), new Vector3(1, 0, 0), new Vector2(0,radius.z*2)),
                    new Vertex(new Vector3(1, -1, 1).Multiply (radius), new Vector3(1, 0, 0), new Vector2(radius.y*2,radius.z*2)),
                });

                polygons[2] = new Polygon(new Vertex[] {
                    new Vertex(new Vector3(-1, -1, -1).Multiply (radius) , new Vector3(0, -1, 0), new Vector2(radius.x*2,0)),
                    new Vertex(new Vector3(1, -1, -1).Multiply (radius) , new Vector3(0, -1, 0), new Vector2(0,0)),
                    new Vertex(new Vector3(1, -1, 1).Multiply(radius) , new Vector3(0, -1, 0), new Vector2(0,radius.z*2)),
                    new Vertex(new Vector3(-1, -1, 1).Multiply (radius) , new Vector3(0, -1, 0), new Vector2(radius.x*2,radius.z*2)),
                });

                polygons[3] = new Polygon(new Vertex[] {
                    new Vertex(new Vector3(-1, 1, -1).Multiply (radius), new Vector3(0, 1, 0), new Vector2(radius.z*2,0)),
                    new Vertex(new Vector3(-1, 1, 1).Multiply (radius), new Vector3(0, 1, 0), new Vector2(0,0)),
                    new Vertex(new Vector3(1, 1, 1).Multiply (radius), new Vector3(0, 1, 0), new Vector2(0,radius.x*2)),
                    new Vertex(new Vector3(1, 1, -1).Multiply (radius), new Vector3(0, 1, 0), new Vector2(radius.z*2,radius.x*2)),
                });

                polygons[4] = new Polygon(new Vertex[] {
                    new Vertex(new Vector3(-1, -1, -1).Multiply (radius), new Vector3(0, 0, -1), new Vector2(radius.y*2,0)),
                    new Vertex(new Vector3(-1, 1, -1).Multiply (radius), new Vector3(0, 0, -1), new Vector2(0,0)),
                    new Vertex(new Vector3(1, 1, -1).Multiply (radius), new Vector3(0, 0, -1), new Vector2(0,radius.x*2)),
                    new Vertex(new Vector3(1, -1, -1).Multiply (radius), new Vector3(0, 0, -1), new Vector2(radius.y*2,radius.x*2)),
                });

                polygons[5] = new Polygon(new Vertex[] {
                    new Vertex(new Vector3(-1, -1, 1).Multiply (radius), new Vector3(0, 0, 1), new Vector2(radius.x*2,0)),
                    new Vertex(new Vector3(1, -1, 1).Multiply (radius), new Vector3(0, 0, 1), new Vector2(0,0)),
                    new Vertex(new Vector3(1, 1, 1).Multiply (radius), new Vector3(0, 0, 1), new Vector2(0,radius.y*2)),
                    new Vertex(new Vector3(-1, 1, 1).Multiply (radius), new Vector3(0, 0, 1), new Vector2(radius.x*2,radius.y*2)),
                });
				
				for (int i = 0; i < polygons.Length; i++) {
					for (int j = 0; j < polygons[i].Vertices.Length; j++) {						
						polygons[i].Vertices[j].Position = rotation * polygons[i].Vertices[j].Position + center;
						polygons[i].Vertices[j].Normal = rotation * polygons[i].Vertices[j].Normal;
					}
				}

                return CSG.FromPolygons(polygons);
            }


        }

        public class Sphere
        {
            Vector3 center = Vector3.zero;
            float radius = 1.0f;
            int slices = 16;
            int stacks = 8;

            List<Polygon> polygons = new List<Polygon>();
            List<Vertex> vertices = new List<Vertex>();

            public CSG Generate(Vector3 center, float radius, int slices, int stacks)
            {
                this.center = center;
                this.radius = radius;
                for (float i = 0; i < slices; i++)
                {
                    for (float j = 0; j < stacks; j++)
                    {
                        vertices = new List<Vertex>();
                        Vertex(i / slices, j / stacks);
                        if (j > 0) Vertex((i + 1) / slices, j / stacks);
                        if (j < stacks - 1) Vertex((i + 1) / slices, (j + 1) / stacks);
                        Vertex(i / slices, (j + 1) / stacks);

                        for (int vi = 0; vi < vertices.Count; vi++)
                        {
                            //Debug.Log(vertices[vi].Position + " " + vertices[vi].Normal);
                        }

                        polygons.Add(new Polygon(vertices.ToArray()));
                    }
                }

                return CSG.FromPolygons(polygons.ToArray());
            }

            void Vertex(float theta, float phi)
            {

                theta *= (float)Math.PI * 2;
                phi *= (float)Math.PI;
                Vector3 dir = new Vector3(
                    (float)(Math.Cos(theta) * Math.Sin(phi)),
                    (float)Math.Cos(phi),
                    (float)(Math.Sin(theta) * Math.Sin(phi))
                );
                vertices.Add(new Vertex(center + dir * radius, dir, Vector2.zero));
            }
        }

        public class Cylinder
        {
//CSG.cylinder = function(options) {
//  options = options || {};
//  var s = new CSG.Vector(options.start || [0, -1, 0]);
//  var e = new CSG.Vector(options.end || [0, 1, 0]);
//  var ray = e.minus(s);
//  var r = options.radius || 1;
//  var slices = options.slices || 16;
//  var axisZ = ray.unit(), isY = (Math.abs(axisZ.y) > 0.5);
//  var axisX = new CSG.Vector(isY, !isY, 0).cross(axisZ).unit();
//  var axisY = axisX.cross(axisZ).unit();
//  var start = new CSG.Vertex(s, axisZ.negated());
//  var end = new CSG.Vertex(e, axisZ.unit());
//  var polygons = [];
//  function point(stack, slice, normalBlend) {
//    var angle = slice * Math.PI * 2;
//    var out = axisX.times(Math.cos(angle)).plus(axisY.times(Math.sin(angle)));
//    var pos = s.plus(ray.times(stack)).plus(out.times(r));
//    var normal = out.times(1 - Math.abs(normalBlend)).plus(axisZ.times(normalBlend));
//    return new CSG.Vertex(pos, normal);
//  }
//  for (var i = 0; i < slices; i++) {
//    var t0 = i / slices, t1 = (i + 1) / slices;
//    polygons.push(new CSG.Polygon([start, point(0, t0, -1), point(0, t1, -1)]));
//    polygons.push(new CSG.Polygon([point(0, t1, 0), point(0, t0, 0), point(1, t0, 0), point(1, t1, 0)]));
//    polygons.push(new CSG.Polygon([end, point(1, t1, 1), point(1, t0, 1)]));
//  }
//  return CSG.fromPolygons(polygons);
//};
			Vector3 start;
			Vector3 end;
			float radius;
			Vector3 axisX;
			Vector3 axisY;
			Vector3 axisZ;
			Vector3 ray;
			
			public CSG Generate(Vector3 start, Vector3 end, float radius, int slices)
			{
				this.start = start;
				this.end = end;
				this.radius = radius;
				
				ray = end - start;
				axisZ = ray.normalized;
				bool isY = (Mathf.Abs(axisZ.y) > 0.5f);
				axisX = Vector3.Cross(new Vector3((isY ? 1 : 0), (isY ? 0 : 1), 0), axisZ).normalized;
				axisY = Vector3.Cross(axisX, axisZ).normalized;
				Vertex startVertex = new Vertex(start, axisZ * -1, Vector2.zero);
				Vertex endVertex = new Vertex(end, axisZ.normalized, Vector2.zero);
				
				List<Polygon> polygons = new List<Polygon>();
				
				for (int i = 0; i < slices; i++) 
				{
					float t0 = (float)i/slices;
					float t1 = (float)(i + 1) / slices;
					
					polygons.Add(new Polygon(new Vertex[] { startVertex, Point(0, t0, -1), Point(0, t1, -1)}));
					polygons.Add(new Polygon(new Vertex[] { Point(0, t1, 0), Point(0, t0, 0), Point(1, t0, 0), Point(1, t1, 0)}));
					polygons.Add(new Polygon(new Vertex[] { endVertex, Point(1, t1, 1), Point(1, t0, 1)}));
				}
				return CSG.FromPolygons(polygons.ToArray());
			}
			Vertex Point(float stack, float slice, float normalBlend) 
			{
				float angle = slice * Mathf.PI * 2;
				Vector3 output = axisX * Mathf.Cos(angle) + axisY * (Mathf.Sin(angle));
				Vector3 pos = (start + ray * stack) + output * radius;
				Vector3 normal = (output * (1 - Mathf.Abs(normalBlend))) + axisZ * normalBlend;
				return new Vertex(pos, normal, Vector2.zero);
			}
        }
    }
}
