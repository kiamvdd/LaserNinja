using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Will find a series of events with
public class LinearTrickParser : TrickSequenceParser
{
    [SerializeField]
    private TrickCondition[] m_conditions = new TrickCondition[0];
    public TrickCondition[] Conditions { get { return m_conditions; } }
    private Queue<TrickCondition> m_activeConditions;

    //[Serializable]
    //private class LinearTrickParserMemo
    //{
    //    [SerializeField]
    //    private TrickCondition[] m_conditions;
    //    public TrickCondition[] Conditions { get { return m_conditions; } }

    //    public LinearTrickParserMemo(LinearTrickParser parser)
    //    {
    //        m_conditions = parser.m_conditions;
    //    }
    //}

    //[SerializeField]
    //private LinearTrickParserMemo m_trickParserMemo = null;

    private LinearTrickParser(LinearTrickParser parser)
    {
        m_conditions = new TrickCondition[parser.m_conditions.Length];
        for (int i = 0; i < m_conditions.Length; i++) {
            m_conditions[i] = parser.m_conditions[i];
        }

        Reset();
    }

    public override TrickSequenceParser Instantiate()
    {
        return new LinearTrickParser(this);
    }

    public override SequenceState ProcessTrickEvent(TrickEventData eventData)
    {
        return ProcessTrickEvent(eventData, false);
    }

    // Resetcheck is true when processtrickevent is called to test a failed eventdata on the first condition in the list
    private SequenceState ProcessTrickEvent(TrickEventData eventData, bool resetCheck)
    {
        TrickCondition currentCondition = m_activeConditions.Peek();
        TrickCondition.ConditionState conditionState = currentCondition.TestCondition(eventData);

        switch (conditionState) {
            case TrickCondition.ConditionState.FAIL:
                if (m_state == ParserState.RUNNING && !resetCheck) {
                    Reset();
                    ProcessTrickEvent(eventData, true);
                } else {
                    m_state = ParserState.EXIT;
                    return SequenceState.FAIL;
                }
                break;
            case TrickCondition.ConditionState.RUNNING:
                m_state = ParserState.RUNNING;
                return SequenceState.RUNNING;
            case TrickCondition.ConditionState.SUCCESS:
                m_activeConditions.Dequeue();
                if (m_activeConditions.Count == 0) {
                    m_state = ParserState.EXIT;
                    return SequenceState.SUCCESS;
                } else {
                    m_state = ParserState.RUNNING;
                    return SequenceState.RUNNING;
                }
        }

        return SequenceState.FAIL;
    }

    protected override void @Reset()
    {
        m_activeConditions = new Queue<TrickCondition>();
        for (int i = 0; i < m_conditions.Length; i++) {
            m_activeConditions.Enqueue(m_conditions[i]);
        }

        m_state = ParserState.START;
    }

#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        foreach (TrickCondition condition in m_conditions) {
            // draw shit idfk
        }
    }   
#endif

    #region Serialization
    //public override void OnBeforeSerialize()
    //{
    //    m_trickParserMemo = new LinearTrickParserMemo(this);
    //}

    //public override void OnAfterDeserialize()
    //{
    //    if (m_trickParserMemo != null) {
    //        m_conditions = new List<TrickCondition>(m_trickParserMemo.Conditions);
    //    }
    //}
    #endregion
}
