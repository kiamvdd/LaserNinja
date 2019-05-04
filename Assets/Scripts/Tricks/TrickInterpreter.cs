using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickInterpreter : MonoBehaviour
{
    [SerializeField]
    private List<Trick> m_tricks = new List<Trick>();

    [SerializeField]
    private LevelTimer m_levelTimer;

    private void Awake()
    {
        EventBus.OnTrickEvent += OnTrickEvent;

        foreach (Trick trick in m_tricks) {
            trick.OnTrickCompleted += OnTrickCompleted;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) {
            Debug.Log("<color=red>START OF DEBUG LOG STREAM</color>");
            foreach (Trick trick in m_tricks) {
                trick.ForcePrintDebugLogs();
            }
            Debug.Log("<color=red>END OF DEBUG LOG STREAM</color>");
        }
    }

    private void OnTrickEvent(TrickEventData eventData)
    {
        Debug.Log(eventData.Type.ToString()); 
        // receive jump
        foreach (Trick trick in m_tricks) {
            // pass jump to each trick
            trick.ProcessTrickEvent(eventData);
        }
    }

    private void OnTrickCompleted(Trick trick)
    {
        m_levelTimer.AddTimeFromTrick(trick);
    }

    private void OnDestroy()
    {
        EventBus.OnTrickEvent -= OnTrickEvent;
        foreach (Trick trick in m_tricks) {
            trick.OnTrickCompleted -= OnTrickCompleted;
        }
    }
}