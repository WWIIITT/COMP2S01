using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoRouteController : MonoBehaviour
{
    public string video;
    public VideoPlayer videoPlayer;
    public Button routeAButton;
    public Button routeBButton;
    public GameObject choicePanel; // UI panel for route selection
    public int debugStartTime;
    private bool waitingForChoice = false;
    private bool choiceMade = false;
    private bool choiceMadeP1 = false;

    // Point 1
    private double P1_decisionTime = 900; // start time
    private double P1_decisionTimeB = 1245; // start time for B
    private double Point1_routeAEnd = 1245;   // A ends
    private double convergeTimeA = 1245; // end time
    private bool P1_EnteredA = false;
    private bool P1_EnteredB = false;
    private bool lockP1 = false;

    void Start()
    {
        video = PlayerPrefs.GetString("Selected", "Default");
        // Hide choice UI initially
        choicePanel.SetActive(false);

        // Assign button listeners
        routeAButton.onClick.AddListener(ChoosePoint1_routeA);
        routeBButton.onClick.AddListener(ChoosePoint1_routeB);
        videoPlayer.time = debugStartTime;
        // Start video playback
        videoPlayer.Play();

    }

    void Update()
    {
        if(video == "1"){
            // Check if video reaches decision point (3:41)
            if (!waitingForChoice && !choiceMadeP1 && videoPlayer.time >= P1_decisionTime && (!P1_EnteredA && !P1_EnteredB))
            {
                videoPlayer.Pause();
                choicePanel.SetActive(true); // Show route selection UI
                waitingForChoice = true;
            }
            
            // for debug use, fast forward from P1 to P2
            // if(videoPlayer.time >= 1090 && videoPlayer.time <= 1095) videoPlayer.time = 1260; 

            // if(P1_EnteredA && videoPlayer.time >= 967 && !jump) // For A part 1 to A part 2
            // {
            //     jump = true;
            //     videoPlayer.time = 1053;
            // }
            if (P1_EnteredA && choiceMadeP1 && videoPlayer.time >= Point1_routeAEnd && !lockP1)
            {
                videoPlayer.time = convergeTimeA;
                lockP1 = true;
            }
        }

    }
    //////////////////////////////////////////////////////////////////////////////////
    void ChoosePoint1_routeA()
    {
        P1_EnteredA = true;
        choiceMadeP1 = true;
        waitingForChoice = false;
        choicePanel.SetActive(false);
        videoPlayer.time = P1_decisionTime; 
        videoPlayer.Play();
    }

    void ChoosePoint1_routeB()
    {
        P1_EnteredB = true;
        choiceMadeP1 = true;
        waitingForChoice = false;
        choicePanel.SetActive(false);
        videoPlayer.time = P1_decisionTimeB;
        videoPlayer.Play();
        // debug use
        // videoPlayer.time = 1043;  
    }
}