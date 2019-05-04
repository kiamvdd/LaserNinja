using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class TrickSequenceParser
{
    #region Enums
    public enum ParserType
    {
        LINEAR,
        ACHRONOLOGICAL
    }

    public enum ParserState
    {
        START,
        RUNNING,
        EXIT,
    }

    public enum SequenceState
    {
        FAIL,
        RUNNING,
        SUCCESS,
    }
    #endregion

    public abstract void ForcePrintDebugLog();

    protected ParserState m_state = ParserState.START;
    public ParserState State { get { return m_state; } }
    public int RepeatAmount;
    public abstract ParserType GetParserType(); 
    public abstract TrickSequenceParser Instantiate();
    public abstract SequenceState ProcessTrickEvent(TrickEventData eventData);
    protected abstract void @Reset();
    public abstract void OnValidate();

#if UNITY_EDITOR
    public virtual void OnInspectorGUIBody()
    {
        EditorGUILayout.BeginHorizontal();
        Rect labelRect = GUILayoutUtility.GetRect(new GUIContent("Repeat"), "label");
        GUI.Label(labelRect, "Finish");
        RepeatAmount = EditorGUILayout.IntField(RepeatAmount, GUILayout.Width(30));

        if (RepeatAmount < 1)
            RepeatAmount = 1;

        EditorGUILayout.LabelField("times to complete");
        EditorGUILayout.EndHorizontal();
    }
#endif
}