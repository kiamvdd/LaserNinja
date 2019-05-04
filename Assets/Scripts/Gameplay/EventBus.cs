using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    public static Action<TrickEventData> OnTrickEvent;
    public static Action OnTimerStart;
}