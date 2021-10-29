using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PauseCheck))]
public class PauseCheckEditor : Editor
{

    public override void OnInspectorGUI()
    {
        PauseCheck pauseCheck = (PauseCheck)target;
        if (DrawDefaultInspector())
        {

        }
        if (GUILayout.Button("pause"))
        {
            pauseCheck.TogglePause();
        }
    }
}