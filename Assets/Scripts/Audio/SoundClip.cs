using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClip : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_source;

    [SerializeField]
    private bool m_detachOnPlay = false;

    [SerializeField]
    private bool m_DestroyOnEnd = true;



    [SerializeField]
    private float m_minPitch = 1;
    [SerializeField]
    private float m_maxPitch = 1;

    public void Play()
    {
        m_source.pitch = Random.Range(m_minPitch, m_maxPitch);
        m_source.Play();

        if (m_detachOnPlay)
            transform.parent = null;

        if (m_DestroyOnEnd)
            StartCoroutine(DestroyOnEnd());
    }

    private IEnumerator DestroyOnEnd()
    {
        while (m_source.isPlaying) {
            yield return null;
        }

        Destroy(gameObject);

        yield return null;
    }
}
