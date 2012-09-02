#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using OLDE;
using System;

[ExecuteInEditMode]
public class PrimitiveBrush : Brush {
	
	public enum PrimitiveBrushType { Cube, Sphere, Cylinder };
	
	[SerializeField]
    protected PrimitiveBrushType primitiveBrushType = PrimitiveBrushType.Cube;
	
    public PrimitiveBrushType Type
    {
        get
        {
            return this.primitiveBrushType;
        }
        set
        {
            this.primitiveBrushType = value;
        }
    }		
	public override CSG GenerateCSG ()
	{
		if (primitiveBrushType == PrimitiveBrushType.Cube)
        {
            return new CSG.Cube().Generate(transform.position, transform.localScale * 0.5f, transform.rotation);
        }
        else if (primitiveBrushType == PrimitiveBrushType.Sphere)
        {
            return new CSG.Sphere().Generate(transform.position, transform.localScale.x, 16, 8);
        }
		else if (primitiveBrushType == PrimitiveBrushType.Cylinder)
        {
            return new CSG.Cylinder().Generate(transform.position, transform.position + Vector3.up * 5, 2.0f, 16);
        }
		else
		{
			throw new NotImplementedException();
		}
	}

    public void OnDrawGizmos()
    {		
		if(name == "ActiveBrush")
		{
        	Gizmos.color = Color.red;
		}
		else
		{
        	Gizmos.color = Color.blue;
		}

        if (primitiveBrushType == PrimitiveBrushType.Cube)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
        else if (primitiveBrushType == PrimitiveBrushType.Sphere)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.x));
            Gizmos.DrawWireSphere(Vector3.zero, 1);
        }
    }
}

#endif