using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [Header("Video Components")]
    public VideoPlayer videoPlayer; // Assign your VideoPlayer here

    [Header("UI Components")]
    public Slider progressSlider;   // Slider to control video progress
    public Button playButton;
    public Button pauseButton;
    public Button restartButton;    // Restart button

    private bool isDraggingSlider = false;

    void Start()
    {
        if (videoPlayer.clip != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = (float)videoPlayer.clip.length;
        }

        progressSlider.onValueChanged.AddListener(OnProgressSliderChanged);
        playButton.onClick.AddListener(OnPlayButtonClicked);
        pauseButton.onClick.AddListener(OnPauseButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    void Update()
    {
        if (!isDraggingSlider && videoPlayer.isPlaying)
        {
            progressSlider.value = (float)videoPlayer.time;
        }
    }

    public void OnProgressSliderChanged(float value)
    {
        videoPlayer.time = value;
    }

    public void OnBeginDragProgress()
    {
        isDraggingSlider = true;
    }

    public void OnEndDragProgress()
    {
        isDraggingSlider = false;
        videoPlayer.time = progressSlider.value;
    }

    public void OnPlayButtonClicked()
    {
        videoPlayer.Play();
    }

    public void OnPauseButtonClicked()
    {
        videoPlayer.Pause();
    }

    public void OnRestartButtonClicked()
    {
        videoPlayer.Stop();
        videoPlayer.Play(); // Remove Play() if you prefer the video to remain paused at the beginning
    }
}
