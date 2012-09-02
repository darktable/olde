using System;
using UnityEngine;

public class SharedBrushData
{
	Color brushTintColor = Color.white; // Tint color for vertices using this shared brush data
	Material material = null; // Material used to draw polygons using this shared brush data

	public Color BrushTintColor {
		get {
			return this.brushTintColor;
		}
		set {
			brushTintColor = value;
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
	
	public SharedBrushData (Material material, Color brushTintColor)
	{
		this.material = material;
		this.brushTintColor = brushTintColor;
	}
}