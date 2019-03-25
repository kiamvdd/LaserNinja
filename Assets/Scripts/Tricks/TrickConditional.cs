using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TrickConditional : ScriptableObject
{
    public enum ConditionState
    {
        FAIL,
        RUNNING,
        SUCCESS,
    }

    public abstract ConditionState TestCondition(TrickEventData eventData);

#if UNITY_EDITOR
    private bool m_removeFromGUI = false;
    public bool RemoveFromGUI { get { return m_removeFromGUI; } }

    public void TagForRemoval() { m_removeFromGUI = true; }
    public virtual void DrawConditionalGUI() { }
#endif
}