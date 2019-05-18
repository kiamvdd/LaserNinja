using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickTrigger : MonoBehaviour
{
    [SerializeField]
    private Trick m_trick;
    private TrickInterpreter m_trickInterpreter;

    [SerializeField]
    private float m_minTriggerInterval = 0.5f;
    private float m_lastTriggerTime = 0;

    private void Awake()
    {
        m_trickInterpreter = FindObjectOfType<TrickInterpreter>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Time.realtimeSinceStartup - m_lastTriggerTime > m_minTriggerInterval)
        {
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();
            if (player != null && player.IsAlive())
            {
                m_trickInterpreter.OnTrickCompleted(m_trick);
                m_lastTriggerTime = Time.realtimeSinceStartup;
            }
        }
        
    }
}
