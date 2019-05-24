using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TMProColorButtonExtension : PointerInputHandler
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_text;
    [SerializeField]
    private Color m_defaultColor;
    [SerializeField]
    private Color m_clickedColor;
    [SerializeField]
    private Color m_hoverColor;

    private void OnEnable()
    {
        m_text.color = m_defaultColor;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        m_text.color = m_hoverColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        m_text.color = m_defaultColor;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        m_text.color = m_clickedColor;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        m_text.color = PointerStay ? m_hoverColor : m_defaultColor;
    }
}
