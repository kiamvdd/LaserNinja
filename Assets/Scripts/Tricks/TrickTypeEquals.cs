using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "LightBounce/TrickConditionals/Equals")]
public class TrickTypeEquals : TrickConditional
{
    [EnumFlag]
    public TrickEventData.TrickEventType TrickEventType;

    public bool Negate = false;
    public override ConditionState TestCondition(TrickEventData eventData)
    {
        bool flagContainsEvent = (TrickEventType & eventData.Type) > 0;

        if (Negate)
            return (!flagContainsEvent ? ConditionState.SUCCESS : ConditionState.FAIL);
        else
            return (flagContainsEvent ? ConditionState.SUCCESS : ConditionState.FAIL);
    }
}
