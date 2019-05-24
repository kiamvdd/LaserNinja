using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugTrickCondition : TrickCondition
{
    public bool ReturnValue = true;
    public override ConditionState TestCondition(TrickEventData trickEvent)
    {
        return ReturnValue ? ConditionState.SUCCESS : ConditionState.FAIL;
    }

#if UNITY_EDITOR && !FAKE_BUILD
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        ReturnValue = EditorGUILayout.Toggle("Return Value: ", ReturnValue);
        EditorGUILayout.EndVertical();
    }

    public override string GetInspectorHeaderName()
    {
        return "Debug Condition";
    }
#endif
}
