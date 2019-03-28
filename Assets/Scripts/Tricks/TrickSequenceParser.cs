using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrickSequenceParser : ISerializationCallbackReceiver
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

    public virtual ParserType GetParserType() { throw new NotImplementedException(); }
    public virtual TrickSequenceParser Instantiate() { throw new NotImplementedException(); }
    public virtual SequenceState ProcessTrickEvent(TrickEventData eventData) { throw new NotImplementedException(); }
    protected virtual void @Reset() { throw new NotImplementedException(); }

    public virtual void OnBeforeSerialize() { return; }
    public virtual void OnAfterDeserialize() { return; }

#if UNITY_EDITOR
    public virtual void OnInspectorGUI() { return; }
#endif
}