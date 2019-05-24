using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverShaker : PointerInputHandler
{
    [SerializeField]
    private Vector2 m_shakeRange;
    [SerializeField]
    private float m_shakeInterval;

    private Vector3 m_startingPos = Vector3.zero;

    private float m_timer = 0;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (m_startingPos == Vector3.zero)
            m_startingPos = transform.position;

        m_timer = 0;
        SetRandomOffset();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        transform.position = m_startingPos;
    }

    void Update()
    {
        if (!PointerStay)
            return;

        m_timer += Time.deltaTime;

        if (m_timer > m_shakeInterval)
        {
            SetRandomOffset();
            m_timer = 0;
        }
    }

    private void SetRandomOffset()
    {
        Vector3 offset = Vector3.zero;
        offset.x = Random.Range(-m_shakeRange.x, m_shakeRange.x);
        offset.y = Random.Range(-m_shakeRange.y, m_shakeRange.y);

        transform.position = m_startingPos + offset;
    }
}
