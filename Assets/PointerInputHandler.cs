using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class PointerInputHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    public bool PointerStay { get; private set; }

    public virtual void OnPointerDown(PointerEventData eventData) { }
    public virtual void OnPointerUp(PointerEventData eventData) { }
    public virtual void OnPointerEnter(PointerEventData eventData) { PointerStay = true; }
    public virtual void OnPointerExit(PointerEventData eventData) { PointerStay = false; }
}
