using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoButton : MonoBehaviour
{
    public string videoClipName;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => LoadVideoScene());
    }

    private void LoadVideoScene()
    {
        LoadSceneManager.Instance.LoadSceneWithVideo(videoClipName);
        PlayerPrefs.SetString("Selected", videoClipName);
    }
}
