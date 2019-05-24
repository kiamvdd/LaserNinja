using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField]
    private LevelOrder m_levelOrder;
    [SerializeField]
    private LevelItem m_itemPrefab;
    [SerializeField]
    private FadeOutButtonScrollList m_list;

    private void Awake()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        m_list.OnActiveItemSelected += LoadLevel;

        LevelData data = SerializationIOHandler.Load<LevelData>("Leveldata.bin");

        if (data == null)
        {
            data = new LevelData();
            data.CurrentLevel = 0;
            data.LevelTimes = new List<float>(m_levelOrder.LevelCount);

            for (int i = 0; i < m_levelOrder.LevelCount; i++)
            {
                data.LevelTimes.Add(0);
            }

            SerializationIOHandler.Save<LevelData>("Leveldata.bin", data);
        }

        for (int i = 0; i < Mathf.Min(data.CurrentLevel + 1, m_levelOrder.LevelCount); i++)
        {
            LevelItem item = Instantiate(m_itemPrefab, m_list.transform);
            item.transform.SetAsLastSibling();
            item.Init("LEVEL " + (i + 1), data.LevelTimes[i]);
        }

        // Forcing unity UI to update doesn't seem to actually ensure an update within the frame so here's the awful workaround
        m_list.Init();
        yield return new WaitForSeconds(0.1f);
        m_list.SetActiveIndex(Mathf.Min(data.CurrentLevel, m_levelOrder.LevelCount - 1));
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        m_list.OnActiveItemSelected -= LoadLevel;
    }

    private void LoadLevel(int index)
    {
        GameObject music = GameObject.FindGameObjectWithTag("Music");
        AudioSource source = music.GetComponent<AudioSource>();
        source.Play();

        SceneManager.LoadScene(m_levelOrder.GetLevelNameAt(index));
    }
}