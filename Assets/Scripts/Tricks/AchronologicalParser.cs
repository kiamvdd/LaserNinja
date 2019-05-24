using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

// Works mostly the same as a linear parser, but achronological events can be specified and they will be compared against 
public class AchronologicalParser : LinearTrickParser
{
    public enum RelativeOrder
    {
        BEFORE,
        AFTER,
    }

    public List<TrickCondition> AchronConditions = new List<TrickCondition>();
    public List<Tuple<RelativeOrder, int>> m_achronConditionOrder;

    private List<TrickCondition> m_activeAchronConditions = new List<TrickCondition>();
    private List<List<float>> m_activeAchronConditionTimeStamps = new List<List<float>>();
    private List<Tuple<RelativeOrder, int>> m_activeAchronConditionOrder;
    private List<float> m_achronSuccessTimeStamps = new List<float>();

    public float m_waitTimeLimit = 20;

    private float m_mainSequenceFinishTime = -1;
    private bool m_mainSequenceFinished = false;
    private bool m_allAchronConditionsFinished = false;

    public AchronologicalParser()
    {
        Conditions = new List<TrickCondition>();
        AchronConditions = new List<TrickCondition>();
        m_achronConditionOrder = new List<Tuple<RelativeOrder, int>>();
    }

    private AchronologicalParser(AchronologicalParser parser)
    {
        Conditions = new List<TrickCondition>(parser.Conditions);
        TimeIntervals = new List<float>(parser.TimeIntervals);
        AchronConditions = new List<TrickCondition>(parser.AchronConditions);
        m_achronConditionOrder = new List<Tuple<RelativeOrder, int>>(parser.m_achronConditionOrder);
        RepeatAmount = parser.RepeatAmount;
        m_waitTimeLimit = parser.m_waitTimeLimit;
        Reset();
    }

    public override TrickSequenceParser Instantiate()
    {
        return new AchronologicalParser(this);
    }

    public override SequenceState ProcessTrickEvent(TrickEventData eventData)
    {
        SequenceState mainSequenceState = SequenceState.SUCCESS;
        int succeededEventCount = m_successTimeStamps.Count;

        if (!m_mainSequenceFinished) {
            mainSequenceState = base.ProcessTrickEvent(eventData);

            switch (mainSequenceState) {
                case SequenceState.FAIL:
                    m_state = ParserState.EXIT;
                    return SequenceState.FAIL;
                case SequenceState.SUCCESS:
                    m_mainSequenceFinished = true;
                    m_mainSequenceFinishTime = Time.time;

                    ProcessBufferedTimeStamps();

                    if (m_activeAchronConditions.Count == 0) {
                        m_state = ParserState.EXIT;
                        return SequenceState.SUCCESS;
                    } else {
                        m_state = ParserState.RUNNING;
                    }
                    break;
            }
        }

        // If this event did not cause a condition to be met in the linear list, test it on the achronological conditions
        if (succeededEventCount == m_successTimeStamps.Count) {
            SequenceState achronSequenceState = TestAchronConditions(eventData);

            switch (achronSequenceState) {
                case SequenceState.FAIL:
                    m_state = ParserState.EXIT;
                    return SequenceState.FAIL;
                case SequenceState.RUNNING:
                    m_state = ParserState.RUNNING;
                    return SequenceState.RUNNING;
                case SequenceState.SUCCESS:
                    if (mainSequenceState == SequenceState.SUCCESS) {
                        m_state = ParserState.EXIT;
                        return SequenceState.SUCCESS;
                    } else {
                        m_state = ParserState.RUNNING;
                        return SequenceState.RUNNING;
                    }
            }
        }

        if (mainSequenceState == SequenceState.RUNNING || mainSequenceState == SequenceState.SUCCESS) {
            m_state = ParserState.RUNNING;
            return SequenceState.RUNNING;
        }


        m_state = ParserState.EXIT;
        return SequenceState.FAIL;
    }

    private void ProcessBufferedTimeStamps()
    {
        for (int i = m_activeAchronConditionTimeStamps.Count - 1; i >= 0; i--) {
            foreach (float timestamp in m_activeAchronConditionTimeStamps[i]) {
                if (TestForTimestampAtIndex(timestamp, i)) {
                    m_activeAchronConditions.RemoveAt(i);
                    m_activeAchronConditionOrder.RemoveAt(i);
                    break;
                }
            }

            // remove list after it's been processed
            m_activeAchronConditionTimeStamps.RemoveAt(i);
        }
    }

    private bool TestForTimestampAtIndex(float timestamp, int index)
    {
        if (m_activeAchronConditionOrder[index].Item1 == RelativeOrder.BEFORE) {
            if (timestamp < m_successTimeStamps[m_activeAchronConditionOrder[index].Item2]
                && (m_activeAchronConditionOrder[index].Item2 - 1 < 0 || timestamp > m_successTimeStamps[m_activeAchronConditionOrder[index].Item2 - 1])) {
                return true;
            }
        } else if (m_activeAchronConditionOrder[index].Item1 == RelativeOrder.AFTER) {
            if (timestamp > m_successTimeStamps[m_activeAchronConditionOrder[index].Item2]
                && (m_activeAchronConditionOrder[index].Item2 + 1 >= m_successTimeStamps.Count || timestamp < m_successTimeStamps[m_activeAchronConditionOrder[index].Item2 + 1])) {
                return true;
            }
        }

        return false;
    }

    private SequenceState TestAchronConditions(TrickEventData eventData)
    {
        if (m_mainSequenceFinished && Time.time - m_mainSequenceFinishTime > m_waitTimeLimit) {
            return SequenceState.FAIL;
        }

        if (m_mainSequenceFinished) {

            for (int i = 0; i < m_activeAchronConditions.Count; i++) {
                TrickCondition.ConditionState conditionState = m_activeAchronConditions[i].TestCondition(eventData);
                if (conditionState == TrickCondition.ConditionState.SUCCESS) {

                    if (TestForTimestampAtIndex(eventData.TimeStamp, i)) {
                        m_activeAchronConditions.RemoveAt(i);
                        m_activeAchronConditionOrder.RemoveAt(i);
                        break;
                    }
                }
            }
        } else {
            // Add matching events to timestamp list for later evaluation
            for (int i = 0; i < m_activeAchronConditions.Count; i++) {
                TrickCondition.ConditionState conditionState = m_activeAchronConditions[i].TestCondition(eventData);

                if (conditionState == TrickCondition.ConditionState.SUCCESS) {
                    m_activeAchronConditionTimeStamps[i].Add(eventData.TimeStamp);
                }
            }
        }

        if (m_activeAchronConditions.Count == 0) {
            return SequenceState.SUCCESS;
        } else {
            return SequenceState.RUNNING;
        }
    }

    protected override void Reset()
    {
        base.Reset();
        m_achronSuccessTimeStamps = new List<float>(new float[AchronConditions.Count]);
        m_activeAchronConditions = new List<TrickCondition>(AchronConditions);
        m_activeAchronConditionOrder = new List<Tuple<RelativeOrder, int>>(m_achronConditionOrder);

        m_activeAchronConditionTimeStamps = new List<List<float>>(m_activeAchronConditions.Count);
        for (int i = 0; i < m_activeAchronConditions.Count; i++) {
            m_activeAchronConditionTimeStamps.Add(new List<float>());
        }
    }

    public override ParserType GetParserType()
    {
        return ParserType.ACHRONOLOGICAL;
    }

#if UNITY_EDITOR && !FAKE_BUILD
    public override void OnInspectorGUIBody()
    {
        base.OnInspectorGUIBody();

        if (GUILayout.Button("Add Equals Condition")) {
            AchronConditions.Add(new TrickTypeEquals());
            m_achronConditionOrder.Add(new Tuple<RelativeOrder, int>(RelativeOrder.BEFORE, 0));
        }

        if (GUILayout.Button("Add Debug Condition")) {
            AchronConditions.Add(new DebugTrickCondition());
            m_achronConditionOrder.Add(new Tuple<RelativeOrder, int>(RelativeOrder.BEFORE, 0));
        }

        ReorderableListDrawer listDrawer = new ReorderableListDrawer();
        listDrawer.ShowReorderButtons = false;
        listDrawer.StartList(AchronConditions.Count, GUIStyle.none);
        for (int i = 0; i < AchronConditions.Count; i++) {
            listDrawer.StartItemDraw(i, i + ". " + AchronConditions[i].GetInspectorHeaderName());
            EditorGUILayout.BeginHorizontal();
            m_achronConditionOrder[i] = new Tuple<RelativeOrder, int>((RelativeOrder)EditorGUILayout.EnumPopup(m_achronConditionOrder[i].Item1), m_achronConditionOrder[i].Item2);
            m_achronConditionOrder[i] = new Tuple<RelativeOrder, int>(m_achronConditionOrder[i].Item1, EditorGUILayout.IntSlider(m_achronConditionOrder[i].Item2, 0, Conditions.Count - 1));
            EditorGUILayout.EndHorizontal();
            AchronConditions[i].OnInspectorGUI();
            listDrawer.EndItemDraw();
        }
        listDrawer.EndList();

        var removedIndices = listDrawer.GetRemovedIndices();
        for (int i = AchronConditions.Count - 1; i >= 0; i--) {
            if (removedIndices.Contains(i)) {
                AchronConditions.RemoveAt(i);
                m_achronConditionOrder.RemoveAt(i);
            }
        }
    }
#endif
}
