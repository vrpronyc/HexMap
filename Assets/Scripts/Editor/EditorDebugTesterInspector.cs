using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(EditorDebugTester), editorForChildClasses:true)]
public class EditorDebugTesterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorDebugTester myScript = (EditorDebugTester)target;
        if (GUILayout.Button("Click"))
        {
            myScript.Click();
        }
    }
}
