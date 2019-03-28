using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TrickCondition : ISerializationCallbackReceiver
{
    public enum ConditionState
    {
        FAIL,
        RUNNING,
        SUCCESS,
    }

    public abstract ConditionState TestCondition(TrickEventData trickEvent);

    public abstract void OnBeforeSerialize();
    public abstract void OnAfterDeserialize();

#if UNITY_EDITOR
    public abstract void OnInspectorGUI();
#endif
}
