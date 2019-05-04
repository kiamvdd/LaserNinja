using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

// Will find a series of events with
public class LinearTrickParser : TrickSequenceParser
{
    public List<TrickCondition> Conditions = new List<TrickCondition>();
    public List<float> TimeIntervals = new List<float>();
    public float RepeatInterval = 0;
    private Queue<TrickCondition> m_activeConditions;
    protected List<float> m_successTimeStamps = new List<float>();
    protected int m_successCount = 0;
    protected float m_repeatTimeStamp = 0;

    private StringBuilder m_debugSB = new StringBuilder();//

    public LinearTrickParser()
    {
        Conditions = new List<TrickCondition>();
        TimeIntervals = new List<float>();
    }

    private LinearTrickParser(LinearTrickParser parser)
    {
        Conditions = new List<TrickCondition>(parser.Conditions);
        TimeIntervals = new List<float>(parser.TimeIntervals);
        RepeatAmount = parser.RepeatAmount;
        RepeatInterval = parser.RepeatInterval;
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

    public override void ForcePrintDebugLog()
    {
        Debug.Log(m_debugSB.ToString());
    }

    // Resetcheck is true when processtrickevent is called to test a failed eventdata on the first condition in the list
    private SequenceState ProcessTrickEvent(TrickEventData eventData, bool resetCheck)
    {
        if (eventData.Type == TrickEventData.TrickEventType.KILL) {
            Debug.Log("Kill event received");
        }

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
                    return ProcessTrickEvent(eventData, true);
                } else {
                    m_debugSB.AppendLine("<color=red>Parser exiting.</color>");
                    Debug.Log(m_debugSB.ToString());
                    m_debugSB.AppendLine("<color=red>IF THIS SHOWS, THE PARSER HAS NOT EXITED CORRECTLY.</color>");
                    m_state = ParserState.EXIT;
                    return SequenceState.FAIL;
                }
            case TrickCondition.ConditionState.RUNNING:
                m_debugSB.AppendLine("<color=blue>Parser continuing.</color>");
                m_state = ParserState.RUNNING;
                return SequenceState.RUNNING;
            case TrickCondition.ConditionState.SUCCESS:
                m_successTimeStamps.Add(eventData.TimeStamp);
                int index = Conditions.Count - m_activeConditions.Count;
                if (index > 0)
                {
                    if (TimeIntervals[index - 1] != 0)
                    {
                        float interval = m_successTimeStamps[index] - m_successTimeStamps[index - 1];
                        if (interval > TimeIntervals[index - 1])
                        {
                            m_state = ParserState.EXIT;
                            return SequenceState.FAIL;
                        }
                    }
                }

                m_activeConditions.Dequeue();
                if (m_activeConditions.Count == 0) {

                    if (RepeatAmount > 1 && RepeatInterval != 0)
                    {
                        if (m_successCount > 0)
                        {
                            if (eventData.TimeStamp - m_repeatTimeStamp > RepeatInterval)
                            {
                                m_state = ParserState.EXIT;
                                return SequenceState.FAIL;
                            }
                        }
                    }

                    ++m_successCount;
                    m_repeatTimeStamp = eventData.TimeStamp;

                    if (m_successCount == RepeatAmount) {
                        m_debugSB.AppendLine("<color=green>Sequence completed on iteration " + m_successCount + ". Parser exiting.</color>");
                        Debug.Log(m_debugSB.ToString());
                        m_debugSB.AppendLine("<color=red>IF THIS SHOWS, THE PARSER HAS NOT EXITED CORRECTLY.</color>");

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

        Debug.Log("<color=red>Parser exited unexpectedly.</color>");
        Debug.Log(m_debugSB.ToString());
        return SequenceState.FAIL;
    }

    protected override void Reset()
    {
        m_activeConditions = new Queue<TrickCondition>();
        for (int i = 0; i < Conditions.Count; i++) {
            m_activeConditions.Enqueue(Conditions[i]);
        }

        m_successTimeStamps = new List<float>();

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
        OnValidate();

        if (RepeatAmount > 1)
        {
            RepeatInterval = EditorGUILayout.FloatField("Max time between repeats", RepeatInterval);
        }

        if (GUILayout.Button("Add Equals Condition"))
        {
            Conditions.Add(new TrickTypeEquals());
            if (Conditions.Count > 1)
            {
                TimeIntervals.Add(0);
            }
        }

        if (GUILayout.Button("Add Debug Condition"))
        {
            Conditions.Add(new DebugTrickCondition());
            if (Conditions.Count > 1)
            {
                TimeIntervals.Add(0);
            }
        }

        ReorderableListDrawer listDrawer = new ReorderableListDrawer();

        listDrawer.StartList(Conditions.Count, GUIStyle.none);
            for (int i = 0; i < Conditions.Count; i++) {
                listDrawer.StartItemDraw(i, i + ". " + Conditions[i].GetInspectorHeaderName());
                    Conditions[i].OnInspectorGUI();
                listDrawer.EndItemDraw();

                if (Conditions.Count > 1 && i < Conditions.Count - 1)
                {
                    TimeIntervals[i] = EditorGUILayout.FloatField("Max interval", TimeIntervals[i]);        
                }   
            }
        listDrawer.EndList();

        var removedIndices = listDrawer.GetRemovedIndices();
        for (int i = Conditions.Count - 1; i >= 0; i--) {
            if (removedIndices.Contains(i)) {
                if (Conditions.Count > 1 && i > 0)
                {
                    TimeIntervals.RemoveAt(i - 1);
                }

                Conditions.RemoveAt(i);
            }
        }

        if (removedIndices.Count == 0)
        {
            var indexOrder = listDrawer.GetFinalListOrder();
            List<TrickCondition> tempConditions = new List<TrickCondition>(Conditions);
            for (int i = 0; i < Conditions.Count; i++) {
                Conditions[i] = tempConditions[indexOrder[i]];
            }
        }
    }

    public override void OnValidate()
    {
        if (Conditions.Count > 1 && TimeIntervals == null || TimeIntervals.Count == 0)
        {
            TimeIntervals = new List<float>();

            for (int i = 0; i < Conditions.Count - 1; i++)
            {
                TimeIntervals.Add(0);
            }
        }
    }
#endif
}
