using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideUI : MonoBehaviour
{
    public float MinValue { get; set; } = 0;
    public float MaxValue { get; set; } = 1;

    [SerializeField]
    private SpriteRenderer m_renderer;

    [SerializeField]
    private Color m_minColor;

    [SerializeField]
    private Color m_maxColor;

    [SerializeField]
    private float m_fadeOutDuration = 2;

    private float m_val = 0;
    private float m_maxScale;

    private IEnumerator m_fadeOutCR = null;

    public float Value {
        get {
            return m_val;
        }

        set {
            m_val = Mathf.Clamp(value, MinValue, MaxValue);
            float lerp = ( m_val - MinValue ) / ( MaxValue - MinValue );
            Vector3 scale = transform.localScale;
            scale.y = Mathf.Lerp(0, m_maxScale, lerp);
            transform.localScale = scale;

            Color newColor = Color.Lerp(m_minColor, m_maxColor, lerp);
            newColor.a = m_renderer.color.a;
            m_renderer.color = newColor;
        }
    } 

    private void Awake()
    {
        m_maxScale = transform.localScale.y;
    }

    public void Show()
    {
        if (m_fadeOutCR != null)
            StopCoroutine(m_fadeOutCR);

        Color newColor = m_renderer.color;
        newColor.a = 1;
        m_renderer.color = newColor;
    }

    public void Hide()
    {
        m_fadeOutCR = FadeOut();
        StartCoroutine(m_fadeOutCR);
    }

    private IEnumerator FadeOut()
    {
        float timer = m_fadeOutDuration;

        Color newColor;
        while (timer > 0)
        {
            float lerp = timer / m_fadeOutDuration;

            newColor = m_renderer.color;
            newColor.a = lerp;
            m_renderer.color = newColor;

            timer -= Time.deltaTime;
            yield return null;
        }

        newColor = m_renderer.color;
        newColor.a = 0;
        m_renderer.color = newColor;

        m_fadeOutCR = null;
    }
}
