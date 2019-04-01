using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickInterpreter : MonoBehaviour
{
    [SerializeField]
    private List<Trick> m_tricks = new List<Trick>();

    [SerializeField]
    private TrickList m_trickListUI;

    private void Awake()
    {
        EventBus.OnTrickEvent += OnTrickEvent;

        foreach (Trick trick in m_tricks) {
            trick.OnTrickCompleted += OnTrickCompleted;
        }
    }

    private RingBuffer<TrickEventData> m_eventDataBuffer = new RingBuffer<TrickEventData>(4);
    private void OnTrickEvent(TrickEventData eventData)
    {
        m_eventDataBuffer.Add(eventData);

        for (int i = 0; i < m_eventDataBuffer.Length; i++) {
            for (int j = 0; j < m_eventDataBuffer.Length; j++) {
                if (j == i)
                    continue;

                if (m_eventDataBuffer[j].TimeStamp == m_eventDataBuffer[i].TimeStamp)
                    Debug.Log("<color=red>Duplicate event!</color>");
            }
        }

        foreach (Trick trick in m_tricks) {
            trick.ProcessTrickEvent(eventData);
        }
    }

    private void OnTrickCompleted(Trick trick)
    {
        m_trickListUI.AddTrick(trick);
    }

    private void OnDestroy()
    {
        EventBus.OnTrickEvent -= OnTrickEvent;
        foreach (Trick trick in m_tricks) {
            trick.OnTrickCompleted -= OnTrickCompleted;
        }
    }
}