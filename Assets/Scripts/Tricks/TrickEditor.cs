using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Trick))]
public class TrickEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Trick trick = (Trick)target;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        trick.ParserTemplate.OnInspectorGUIBody();
        EditorGUILayout.EndVertical();
    }
}
