using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance { get; private set; }

    // Parameter example
    public string VideoClipName { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the manager alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSceneWithVideo(string videoClipName)
    {
        VideoClipName = videoClipName;
        SceneManager.LoadScene("GameScene");
    }
}
