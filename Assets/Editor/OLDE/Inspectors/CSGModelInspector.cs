using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CSGModel))]
public class CSGModelInspector : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Rebuild Model"))
        {
            ((CSGModel)target).Rebuild();
        }
    }
    //void OnSceneGUI()
    //{
    //    Debug.Log("Current detected event: " + Event.current);
    //    if (Event.current.Equals(Event.KeyboardEvent("a")))
    //    {
    //        Debug.Log("Success");
    //        GameObject instantiatedBrush = ((ActiveBrush)Instantiate(((CSGModel)target).ActiveBrush)).gameObject;
    //        instantiatedBrush.transform.parent = ((CSGModel)target).transform;
    //    }

    //}
}
