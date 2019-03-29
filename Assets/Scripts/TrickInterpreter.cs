using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickInterpreter : MonoBehaviour
{
    [SerializeField]
    private List<Trick> m_trickList = new List<Trick>();

    private void Awake()
    {
        EventBus.OnTrickEvent += OnTrickEvent;

        foreach (Trick trick in m_trickList) {
            trick.OnTrickCompleted += OnTrickCompleted;
        }
    }

    private void OnTrickEvent(TrickEventData eventData)
    {

        //foreach (Trick trick in m_trickList) {
        //    trick.ProcessTrickEvent(eventData);
        //}
    }

    private void OnTrickCompleted(Trick trick)
    {

    }

    private void OnDestroy()
    {
        EventBus.OnTrickEvent -= OnTrickEvent;
    }
}

public class TrickEventData : IComparable<TrickEventData>, IEquatable<TrickEventData>
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
        CLOSECALL = 1 << 4,
    }

    private TrickEventType m_type;
    public TrickEventType Type { get { return m_type; } }
    private float m_timeStamp;
    public float TimeStamp { get { return m_timeStamp; } }

    public bool Active { get; set; }

    public TrickEventData(TrickEventType type, bool active = true)
    {
        m_type = type;
        m_timeStamp = Time.realtimeSinceStartup;
    }

    public int CompareTo(TrickEventData other)
    {
        if (other.TimeStamp < m_timeStamp)
            return 1;
        else if (other.TimeStamp > m_timeStamp)
            return -1;
        else
            return 0;
    }

    public bool Equals(TrickEventData other)
    {
        return (other.Type == m_type) && (other.TimeStamp == m_timeStamp);
    }
}

public class TrickEventDataComparer : EqualityComparer<TrickEventData>
{
    public override bool Equals(TrickEventData x, TrickEventData y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return x.Equals(y);
    }

    public override int GetHashCode(TrickEventData obj)
    {
        return obj.TimeStamp.GetHashCode();
    }
}