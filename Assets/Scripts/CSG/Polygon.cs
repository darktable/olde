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

namespace OLDE
{
    /// <summary>
    /// Represents a convex polygon. The vertices used to initialize a polygon must
    /// be coplanar and form a convex loop. They do not have to be `CSG.Vertex`
    /// instances but they must behave similarly (duck typing can be used for
    /// customization).
    /// 
    /// Each convex polygon has a `shared` property, which is shared between all
    /// polygons that are clones of each other or were split from the same polygon.
    /// This can be used to define per-polygon properties (such as surface color).
    /// </summary>
	public class Polygon : ICloneable<Polygon>
	{
        Vertex[] vertices;
        object shared;
        Plane plane;

        public Plane Plane
        {
            get { return plane; }
        }

        public Vertex[] Vertices
        {
            get { return vertices; }
        }

        public object Shared
        {
            get { return shared; }
            set { shared = value; }
        }

        public Polygon(Vertex[] vertices)
        {
            this.vertices = vertices;
            this.shared = null;
            this.plane = Plane.FromPoints(vertices[0].Position, vertices[1].Position, vertices[2].Position);
        }
        public Polygon(Vertex[] vertices, object shared)
        {
            this.vertices = vertices;
            this.shared = shared;
            this.plane = Plane.FromPoints(vertices[0].Position, vertices[1].Position, vertices[2].Position);
        }

        public Polygon Clone()
        {
            return new Polygon(vertices.Clone<Vertex>().ToArray(), shared);
        }

        public void Flip()
        {
            Array.Reverse(vertices);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Flip();
//                console.log("FlippedN " + vertices[i].Normal.X + " " + vertices[i].Normal.Y + " " + vertices[i].Normal.Z);
//                console.log("FlippedP " + vertices[i].Position.X + " " + vertices[i].Position.Y + " " + vertices[i].Position.Z);
            }
            plane.Flip();            
        }

	}
}
