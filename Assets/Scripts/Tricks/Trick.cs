using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using OdinSerializer;

[CreateAssetMenu(menuName = "LightBounce/Trick")]
[Serializable]
public class Trick : ScriptableObject, ISerializationCallbackReceiver
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
    private TrickSequenceParser.ParserType m_parserType = TrickSequenceParser.ParserType.LINEAR;

    [SerializeField]
    private TrickSequenceParser m_odinParserTemplate = new LinearTrickParser();
    public TrickSequenceParser ParserTemplate { get { return m_odinParserTemplate; } }
    private List<TrickSequenceParser> m_activeParsers = new List<TrickSequenceParser>();

    [HideInInspector]
    [SerializeField]
    private byte[] m_odinBytes;

    public delegate void TrickCallBack(Trick trick);
    public event TrickCallBack OnTrickCompleted;

    public void ProcessTrickEvent(TrickEventData eventData)
    {
        // jump received
        // if no parsers or concurrency == unlimited, add new parser
        if (m_activeParsers.Count == 0 || m_concurrencyType == Concurrency.UNLIMITED) {
            // copy template values into new instance
            TrickSequenceParser newParser = m_odinParserTemplate.Instantiate();
            // add to active parser list
            m_activeParsers.Add(newParser);
        }

        // One event should only cause one callback so once a parser has completed, 
        // if any other parsers complete on the same event they get removed
        // this might need a refactor eventually

        // for each active parser
        for (int i = m_activeParsers.Count - 1; i >= 0; i--) {
            // send jump to parser
            TrickSequenceParser.SequenceState sequenceState = m_activeParsers[i].ProcessTrickEvent(eventData);
            // parser state is set in ProcessTrickEvent
            TrickSequenceParser.ParserState parserState = m_activeParsers[i].State;

            if (parserState == TrickSequenceParser.ParserState.EXIT || sequenceState == TrickSequenceParser.SequenceState.FAIL) {
                m_activeParsers.RemoveAt(i);
            }

            if (sequenceState == TrickSequenceParser.SequenceState.SUCCESS) {
                OnTrickCompleted(this);
            }
        }
    }

#if UNITY_EDITOR && !FAKE_BUILD
    public void OnValidate()
    {
        if (m_odinParserTemplate == null || m_odinParserTemplate.GetParserType() != m_parserType) {
            switch (m_parserType) {
                case TrickSequenceParser.ParserType.LINEAR:
                    m_odinParserTemplate = new LinearTrickParser();
                    break;
                case TrickSequenceParser.ParserType.ACHRONOLOGICAL:
                    m_odinParserTemplate = new AchronologicalParser();
                    break;
            }
        }

        m_odinParserTemplate.OnValidate();
    }
#endif

    public void OnBeforeSerialize()
    {
        m_odinBytes = SerializationUtility.SerializeValue<TrickSequenceParser>(m_odinParserTemplate, DataFormat.Binary);
    }

    public void OnAfterDeserialize()
    {
        m_odinParserTemplate = SerializationUtility.DeserializeValue<TrickSequenceParser>(m_odinBytes, DataFormat.Binary);
    }
}//