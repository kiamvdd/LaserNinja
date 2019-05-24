using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTimer : MonoBehaviour
{
    #region Members
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
    private TrickList m_trickList;

    [SerializeField]
    private PlayerCharacter m_player;

    private float m_timer = 0;

    private bool m_levelEnd = false;

    private bool m_timerActive = false;

    [SerializeField]
    private LevelOrder m_levelOrder;

    private float m_lowestTime = Mathf.Infinity;
    #endregion

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

        if (win)
        {
            SaveLevelData();

            enabled = false;
            m_winText.SetActive(true);
            m_levelEnd = true;

            string nextLevelName = m_levelOrder.GetNextLevelName();

            if (nextLevelName != null)
                StartCoroutine(LoadSceneAfterSeconds(nextLevelName, 2));
        }
        else
        {
            m_timer = 0;
            m_text.enabled = false;
            m_gameOverText.SetActive(true);
            m_levelEnd = true;
        }
    }

    private void SaveLevelData()
    {
        LevelData data = SerializationIOHandler.Load<LevelData>("Leveldata.bin");

        int levelId = m_levelOrder.GetCurrentLevelIndex();
        if (levelId < 0)
            return;

        if (levelId + 1 > data.CurrentLevel)
            data.CurrentLevel = levelId + 1;

        if (m_timer > data.LevelTimes[levelId])
            data.LevelTimes[levelId] = m_timer;

        SerializationIOHandler.Save<LevelData>("Leveldata.bin", data);
    }

    private IEnumerator LoadSceneAfterSeconds(string sceneName, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!m_timerActive)
        {
            if (Input.anyKeyDown)
            {
                m_timerActive = true;
                m_anyKeyText.SetActive(false);

                EventBus.OnTimerStart?.Invoke();
            }

            return;
        }

        if (m_levelEnd)
        {
            m_timer += Time.deltaTime;

            if (m_timer > 2)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        m_timer -= Time.deltaTime;

        if (m_timer < m_lowestTime)
            m_lowestTime = m_timer;

        TimeSpan ts = TimeSpan.FromSeconds(m_timer);
        m_text.text = ts.ToString(@"mm\:ss\:ff");

        if (m_timer <= 0 && !m_levelEnd)
        {
            m_player.Destroy();
        }
    }

    public void AddTimeFromTrick(Trick trick)
    {
        AddTime(trick.TimeBonus);
        m_trickList.AddTrick(trick);
    }

    private void AddTime(float time)
    {
        m_timer += time;
        StartCoroutine(FlashTimer());
    }

    private IEnumerator FlashTimer()
    {
        float startScale = 1.2f;
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime;
            float currentScale = Mathf.Lerp(startScale, 1, timer);
            m_text.gameObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            yield return null;
        }

        m_text.gameObject.transform.localScale = Vector3.one;

        yield return null;
    }
}
