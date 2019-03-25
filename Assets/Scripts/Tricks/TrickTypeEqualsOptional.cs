using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "LightBounce/TrickConditionals/EqualsOptional")]
public class TrickTypeEqualsOptional : TrickConditional
{
    public List<TrickEventData.TrickEventType> OptionalTrickEventTypes = new List<TrickEventData.TrickEventType>();
    public TrickEventData.TrickEventType EndTrickEventType;
    public bool NegateEnd = false;

    public override ConditionState TestCondition(TrickEventData eventData)
    {
        bool endEventFound = (eventData.Type == EndTrickEventType);

        if (NegateEnd)
            endEventFound = !endEventFound;

        if (endEventFound) {
            return ConditionState.SUCCESS;
        }

        if (OptionalTrickEventTypes.Contains(eventData.Type)) {
            return ConditionState.RUNNING;
        } else {
            return ConditionState.FAIL;
        }
    }
}