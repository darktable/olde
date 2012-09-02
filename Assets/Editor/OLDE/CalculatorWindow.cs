using UnityEngine;
using System.Collections;
using UnityEditor;

public class CalculatorWindow : EditorWindow
{
	float rise = 1;
	float run = 1;
	[MenuItem("OLDE/Angle Calculator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CalculatorWindow window = (CalculatorWindow)EditorWindow.GetWindow(typeof(CalculatorWindow));
        //SceneView.onSceneGUIDelegate
    }
	void OnGUI()
	{
		rise = EditorGUILayout.FloatField("Rise", rise);
		run = EditorGUILayout.FloatField("Run", run);
		
		float angle = Mathf.Rad2Deg * Mathf.Atan2(rise, run);
		EditorGUILayout.FloatField("Result", angle);
		//GUILayout.Label(string.Format("Result: {0} degrees", angle));		
	}
}
