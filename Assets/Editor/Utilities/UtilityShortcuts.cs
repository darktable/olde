using UnityEngine;
using System.Collections;
using UnityEditor;

public class UtilityShortcuts : MonoBehaviour
{
    /// <summary>
    /// Toggles the editor scene camera between orthographic and perspective
    /// </summary>
    [MenuItem("MVI/Utility Shortcuts/Toggle Camera Mode %#c")]
    static void ToggleCameraMode()
    {
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.orthographic = !SceneView.lastActiveSceneView.orthographic;
            SceneView.lastActiveSceneView.Repaint();
        }
    }
    /// <summary>
    /// For every selected game object, the object is recursively toggled based on the root selected game object's active state
    /// </summary>
    [MenuItem("MVI/Utility Shortcuts/Toggle Object Recursively %#t")]
    static void ToggleObjectRecursively()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
		{
            Selection.gameObjects[i].SetActiveRecursively(!Selection.gameObjects[i].active);
		}        
    }

    [MenuItem("MVI/Utility Shortcuts/Select Parent %#p")]
    static void SelectParent()
    {
        if (Selection.activeTransform.parent != null)
            Selection.activeTransform = Selection.activeTransform.parent;
    }
}