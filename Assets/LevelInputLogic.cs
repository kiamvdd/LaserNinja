using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInputLogic : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject music = GameObject.FindGameObjectWithTag("Music");
            AudioSource source = music.GetComponent<AudioSource>();
            source.Stop();

            SceneManager.LoadScene("Menu");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
