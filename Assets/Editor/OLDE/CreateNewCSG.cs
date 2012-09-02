using System;
using UnityEditor;
using UnityEngine;

public class CreateNewCSG : Editor
{
	[MenuItem("OLDE/Create New CSG")]
	static void CreateNewCSGObject()
	{
		// Create objects to hold the CSG Model and Active Brush (with associated scripts attached)
		GameObject rootGameObject = new GameObject("NewCSG", typeof(CSGModel));
		GameObject activeBrushGameObject = new GameObject("ActiveBrush", typeof(ActiveBrush));
		
		// Anchor the brush inside the CSG Model
		activeBrushGameObject.transform.parent = rootGameObject.transform;
		
		// Ensure the CSG Model reference to the Active Brush is set
		rootGameObject.GetComponent<CSGModel>().ActiveBrush = activeBrushGameObject.GetComponent<ActiveBrush>();
		
		// Link up a default material to the active brush
		activeBrushGameObject.GetComponent<Brush>().Material = (Material)Resources.Load("DefaultCSGMaterial");
				
		// Set the user's selection to the new CSG Model, so that they can start working with it
		Selection.activeGameObject = rootGameObject;
	}
}

