using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TrickParser : TrickConditional
{
    public List<TrickConditional> m_trickConditions;
    private int m_conditionalIndex;//

    public TrickParser(Trick trick)
    {
        m_trickConditions = trick.TrickConditions;
        m_conditionalIndex = m_trickConditions.Count - 1;
    }

    public override ConditionState TestCondition(TrickEventData nextEvent)
    {
        if (m_trickConditions.Count == 0 || m_conditionalIndex < 0)
            return ConditionState.FAIL;

        ConditionState state = m_trickConditions[m_conditionalIndex].TestCondition(nextEvent);

        switch (state) {
            case ConditionState.FAIL:
                return ConditionState.FAIL;
            case ConditionState.RUNNING:
                if (m_conditionalIndex == 0) {
                    return ConditionState.FAIL;
                } else {
                    return ConditionState.RUNNING;
                }
            case ConditionState.SUCCESS:
                if (m_conditionalIndex == 0) {
                    return ConditionState.SUCCESS;
                } else {
                    m_conditionalIndex--;
                    return ConditionState.RUNNING;
                }
            default:
                break;
        }

        return ConditionState.FAIL;
    }
}