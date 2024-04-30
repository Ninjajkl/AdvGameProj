using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Make sure you're scenes are in order for these!

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;
    }

    public void LoadBunkerScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadEndScene()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }
}
