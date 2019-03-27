using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TrickCondition
{
    public enum ConditionState
    {
        FAIL,
        RUNNING,
        SUCCESS,
    }

    public abstract ConditionState TestCondition(TrickEventData trickEvent);
}
