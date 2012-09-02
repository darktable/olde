#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class ActiveBrush : MonoBehaviour
{
	public enum ActiveBrushType { Primitive, Compound };
	
	[SerializeField]
	ActiveBrushType activeBrushType = ActiveBrushType.Primitive;

	public ActiveBrushType ActiveBrushTypeProp {
		get {
			return this.activeBrushType;
		}
		set {
			ActiveBrushType previousActiveBrushType = activeBrushType;			
			activeBrushType = value;
			// Only update the brush type when the brush type has changed
			if(previousActiveBrushType != activeBrushType)
			{
				UpdateBrushType();
			}
		}
	}	
	void Awake()
	{
		UpdateBrushType();
	}
	void UpdateBrushType()
	{
		// Track the material so when we change the type of brush that's attached we persist the material across
		Material cachedMaterial = null;
				
		if(activeBrushType == ActiveBrushType.Primitive)
		{
			// If there's the wrong sort of brush attached get rid of it
			if(GetComponent<CompoundBrush>() != null)
			{
				cachedMaterial = GetComponent<CompoundBrush>().Material;
				DestroyImmediate(GetComponent<CompoundBrush>());
			}
			// If the desired brush hasn't already been added then add it
			if(GetComponent<PrimitiveBrush>() == null)
			{
				gameObject.AddComponent<PrimitiveBrush>();
				gameObject.GetComponent<PrimitiveBrush>().Material = cachedMaterial;
			}
		}
		else
		{
			// If there's the wrong sort of brush attached get rid of it
			if(GetComponent<PrimitiveBrush>() != null)
			{
				cachedMaterial = GetComponent<PrimitiveBrush>().Material;
				DestroyImmediate(GetComponent<PrimitiveBrush>());
			}
			// If the desired brush hasn't already been added then add it
			if(GetComponent<CompoundBrush>() == null)
			{
				gameObject.AddComponent<StairBrush>();
				gameObject.GetComponent<StairBrush>().Material = cachedMaterial;
			}
		}
	}
}

#endif