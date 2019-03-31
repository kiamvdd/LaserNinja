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

    private void OnTrickEvent(TrickEventData eventData)
    {
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