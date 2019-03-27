using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LightBounce/Trick")]
public class Trick : ScriptableObject
{
    private enum Concurrency
    {
        SINGLE,
        UNLIMITED,
    }

    [SerializeField]
    private string m_name = "New Trick";
    public string Name { get { return m_name; } }

    [SerializeField]
    private Concurrency m_concurrencyType = Concurrency.SINGLE;

    [SerializeField]
    private float m_timeBonus = 0f;
    public float TimeBonus { get { return m_timeBonus; } }

    [SerializeField]
    private TrickSequenceParser m_parserTemplate;
    public TrickSequenceParser ParserTemplate { get { return m_parserTemplate; } }
    private List<TrickSequenceParser> m_activeParsers = new List<TrickSequenceParser>();

    public delegate void TrickCallBack(Trick trick);
    public event TrickCallBack OnTrickCompleted;

    public void ProcessTrickEvent(TrickEventData eventData)
    {
        if (m_activeParsers.Count == 0 || m_concurrencyType == Concurrency.UNLIMITED) {
            TrickSequenceParser newParser = m_parserTemplate.Instantiate();
            m_activeParsers.Add(m_parserTemplate.Instantiate());
        }

        for (int i = m_activeParsers.Count - 1; i >= 0; i--) {
            TrickSequenceParser.SequenceState sequenceState = m_activeParsers[i].ProcessTrickEvent(eventData);
            TrickSequenceParser.ParserState parserState = m_activeParsers[i].State;

            if (parserState == TrickSequenceParser.ParserState.EXIT || sequenceState == TrickSequenceParser.SequenceState.FAIL) {
                m_activeParsers.RemoveAt(i);
            }

            if (sequenceState == TrickSequenceParser.SequenceState.SUCCESS) {
                OnTrickCompleted(this);
            }
        }
    }
}