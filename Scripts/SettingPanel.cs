using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SettingPanel : MonoBehaviour
{
    public GameObject settingPanel;
    public VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePanel()
    {
        if(settingPanel.activeSelf)
        {
            videoPlayer.Play();
            settingPanel.SetActive(false);
        } else
        {
            videoPlayer.Pause();
            settingPanel.SetActive(true);
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
