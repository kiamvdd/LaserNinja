using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CustomButton : PointerInputHandler
{
    [FormerlySerializedAs("m_onPointerDown")]
    public UnityEvent OnPointerDownEvent;
    [FormerlySerializedAs("m_onPointerUp")]
    public UnityEvent OnPointerUpEvent;
    [FormerlySerializedAs("m_onPointerEnter")]
    public UnityEvent OnPointerEnterEvent;
    [FormerlySerializedAs("m_onPointerExit")]
    public UnityEvent OnPointerExitEvent;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnPointerDownEvent.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        OnPointerUpEvent.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        OnPointerEnterEvent.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        OnPointerExitEvent.Invoke();
    }
}
