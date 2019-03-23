using System.Collections;
using TMPro;
using UnityEngine;
public class TrickText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_text;


    [SerializeField]
    private float m_opaqueTime = 2;

    [SerializeField]
    private float m_fadeTime = 2;

    public void Init(string text)
    {
        m_text.text = text;
        StartCoroutine(FadeAndDestroy(m_opaqueTime, m_fadeTime));
    }

    private IEnumerator FadeAndDestroy(float opaqueSeconds, float fadeSeconds)
    {
        float timer = 0;
        while (timer < opaqueSeconds) {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        while (timer < fadeSeconds) {
            timer += Time.deltaTime;
            Color colour = m_text.color;
            colour.a = 1 - (timer / fadeSeconds);
            m_text.color = colour;

            yield return null;
        }

        Destroy(gameObject);
        yield return null;
    }
}
