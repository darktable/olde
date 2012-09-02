using System;
using UnityEngine;
using OLDE;

public abstract class Brush : MonoBehaviour
{
	public enum CSGMode { Union, Subtract, Intersect };
    
	[SerializeField]
	CSGMode mode = CSGMode.Union;
	
	[SerializeField]
	Material material;
	
	[SerializeField]
    protected Color tintColor = Color.white;
	
	public CSGMode Mode {
		get {
			return this.mode;
		}
        set {
			this.mode = value;
		}
	}
	
	public Material Material {
		get {
			return this.material;
		}
		set {
			material = value;
		}
	}

    public Color TintColor
    {
        get { return tintColor; }
        set { tintColor = value; }
    }
	
	public SharedBrushData SharedBrushData
	{
		get { return new SharedBrushData(material, tintColor); }
	}
	
	public abstract CSG GenerateCSG();		
}

