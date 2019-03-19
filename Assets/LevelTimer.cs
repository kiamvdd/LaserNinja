﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTimer : MonoBehaviour
{
    [SerializeField]
    private float m_startTime = 10;

    [SerializeField]
    private TextMeshProUGUI m_text;

    [SerializeField]
    private GameObject m_gameOverText;
    [SerializeField]
    private GameObject m_winText;
    [SerializeField]
    private GameObject m_anyKeyText;

    [SerializeField]
    private PlayerCharacter m_player;

    [SerializeField]
    private SoundClip m_music;

    private float m_timer = 0;

    private bool m_levelEnd = false;

    private bool m_timerActive = false;

    private void Awake()
    {
        m_timer = m_startTime;
        TimeSpan ts = TimeSpan.FromSeconds(m_timer);
        m_text.text = ts.ToString(@"mm\:ss\:ff");
    }

    public void EndLevel(bool win)
    {
        if (m_levelEnd)
            return;

        if (win) {
            enabled = false;
            m_winText.SetActive(true);
            m_levelEnd = true;
        } else {
            m_timer = 0;
            m_text.enabled = false;
            m_gameOverText.SetActive(true);
            m_levelEnd = true;
        }
    }

    private void Update()
    {
        if (!m_timerActive) {
            if (Input.anyKeyDown) {
                m_timerActive = true;
                m_anyKeyText.SetActive(false);
                m_music.Play();
            }

            return;
        }

        if (m_levelEnd) {
            m_timer += Time.deltaTime;

            if (m_timer > 2)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        m_timer -= Time.deltaTime;
        TimeSpan ts = TimeSpan.FromSeconds(m_timer);

        m_text.text = ts.ToString(@"mm\:ss\:ff");

        if (m_timer <= 0 && !m_levelEnd) {
            m_player.Destroy();
        }
    }

    public void AddTime(float time)
    {
        m_timer += time;
        StartCoroutine(FlashTimer());
    }

    private IEnumerator FlashTimer()
    {
        float startScale = 1.2f;
        float timer = 0;

        while (timer < 1) {
            timer += Time.deltaTime;
            float currentScale = Mathf.Lerp(startScale, 1, timer);
            m_text.gameObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            yield return null;
        }

        m_text.gameObject.transform.localScale = Vector3.one;

        yield return null;
    }
}
