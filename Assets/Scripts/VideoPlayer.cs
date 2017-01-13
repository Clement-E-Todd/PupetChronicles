using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VideoPlayer : MonoBehaviour
{
    public static VideoPlayer instance;

    public Button playButton;
    public Button slowButton;
    public Button fastButton;
    public Button fullscreenButton;

    public Text titleText;
    public Text loadingText;
    public Text creditsText;

    RawImage image;

    new AudioSource audio;

    MovieTexture _video;
    MovieTexture video
    {
        get
        {
            return _video;
        }

        set
        {
            _video = value;
            image.texture = _video;
            audio.clip = _video.audioClip;
        }
    }

    float[] possiblePlaybackSpeeds = { 0.25f, 0.5f, 0.75f, 1f, 1.5f, 2f, 3f };
    int playbackSpeedIndex = 3;
    public bool isPlaying { get { return audio.isPlaying; } }
    public float playbackSpeed { get { return possiblePlaybackSpeeds[playbackSpeedIndex]; } }
    public float playbackTime = 0f;
    public float totalPlaybackTime { get { return (audio && audio.clip) ? video.duration : 0f; } }

    Sprite playButtonSprite;
    Sprite pauseButtonSprite;

    int windowedWidth = 640;
    int windowedHeight = 360;

	void Awake()
    {
        instance = this;

        image = GetComponent<RawImage>();
        audio = GetComponent<AudioSource>();

        playButtonSprite = Resources.Load<Sprite>("Textures/Play");
        pauseButtonSprite = Resources.Load<Sprite>("Textures/Pause");
    }

    void Update()
    {
        if (video && video.isPlaying)
        {
            playbackTime += Time.deltaTime * audio.pitch;
        }
    }

    public void PlayPauseToggle()
    {
        if (!video.isPlaying)
        {
            if (!image.enabled)
            {
                image.enabled = true;
                UpdateSlowAndFastButtons();
                loadingText.text = string.Empty;
            }

            StoryManager.instance.shouldCheckForEndOfClip = true;
            video.Play();
            audio.Play();
            playButton.image.sprite = pauseButtonSprite;

            titleText.gameObject.SetActive(false);
        }
        else
        {
            StoryManager.instance.shouldCheckForEndOfClip = false;
            video.Pause();
            audio.Pause();
            playButton.image.sprite = playButtonSprite;
        }
    }

    public void SlowDown()
    {
        if (playbackSpeedIndex > 0)
        {
            playbackSpeedIndex--;
            audio.pitch = playbackSpeed;
        }

        UpdateSlowAndFastButtons();
    }

    public void SpeedUp()
    {
        if (playbackSpeedIndex < possiblePlaybackSpeeds.Length - 1)
        {
            playbackSpeedIndex++;
            audio.pitch = playbackSpeed;
        }

        UpdateSlowAndFastButtons();
    }

    void UpdateSlowAndFastButtons()
    {
        slowButton.interactable = (playbackSpeedIndex > 0);
        fastButton.interactable = (playbackSpeedIndex < possiblePlaybackSpeeds.Length - 1);
    }

    public void ToggleFullscreen()
    {
        if (!Screen.fullScreen)
        {
            windowedWidth = Screen.width;
            windowedHeight = Screen.height;
            Screen.SetResolution(1280, 720, true);
        }
        else
        {
            Screen.SetResolution(windowedWidth, windowedHeight, false);
        }
    }

    public void PauseToLoadCurrentClip()
    {
        if (video && audio)
        {
            StoryManager.instance.shouldCheckForEndOfClip = false;
            video.Pause();
            audio.Pause();
        }

        playButton.interactable = false;
        slowButton.interactable = false;
        fastButton.interactable = false;
    }

    public void SetCurrentVideo(MovieTexture newVideo, bool forcePlay)
    {
        bool wasPlaying = video && video.isPlaying;
        
        video = newVideo;
        audio.clip = video.audioClip;
        playbackTime = 0f;

        playButton.interactable = true;

        if (!image.enabled)
        {
            loadingText.text = "Press ► to start!";
        }
        else
        {
            loadingText.text = string.Empty;

            if (wasPlaying || forcePlay)
            {
                video.Play();
                audio.Play();
            }
        }

        // If there is no next clip, treat this video as the credits video.
        if (StoryManager.instance.currentClip.nextClipA == null &&
            StoryManager.instance.currentClip.nextClipB == null &&
            StoryManager.instance.currentClip.nextClipNoInput == null)
        {
            image.rectTransform.sizeDelta = new Vector2(640, 360);
            image.rectTransform.anchoredPosition = new Vector3(-320, 154);
            creditsText.gameObject.SetActive(true);
            video.loop = true;

            StoryManager.instance.SaveStoryToSharedData();
        }
    }

    public void UpdateLoadingProgress(float progress)
    {
        loadingText.text = "Loading... (" + (int)(progress * 100) + "%)";
    }
}
