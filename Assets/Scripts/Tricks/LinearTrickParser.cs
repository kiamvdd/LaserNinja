using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

// Will find a series of events with
[Serializable]
public class LinearTrickParser : TrickSequenceParser
{
    [SerializeField]
    private List<TrickCondition> m_conditions = new List<TrickCondition>();
    public List<TrickCondition> Conditions { get { return m_conditions; } }
    private Queue<TrickCondition> m_activeConditions;

    private int m_successCount = 0;

    private StringBuilder m_debugSB = new StringBuilder();

    public LinearTrickParser()
    {
        m_conditions = new List<TrickCondition>();
    }

    private LinearTrickParser(LinearTrickParser parser)
    {
        m_conditions = new List<TrickCondition>(parser.Conditions);
        RepeatAmount = parser.RepeatAmount;
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
        m_debugSB.AppendLine("Parser receiving " + eventData.Type.ToString() + (resetCheck ? " from reset check" : "") + ", state is " + m_state.ToString());
        TrickCondition currentCondition = m_activeConditions.Peek();
        TrickCondition.ConditionState conditionState = currentCondition.TestCondition(eventData);
        string color = "";
        switch (conditionState) {
            case TrickCondition.ConditionState.FAIL:
                color = "<color=red>";
                break;
            case TrickCondition.ConditionState.RUNNING:
                color = "<color=blue>";
                break;
            case TrickCondition.ConditionState.SUCCESS:
                color = "<color=green>";
                break;
        }

        m_debugSB.AppendLine("Parser testing event with result " + color + conditionState.ToString() + "</color>");

        switch (conditionState) {
            case TrickCondition.ConditionState.FAIL:
                if (m_state == ParserState.RUNNING && !resetCheck) {
                    m_debugSB.AppendLine("<color=red>Parser resetting to process event with first condition</color>");
                    Reset();
                    ProcessTrickEvent(eventData, true);
                } else {
                    m_debugSB.AppendLine("<color=red>Parser exiting.</color>");
                    m_state = ParserState.EXIT;
                    return SequenceState.FAIL;
                }
                break;
            case TrickCondition.ConditionState.RUNNING:
                m_debugSB.AppendLine("<color=blue>Parser continuing.</color>");
                m_state = ParserState.RUNNING;
                return SequenceState.RUNNING;
            case TrickCondition.ConditionState.SUCCESS:
                m_activeConditions.Dequeue();
                if (m_activeConditions.Count == 0) {
                    ++m_successCount;
                    if (m_successCount == RepeatAmount) {
                        m_debugSB.AppendLine("<color=green>Sequence completed on iteration " + m_successCount + ". Parser exiting.</color>");
                        Debug.Log(m_debugSB.ToString());

                        for (int i = 0; i < 10; i++) {
                            m_debugSB.AppendLine("<color=red>IF THIS SHOWS, THE PARSER HAS NOT EXITED CORRECTLY.</color>");
                        }

                        m_state = ParserState.EXIT;
                        return SequenceState.SUCCESS;
                    } else {
                        m_debugSB.AppendLine("<color=green>Iteration [" + m_successCount + "] successful.</color>");
                        m_debugSB.AppendLine("<color=blue>Parser continuing.</color>");
                        Reset();
                        return SequenceState.RUNNING;
                    }
                } else {
                    m_debugSB.AppendLine("<color=blue>Parser continuing.</color>");
                    m_state = ParserState.RUNNING;
                    return SequenceState.RUNNING;
                }
        }

        return SequenceState.FAIL;
    }

    protected override void Reset()
    {
        m_activeConditions = new Queue<TrickCondition>();
        for (int i = 0; i < m_conditions.Count; i++) {
            m_activeConditions.Enqueue(m_conditions[i]);
        }

        m_state = ParserState.START;
    }

    public override ParserType GetParserType()
    {
        return ParserType.LINEAR;
    }

#if UNITY_EDITOR
    public override void OnInspectorGUIBody()
    {
        base.OnInspectorGUIBody();

        if (GUILayout.Button("Add Equals Condition"))
            m_conditions.Add(new TrickTypeEquals());

        if (GUILayout.Button("Add Debug Condition"))
            m_conditions.Add(new DebugTrickCondition());

        ReorderableListDrawer listDrawer = new ReorderableListDrawer();

        listDrawer.StartList(m_conditions.Count, GUIStyle.none);
            for (int i = 0; i < m_conditions.Count; i++) {
                listDrawer.StartItemDraw(i, m_conditions[i].GetInspectorHeaderName());
                    m_conditions[i].OnInspectorGUI();
                listDrawer.EndItemDraw();
            }
        listDrawer.EndList();

        var removedIndices = listDrawer.GetRemovedIndices();
        for (int i = m_conditions.Count - 1; i >= 0; i--) {
            if (removedIndices.Contains(i)) {
                m_conditions.RemoveAt(i);
            }
        }

        var indexOrder = listDrawer.GetFinalListOrder();
        List<TrickCondition> tempConditions = new List<TrickCondition>(m_conditions);
        for (int i = 0; i < m_conditions.Count; i++) {
            m_conditions[i] = tempConditions[indexOrder[i]];
        }
    }
#endif
}
