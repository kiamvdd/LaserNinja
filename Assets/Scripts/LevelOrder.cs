using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "LightBounce/Level Order")]
[Serializable]
public class LevelOrder : ScriptableObject
{
    [SerializeField]
    private string[] m_sceneNames;

    public int LevelCount { get { return m_sceneNames.Length; } }

    public int GetCurrentLevelIndex()
    {
        string currentLevelName = SceneManager.GetActiveScene().name.ToLower();
        for (int i = 0; i < m_sceneNames.Length; i++)
        {
            if (m_sceneNames[i].ToLower() == currentLevelName)
            {
                return i;
            }
        }

        return -1;
    }

    public string GetNextLevelName()
    {
        string currentLevelName = SceneManager.GetActiveScene().name.ToLower();
        for (int i = 0; i < m_sceneNames.Length - 1; i++)
        {
            if (m_sceneNames[i].ToLower() == currentLevelName)
            {
                return m_sceneNames[i + 1];
            }
        }

        return null;
    }

    public string GetLevelNameAt(int index)
    {
        return m_sceneNames[index];
    }
}
