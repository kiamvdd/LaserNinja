using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelItem : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_levelText;
    [SerializeField]
    private TMPro.TextMeshProUGUI m_timeText;

    public void Init(string levelName, float levelTime)
    {
        m_levelText.text = levelName;

        TimeSpan ts = TimeSpan.FromSeconds(levelTime);
        m_timeText.text = "BEST TIME: " + ts.ToString(@"mm\:ss\:ff");
    }
}
