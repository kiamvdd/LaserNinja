using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickInterpreter : MonoBehaviour
{
    [SerializeField]
    private List<Trick> m_trickList = new List<Trick>();

    [SerializeField]
    private LevelTimer m_levelTimer;

    private RingBuffer<TrickEventData> m_trickEvents = new RingBuffer<TrickEventData>(100);

    private void OnTrickEvent(TrickEventData eventData)
    {
        m_trickEvents.Add(eventData);
        CheckForTrickCompletion();
    }

    private void OnUpdateTrickEvent(TrickEventData eventData)
    {
        int eventIndex = m_trickEvents.IndexOf(eventData);

        if (eventIndex != -1) {
            m_trickEvents[eventIndex].Active = true;
        }
    }

    private void CheckForTrickCompletion()
    {
        if (!(m_trickEvents.Length > 0))
            return;

        foreach (Trick trick in m_trickList) {
            TrickParser trickParser = new TrickParser(trick);

            int eventIndex = m_trickEvents.Length - 1;
            TrickConditional.ConditionState state;

            do {
                state = trickParser.TestCondition(m_trickEvents[eventIndex]);
                if (state == TrickConditional.ConditionState.SUCCESS) {
                    m_levelTimer.AddTimeFromTrick(trick);
                    break;
                }

                eventIndex--;
            } while (state != TrickConditional.ConditionState.FAIL && eventIndex >= 0);
        }
    }
}

public class TrickEventData : IComparable<TrickEventData>, IEquatable<TrickEventData>
{
    public enum TrickEventType
    {
        WALLRIDE,
        JUMP,
        LAND,
        KILL,
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