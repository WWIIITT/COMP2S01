using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

	private List<Joycon> joycons;

	// Values made available via Unity
	public float[] stick;
	public Vector3 gyro;
	public Vector3 accel;
	public int jc_ind = 0;
	public Quaternion orientation;

	// Camera
	public Camera playerCam;
	public Slider rotationSlider;
	public Text cameraSenText;
	public float rotationSpeed = 100.0f; // Control the speed of the rotation

	// Slider
	public Slider swingSlider;
	public Text swingSenText;

	// Video player
	public VideoPlayer videoPlayer;

	// Gyro threshold settings
	private float gyroThreshold = 1.0f; // Threshold for detecting significant movement
	private float belowThresholdTime = 0; // Timer for tracking low gyro readings
	public float requiredTimeBelowThreshold = 2.0f; // Time required below threshold to pause video

	// Advanced low-pass filter settings
	private Vector3 filteredGyro = Vector3.zero;
	public float smoothFactor = 0.1f; // Smoother factor for heavier smoothing

	// Debounce settings
	private bool isPausedDueToLowActivity = false;
	public float debounceDuration = 1.0f; // Time to ignore low spikes before pausing

	// Step counter variables
	private int stepCount = 0;
	private float lastAccelY = 0.0f;
	private bool stepDetected = false;
	public float stepThreshold = 1.2f; // Adjust this based on testing
	public float stepCooldown = 0.5f;  // Minimum time between steps (seconds)
	private float lastStepTime = 0f;
	private float minAccelerationChange = 0.3f; // Minimum required change in acceleration to detect movement
	public Text stepCountText;
	public Text finalStepCountText;

	void Start()
	{
		if (LoadSceneManager.Instance != null)
		{
			LoadVideoClip(LoadSceneManager.Instance.VideoClipName);
		}

		Debug.Log(rotationSpeed);

		gyro = new Vector3(0, 0, 0);
		accel = new Vector3(0, 0, 0);
		// get the public Joycon array attached to the JoyconManager in scene
		joycons = JoyconManager.Instance.j;
		if (joycons.Count < jc_ind + 1)
		{
			//Destroy(gameObject);
		}

		// Ensure videoPlayer is assigned
		if (videoPlayer == null)
		{
			Debug.LogError("VideoPlayer is not assigned.");
			this.enabled = false; // Disable script if no VideoPlayer is assigned
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (joycons != null)
		{
			// make sure the Joycon only gets checked if attached
			if (joycons.Count > 0)
			{
				Joycon j = joycons[jc_ind];
				stick = j.GetStick();
				if (playerCam != null) {
					ControlAngle();
				}

				// Gyro values: x, y, z axis values (in radians per second)
				gyro = j.GetGyro();

				// Accel values:  x, y, z axis values (in Gs)
				accel = j.GetAccel();
				filteredGyro = Vector3.Lerp(filteredGyro, gyro, smoothFactor);
				//Debug.Log("Filtered Gyro Magnitude: " + filteredGyro.magnitude);

				if (filteredGyro.magnitude > gyroThreshold)
				{
					belowThresholdTime = 0;
					isPausedDueToLowActivity = false;
					if (!videoPlayer.isPlaying)
						videoPlayer.Play();
				}
				else
				{
					if (!isPausedDueToLowActivity)
					{
						belowThresholdTime += Time.deltaTime;
						if (belowThresholdTime >= requiredTimeBelowThreshold)
						{
							videoPlayer.Pause();
							isPausedDueToLowActivity = true;
							// Reset the belowThresholdTime to start the debounce timer
							belowThresholdTime = 0;
						}
					}
					else
					{
						// If paused, wait for debounce duration before considering resuming
						if (belowThresholdTime < debounceDuration)
						{
							belowThresholdTime += Time.deltaTime;
						}
						else
						{
							isPausedDueToLowActivity = false; // Reset after debounce period
						}
					}
				}
			}
			// --- Step Counting Logic ---
			DetectStep(accel.x);
		}
	}

	void ControlAngle()
	{
		// Assuming stick[0] is for horizontal and stick[1] for vertical inputs
		float yaw = stick[0] * rotationSpeed * Time.deltaTime; // Calculate rotation around y-axis
		float pitch = -stick[1] * rotationSpeed * Time.deltaTime; // Calculate rotation around x-axis

		// Current rotation plus the new input-based rotation
		Vector3 newRotation = playerCam.transform.eulerAngles + new Vector3(pitch, yaw, 0);

		// Applying the rotation to the camera
		playerCam.transform.eulerAngles = newRotation;
	}

	void LoadVideoClip(string clipName)
	{
		VideoClip clipToPlay = Resources.Load<VideoClip>("VideoClips/" + clipName);
		if (clipToPlay != null)
		{
			videoPlayer.clip = clipToPlay;
			videoPlayer.Play();
		}
		else
		{
			Debug.LogError("Video clip not found: " + clipName);
		}
	}

	public void OnSliderValueChanged(float value)
	{
		rotationSpeed = value;
		if (cameraSenText != null)
		{
			cameraSenText.text = Mathf.RoundToInt(value).ToString();
		}
		Debug.Log("Updated rotation speed to: " + rotationSpeed);
	}

	public void OnSwingSliderValueChanged(float sliderValue)
	{
		// Map the slider value (50 to 150) to a range of 0.5 to 1.5
		gyroThreshold = MapValue(sliderValue, 50, 150, 0.5f, 1.5f);
		if (swingSenText != null)
		{
			swingSenText.text = Mathf.RoundToInt(sliderValue).ToString();
		}
		Debug.Log("Actual value is now: " + gyroThreshold);
	}

	// Map a value from one range to another
	private float MapValue(float value, float fromSource, float toSource, float fromTarget, float toTarget)
	{
		return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
	}

	// Step Detection Function
	void DetectStep(float currentAccelY)
	{
		float timeSinceLastStep = Time.time - lastStepTime;

		// Detect small but periodic movements
		if (!stepDetected && Mathf.Abs(currentAccelY - lastAccelY) > minAccelerationChange)
		{
			if (currentAccelY > stepThreshold && timeSinceLastStep > stepCooldown)
			{
				stepCount++;
				lastStepTime = Time.time;
				UpdateStepUI();
				stepDetected = true;
			}
		}

		// Reset detection when acceleration stabilizes
		if (Mathf.Abs(currentAccelY - lastAccelY) < minAccelerationChange * 0.5f)
		{
			stepDetected = false;
		}

		lastAccelY = currentAccelY; // Store last frame's acceleration
	}

	// Update UI function
	void UpdateStepUI()
	{
		if (stepCountText != null)
		{
			stepCountText.text = "步數: " + stepCount;
		}

		if (finalStepCountText != null)
		{
			finalStepCountText.text = stepCountText.text;
		}
		Debug.Log("步數: " + stepCount);
	}
}
