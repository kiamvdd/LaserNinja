using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickEventData
{
    [Flags]
    public enum TrickEventType
    {
        NONE = 0,
        PLAYERWALLRIDE = 1 << 0,
        PLAYERJUMP = 1 << 1,
        PLAYERWALLJUMP = 1 << 2,
        PLAYERLAND = 1 << 3,
        KILL = 1 << 4,
        CLOSECALL = 1 << 5,
        SHOOT = 1 << 6,
        KILLSHOT = 1 << 7,
        PLAYERFALL = 1 << 8,
    }

    private TrickEventType m_type;
    public TrickEventType Type { get { return m_type; } }
    private float m_timeStamp;
    public float TimeStamp { get { return m_timeStamp; } }

    public TrickEventData(TrickEventType type)
    {
        m_type = type;
        m_timeStamp = Time.realtimeSinceStartup;
    }
}
