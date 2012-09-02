#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using OLDE;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CSGModel : MonoBehaviour
{
    [SerializeField]
    ActiveBrush activeBrush; // The gizmo like object that the level designer can manipulate to stamp brushes

    public ActiveBrush ActiveBrush
    {
        get { return activeBrush; }
		set { activeBrush = value; }
    }

    [SerializeField]
    List<Brush> brushes; // Store the sequence of brushes and their operation (e.g. add, subtract)
	
	GameObject meshGroup = null;

    public void Rebuild()
    {
		// If there's an existing MeshGroup object, get rid of it
		if(transform.FindChild("MeshGroup") != null)
		{
			DestroyImmediate(transform.FindChild("MeshGroup").gameObject);
		}

        if (brushes != null && brushes.Count > 0)
        {
			// Ensure any brush objects that have been deleted by the user are removed from the brush sequence
            for (int i = 0; i < brushes.Count; i++)
            {
                if (brushes[i] == null)
                {
                    brushes.RemoveAt(i);
                    i--;
                }
            }

            CSG current = null;
			// Iterate through each brush in sequence
            for (int i = 0; i < brushes.Count; i++)
            {				
                Brush.CSGMode mode = brushes[i].Mode;
				
                // Generate the CSG geometry for the brush
				CSG brushGeometry = brushes[i].GenerateCSG();
			
                brushGeometry.SetShared(brushes[i].SharedBrushData);

                if (i == 0)
                {
					// The first brush is always added to the space (rather than subtracted)
                    current = brushGeometry;
                }
                else
                {
					// For every brush after the first one, carry out the correct operation
                    if (mode == Brush.CSGMode.Subtract)
                    {
                        current = current.Subtract(brushGeometry);
                    }
                    else if (mode == Brush.CSGMode.Union)
                    {
                        current = current.Union(brushGeometry);
                    }
                    else if (mode == Brush.CSGMode.Intersect)
                    {
                        current = current.Intersect(brushGeometry);
                    }
                }
            }
			// Now that the final CSG has been calculated, fetch the polygons (mixture of triangles and quads)
            Polygon[] polygons = current.Polygons;
			
			// Create polygon subsets for each material
			Dictionary<Material, List<Polygon>> polygonMaterialTable = new Dictionary<Material, List<Polygon>>();
			
			// Iterate through every polygon adding it to the appropiate material list
            foreach (Polygon polygon in current.Polygons)
            {
				Material material = ((SharedBrushData)polygon.Shared).Material;
				if(!polygonMaterialTable.ContainsKey(material))
				{
					polygonMaterialTable.Add(material, new List<Polygon>());
				}
				
				polygonMaterialTable[material].Add(polygon);
			}
			
			// Create a grouping object which will act as a parent for all the per material meshes
			meshGroup = new GameObject("MeshGroup");
			meshGroup.transform.parent = this.transform;
			
			// Create a separate mesh for polygons of each material so that we batch by material
			foreach (KeyValuePair<Material, List<Polygon>> polygonMaterialGroup in polygonMaterialTable)
			{
		        Mesh mesh = new Mesh();
		        List<Vector3> vertices = new List<Vector3>();
		        List<Vector3> normals = new List<Vector3>();
		        List<Vector2> uvs = new List<Vector2>();
		        List<Color> colors = new List<Color>();
		        List<int> triangles = new List<int>();
				
				// Setup an indexer that tracks unique vertices, so that we reuse vertex data appropiately
	            Indexer indexer = new Indexer();
				
				// Iterate through every polygon and triangulate
	            foreach (Polygon polygon in polygonMaterialGroup.Value)
	            {
	                List<int> indices = new List<int>();
										
	                for (int i = 0; i < polygon.Vertices.Length; i++)
	                {
						// Each vertex must know about its shared data for geometry tinting
	                    polygon.Vertices[i].Shared = polygon.Shared;
						// If the vertex is already in the indexer, fetch the index otherwise add it and get the added index
	                    int index = indexer.Add(polygon.Vertices[i]);
						// Put each vertex index in an array for use in the triangle generation
	                    indices.Add(index);
	                }
					
					// Triangulate the n-sided polygon and allow vertex reuse by using indexed geometry
	                for (int i = 2; i < indices.Count; i++)
	                {
	                    triangles.Add(indices[0]);
	                    triangles.Add(indices[i-1]);
	                    triangles.Add(indices[i]);
	                }
	            }
				
				// Create the relevant buffers from the vertex array
	            for (int i = 0; i < indexer.Vertices.Count; i++)
	            {
	                vertices.Add(indexer.Vertices[i].Position);
	                normals.Add(indexer.Vertices[i].Normal);
	                uvs.Add(indexer.Vertices[i].UV);
	                colors.Add(((SharedBrushData)indexer.Vertices[i].Shared).BrushTintColor);
	            }           
	        
				// Set the mesh buffers
		        mesh.vertices = vertices.ToArray();
		        mesh.normals = normals.ToArray();
		        mesh.colors = colors.ToArray();
		        mesh.uv = uvs.ToArray();
		        mesh.triangles = triangles.ToArray();
				
				// Optionally you can turn these on to ensure normals are correct and also generate tangents
					// mesh.RecalculateNormals();
					// GenerateTangents(mesh);
				
				GameObject materialMesh = new GameObject("MaterialMesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
				materialMesh.transform.parent = meshGroup.transform;
				
				// Set the mesh to be rendered
		        materialMesh.GetComponent<MeshFilter>().mesh = mesh;
				// Set the collision mesh
		        materialMesh.GetComponent<MeshCollider>().mesh = mesh;
				
				materialMesh.renderer.material = polygonMaterialGroup.Key;
			}
		}
    }
	/// <summary>
	/// Adds a brush to the CSG using the Active Brush
	/// </summary>
    public void AddActiveBrush()
    {
		CreateBrushFromActive(Brush.CSGMode.Union);
    }
	/// <summary>
	/// Subtracts a brush from the CSG using the Active Brush
	/// </summary>
    public void SubtractActiveBrush()
    {
		CreateBrushFromActive(Brush.CSGMode.Subtract);
    }
	private void CreateBrushFromActive(Brush.CSGMode csgMode)
	{
		if (brushes == null)
        {
            brushes = new List<Brush>();
        }
        GameObject instantiatedBrush = (GameObject)Instantiate(activeBrush.gameObject);
        instantiatedBrush.transform.parent = this.transform;
        instantiatedBrush.name = "AppliedBrush";
        DestroyImmediate(instantiatedBrush.GetComponent<ActiveBrush>());
		
        Brush brush = instantiatedBrush.GetComponent<Brush>();
        brush.Mode = csgMode;
        brushes.Add(brush);

        Rebuild();
	}
	
	/// <summary>
	/// By Kyle Gibbar (see http://answers.unity3d.com/questions/7789/calculating-tangents-vector4.html)
	/// </summary>
	public void GenerateTangents(Mesh mesh)
    {
        int triangleCount = mesh.triangles.Length;
        int vertexCount = mesh.vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

        for(long a = 0; a < triangleCount; a+=3)
        {
            long i1 = mesh.triangles[a+0];
            long i2 = mesh.triangles[a+1];
            long i3 = mesh.triangles[a+2];

            Vector3 v1 = mesh.vertices[i1];
            Vector3 v2 = mesh.vertices[i2];
            Vector3 v3 = mesh.vertices[i3];

            Vector2 w1 = mesh.uv[i1];
            Vector2 w2 = mesh.uv[i2];
            Vector2 w3 = mesh.uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }


        for (long a = 0; a < vertexCount; ++a)
        {
            Vector3 n = mesh.normals[a];
            Vector3 t = tan1[a];

            Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
            tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }
}

#endif