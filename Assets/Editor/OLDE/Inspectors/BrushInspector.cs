using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Brush))]
public class BrushInspector : Editor
{
    void OnSceneGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent("#r")))
        {
            ((Brush)target).transform.parent.GetComponent<CSGModel>().Rebuild();
        }
    }
}