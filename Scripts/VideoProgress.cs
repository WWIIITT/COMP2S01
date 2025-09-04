using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoProgress : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider progressBar;
    public GameObject endPanel; // Reference to the panel you want to display when video ends
    public Text kilometerText; // UI Text to display the kilometer value
    public Text finalKilometerText;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Mute all audio tracks
        for (ushort i = 0; i < videoPlayer.audioTrackCount; i++)
        {
            videoPlayer.SetDirectAudioMute(i, true);
        }

        // Optional: Add a listener to update the progress bar each time the video frame changes.
        videoPlayer.frameReady += UpdateProgressBar;

        // Add a listener for the loopPointReached event
        videoPlayer.loopPointReached += OnVideoEnded;
    }

    void Update()
    {
        if (videoPlayer.isPlaying)
        {
            UpdateProgressBar(videoPlayer, videoPlayer.frame);
            UpdateKilometerText();
        }
    }

    void UpdateProgressBar(VideoPlayer source, long frameIdx)
    {
        if (videoPlayer.frameCount > 0)
        {
            progressBar.value = (float)(videoPlayer.time / videoPlayer.length);
        }
    }

    // Function to be called when the video finishes
    void OnVideoEnded(VideoPlayer vp)
    {
        finalKilometerText.text = kilometerText.text;
        endPanel.SetActive(true); // Activate the panel
    }

    // Update kilometer text based on the video playtime
    void UpdateKilometerText()
    {
        float kilometers = (float)videoPlayer.time / 1000f; // Calculate the distance
        kilometerText.text = $"公里 : {kilometers:0.000}"; // Display it with two decimal places
    }
}
