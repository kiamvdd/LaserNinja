using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LayoutGroupRefresher : MonoBehaviour
{
    RectTransform m_layoutGroup;
    private void Awake()
    {
        m_layoutGroup = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (m_layoutGroup == null)
            return;

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_layoutGroup);
    }
}
