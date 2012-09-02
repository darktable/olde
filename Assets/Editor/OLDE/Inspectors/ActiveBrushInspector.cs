using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ActiveBrush))]
public class ActiveBrushInspector : Editor
{
    public override void OnInspectorGUI()
    {
		// TODO: Undo support		
		((ActiveBrush)target).ActiveBrushTypeProp = (ActiveBrush.ActiveBrushType)EditorGUILayout.EnumPopup(((ActiveBrush)target).ActiveBrushTypeProp);//, ActiveBrush.ActiveBrushType);		
		
        if (GUILayout.Button("Add"))
        {
            ((ActiveBrush)target).transform.parent.GetComponent<CSGModel>().AddActiveBrush();
        }
        if (GUILayout.Button("Subtract"))
        {
            ((ActiveBrush)target).transform.parent.GetComponent<CSGModel>().SubtractActiveBrush();
        }        	
    }
    void OnSceneGUI()
    {
        //Debug.Log(Event.current);

        if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.KeypadPlus)
        {
            ((ActiveBrush)target).transform.parent.GetComponent<CSGModel>().AddActiveBrush();
        }
        if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.KeypadMinus)
        {
            ((ActiveBrush)target).transform.parent.GetComponent<CSGModel>().SubtractActiveBrush();
        }
        if (Event.current.Equals(Event.KeyboardEvent("#r")))
        {
            ((ActiveBrush)target).transform.parent.GetComponent<CSGModel>().Rebuild();
        }
//        if (Event.current.isMouse)
//        {
//            //if(Physics.RaycastAll)
//            //Event.current.mouseRay
//        }
    }
}
