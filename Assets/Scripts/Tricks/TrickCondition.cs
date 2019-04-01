using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TrickCondition
{
    public enum ConditionState
    {
        FAIL,
        RUNNING,
        SUCCESS,
    }

    public abstract ConditionState TestCondition(TrickEventData trickEvent);

#if UNITY_EDITOR
    public abstract void OnInspectorGUI();
    public abstract string GetInspectorHeaderName();
#endif
}
