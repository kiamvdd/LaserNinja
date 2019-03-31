using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TrickSequenceParser
{
    public enum ParserType
    {
        LINEAR,
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

    protected ParserState m_state = ParserState.START;
    public ParserState State { get { return m_state; } }

    public abstract ParserType GetParserType();
    public abstract TrickSequenceParser Instantiate();
    public abstract SequenceState ProcessTrickEvent(TrickEventData eventData);
    protected abstract void @Reset();

#if UNITY_EDITOR
    public virtual void OnInspectorGUI() { return; }
#endif
}