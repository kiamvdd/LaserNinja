using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FadeOutButtonScrollList : MonoBehaviour, IScrollHandler
{
    private List<Tuple<CanvasGroup, CustomButton>> m_listItems = new List<Tuple<CanvasGroup, CustomButton>>();

    public Action<int> OnActiveItemSelected;

    [SerializeField]
    private Transform m_targetTransform;

    [SerializeField]
    private int m_visibilityRange = 2;

    public int ActiveIndex { get; private set; } = 0;

    public void Init()
    {
        CanvasGroup[] groups = GetComponentsInChildren<CanvasGroup>();

        // only get direct children
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i].transform.parent == transform)
            {
                FadeOutScrollHelper helper = groups[i].GetComponent<FadeOutScrollHelper>();
                if (helper != null)
                    helper.FadeOutList = this;

                CustomButton button = groups[i].GetComponentInChildren<CustomButton>();

                m_listItems.Add(new Tuple<CanvasGroup, CustomButton>(groups[i], button));
            }
        }

        foreach (Tuple<CanvasGroup, CustomButton> item in m_listItems)
        {
            item.Item1.alpha = 0;
        }

        LayoutGroup[] lgroups = GetComponentsInChildren<LayoutGroup>();
        for (int i = lgroups.Length - 1; i >= 0; i--)
        {
            lgroups[i].CalculateLayoutInputHorizontal();
            lgroups[i].CalculateLayoutInputVertical();
            lgroups[i].SetLayoutHorizontal();
            lgroups[i].SetLayoutVertical();
        }

        LayoutGroup lg = GetComponent<LayoutGroup>();
        lg.CalculateLayoutInputHorizontal();
        lg.CalculateLayoutInputVertical();
        lg.SetLayoutHorizontal();
        lg.SetLayoutVertical();


        UpdateChildTransparency();
        UpdatePosition();
    }

    public void Refresh()
    {
        UpdateChildTransparency();
        UpdatePosition();
    }

    public void ReceiveChildInput(Transform child)
    {
        int id = child.GetSiblingIndex();
        if (id == ActiveIndex)
        {
            OnActiveItemSelected(ActiveIndex);
        }
        else
        {
            ActiveIndex = id;
            UpdateChildTransparency();
            UpdatePosition();
        }
    }

    public void SetActiveIndex(int index)
    {
        ActiveIndex = index;
        UpdatePosition();
        UpdateChildTransparency();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ScrollDown();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ScrollUp();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnActiveItemSelected(ActiveIndex);
        }
    }
        
    private void UpdatePosition()
    {
        Transform activeChild = transform.GetChild(ActiveIndex);
        transform.position += ( m_targetTransform.position - activeChild.position );
    }

    private void UpdateChildTransparency()
    {
        for (int i = 0; i < m_listItems.Count; i++)
        {
            float dist = Mathf.Abs(ActiveIndex - i);
            float opacity = Mathf.Max(( ( m_visibilityRange + 1 ) - dist ) / ( m_visibilityRange + 1 ), 0);
            m_listItems[i].Item1.alpha = opacity;
        }
    }

    private void ScrollDown()
    {
        if (ActiveIndex == m_listItems.Count - 1)
            return;

        m_listItems[ActiveIndex].Item2.OnPointerExitEvent.Invoke();
        ActiveIndex++;
        m_listItems[ActiveIndex].Item2.OnPointerEnterEvent.Invoke();
        UpdatePosition();
        UpdateChildTransparency();
    }

    private void ScrollUp()
    {
        if (ActiveIndex == 0)
            return;
        m_listItems[ActiveIndex].Item2.OnPointerExitEvent.Invoke();
        ActiveIndex--;
        m_listItems[ActiveIndex].Item2.OnPointerEnterEvent.Invoke();
        UpdatePosition();
        UpdateChildTransparency();
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (eventData.scrollDelta.y < 0)
        {
            ScrollDown();
        }
        else if (eventData.scrollDelta.y > 0)
        {
            ScrollUp();
        }
    }
}