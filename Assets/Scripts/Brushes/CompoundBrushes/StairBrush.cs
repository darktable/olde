#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using OLDE;

[ExecuteInEditMode]
public class StairBrush : CompoundBrush
{
	private struct BrushData
	{
    	public Vector3 position;
    	public Vector3 size;
	}
    [SerializeField] int stepCount = 10;

    [SerializeField] float stepWidth = 3;
    [SerializeField] float stepHeight = 0.1f;
    [SerializeField] float stepDepth = 0.5f;
    
    [SerializeField] float stepHorizontalSpacing = 0;
    [SerializeField] float stepVerticalSpacing = 0.1f;
	
	public override CSG GenerateCSG ()
	{
		CSG current = null;
		BrushData[] brushData = BrushDataProp;
        for (int i = 0; i < brushData.Length; i++)
        {
			CSG temp = new CSG.Cube().Generate( transform.position + transform.rotation * brushData[i].position, brushData[i].size * 0.5f, transform.rotation);
			if(i == 0)
			{
				current = temp;
			}
			else
			{
            	current = current.Union(temp);
			}
        }
		return current;
	}

    void OnDrawGizmos()
    {
        if(name == "ActiveBrush")
		{
        	Gizmos.color = Color.red;
		}
		else
		{
        	Gizmos.color = Color.blue;
		}
		
		// Draw the steps
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(stepWidth, stepHeight, stepDepth));
        for (int i = 0; i < stepCount; i++)
		{
			Gizmos.DrawWireCube( new Vector3(1.0f/stepWidth, 1.0f/stepHeight, 1.0f/stepDepth).Multiply(Vector3.forward * i * (stepDepth + stepHorizontalSpacing) + Vector3.up * i * (stepHeight + stepVerticalSpacing)), Vector3.one);
		}
    }
	/// <summary>
	/// Use to get the position and size of each step cuboid in the stairs
	/// </summary>
    private BrushData[] BrushDataProp
    {
        get
        {
            BrushData[] data = new BrushData[stepCount];
            for (int i = 0; i < stepCount; i++)
            {
                data[i] = new BrushData();
                data[i].position = Vector3.forward * i * (stepDepth + stepHorizontalSpacing) + Vector3.up * i * (stepHeight + stepVerticalSpacing);
                data[i].size = new Vector3(stepWidth, stepHeight, stepDepth);
            }
            return data;
        }
    }
}

#endif