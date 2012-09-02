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
    /// Holds a node in a BSP tree. A BSP tree is built from a collection of polygons
    /// by picking a polygon to split along. That polygon (and all other coplanar
    /// polygons) are added directly to that node and the other polygons are added to
    /// the front and/or back subtrees. This is not a leafy BSP tree since there is
    /// no distinction between internal and leaf nodes.
    /// </summary>
	public class Node
	{
		Plane plane;
		Node front;
		Node back;
		List<Polygon> polygons = new List<Polygon>();

		public Polygon[] Polygons
		{
			get
			{
				return this.polygons.ToArray();
			}
		}
		public Polygon[] AllPolygons
		{
			get
			{
				Polygon[] polygons = this.polygons.Clone<Polygon>().ToArray();

				if(this.front != null) polygons = polygons.Concat(this.front.AllPolygons).ToArray();
				if(this.back != null) polygons = polygons.Concat(this.back.AllPolygons).ToArray();
				return polygons;
			}
		}

		public Node()
		{
            this.polygons = new List<Polygon>();
		}
		public Node(Polygon[] polygons)
		{
            this.polygons = new List<Polygon>();

			if (polygons.Length > 0)
			{
				this.Build(polygons.ToList());
			}
		}

		public Node Clone()
		{
			Node node = new Node();
			node.plane = this.plane;
			node.front = this.front;
			node.back = this.back;
			node.polygons = this.polygons;
			return node;
		}

        /// <summary>
        /// Convert solid space to empty space and empty space to solid space.
        /// </summary>
		public void Invert()
		{
			for (int i = 0; i < polygons.Count; i++)
			{
				this.polygons[i].Flip();
			}

			this.plane.Flip();
			if (this.front != null) this.front.Invert();
			if (this.back != null) this.back.Invert();
			Node temp = this.front;
			this.front = this.back;
			this.back = temp;
		}
		// TODO: Check this function
        /// <summary>
        /// Recursively remove all polygons in `polygons` that are inside this BSP tree.
        /// </summary>
		public List<Polygon> ClipPolygons(Polygon[] polygons)
		{
            if (this.plane == null) 
            {
                return polygons.Clone<Polygon>().ToList();
            } // TODO: Removed slice here?

			List<Polygon> front = new List<Polygon>();
			List<Polygon> back = new List<Polygon>();

			for (int i = 0; i < polygons.Length; i++)
			{
				this.plane.SplitPolygon(polygons[i], ref front, ref back, ref front, ref back);
            }
            if (this.front != null)
            {
                front = this.front.ClipPolygons(front.ToArray());
            }
            if (this.back != null)
            {
                back = this.back.ClipPolygons(back.ToArray());
            }
            else back = new List<Polygon>();

			return front.Concat(back).ToList();
		}

		public void ClipTo(Node bsp)
		{
			this.polygons = bsp.ClipPolygons(this.polygons.ToArray());
			if (this.front != null) this.front.ClipTo(bsp);
			if (this.back != null) this.back.ClipTo(bsp);
		}
		public void Build(List<Polygon> polygons)
        {
            //Debug.Log("node:build started, polycount " + polygons.Count);
            if (polygons.Count == 0)
            {
                //Debug.Log("node:build no polys, returning");
                
                return;
            }
            if (this.plane == null) this.plane = polygons[0].Plane.Clone();

			List<Polygon> front = new List<Polygon>();
			List<Polygon> back = new List<Polygon>();

			for (int i = 0; i < polygons.Count; i++)
			{
				this.plane.SplitPolygon(polygons[i], ref this.polygons, ref this.polygons, ref front, ref back);
			}

            //Debug.Log("node:build front length: " + front.Count + " back length " + back.Count); 

			if (front.Count > 0)
			{
				if (this.front == null) this.front = new Node();
				this.front.Build(front);
			}

			if (back.Count > 0)
			{
				if (this.back == null) this.back = new Node();
				this.back.Build(back);
			}
		}
	}

}
