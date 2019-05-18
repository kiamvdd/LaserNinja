using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_layerMask;

    [SerializeField]
    private UnityEvent m_onEnter;
    [SerializeField]
    private UnityEvent m_onExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (( m_layerMask.value & ( 1 << collision.gameObject.layer ) ) == 0)
            return;
        m_onEnter.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (( m_layerMask.value & ( 1 << collision.gameObject.layer ) ) == 0)
            return;
        m_onExit.Invoke();
    }
}
