using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    public static StoryManager instance;

    public Button optionAButton;
    public Button optionBButton;

    public Text optionAText;
    public Text optionBText;

    public StoryClip previousClip;
    public StoryClip currentClip;
    bool currentClipLoaded = false;

    public bool shouldCheckForEndOfClip = false;
    
    public bool parfityWasDeterred;
    public bool ancestorsWereEvil;
    public bool joinedForcesWithProf;
    public bool yuDied;
    public bool profDied;
    public bool parfityDied;
    public bool metClasper;

    void Start()
    {
        instance = this;
        SetCurrentClip(new StoryClip01_Intro());
    }

    void Update()
    {
        // Update UI accordingly while the current clip is loading.
        if (!currentClipLoaded)
        {
            if (currentClip.resourceRequest.isDone)
            {
                SendCurrentClipToVideoPlayer(false);
                currentClipLoaded = true;
            }
            else
            {
                VideoPlayer.instance.UpdateLoadingProgress(currentClip.resourceRequest.progress);
            }
        }

        // Show and hide the narrative option buttons as needed.
        if (!currentClip.noDecisionInThisClip &&
            VideoPlayer.instance.playbackTime < currentClip.answerCutoffTime)
        {
            if (VideoPlayer.instance.playbackTime >= currentClip.optionATime)
            {
                optionAButton.gameObject.SetActive(true);
            }

            if (VideoPlayer.instance.playbackTime >= currentClip.optionBTime)
            {
                optionBButton.gameObject.SetActive(true);
            }
        }
        else
        {
            optionAButton.gameObject.SetActive(false);
            optionBButton.gameObject.SetActive(false);
        }

        // If an option has been selected and we have advanced enough in the clip, switch the clip.
        if (VideoPlayer.instance.isPlaying &&
            VideoPlayer.instance.playbackTime > currentClip.answerStartTime &&
            currentClip.decision != StoryClip.Decision.None)
        {
            // If there is a clip to be played, play it.
            if (currentClip.nextClipA != null ||
                currentClip.nextClipB != null ||
                currentClip.nextClipNoInput != null)
            {
                if (currentClip.decision == StoryClip.Decision.OptionA)
                {
                    if (currentClip.nextClipA != null)
                        SetCurrentClip(currentClip.nextClipA);
                    else
                        SetCurrentClip(currentClip.nextClipNoInput);
                }
                else if (currentClip.decision == StoryClip.Decision.OptionB)
                {
                    if (currentClip.nextClipB != null)
                        SetCurrentClip(currentClip.nextClipB);
                    else
                        SetCurrentClip(currentClip.nextClipNoInput);
                }
            }

            // If there is no next clip, restart the game.
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }

        // If we reach the end of the current clip, trigger its "no decision" follow-up.
        if (shouldCheckForEndOfClip && !VideoPlayer.instance.isPlaying)
        {
            SetCurrentClip(currentClip.nextClipNoInput, true);
        }
    }

    void SetCurrentClip(StoryClip clip, bool forcePlay = false)
    {
        if (clip == null)
        {
            Debug.LogError("New clip must not be null!");
        }

        previousClip = currentClip;
        currentClip = clip;

        // Perform any last-minute logic that the previous clip may need.
        if (previousClip != null)
        {
            previousClip.OnEndOfClip();
        }

        // Start preparing the next potential clips
        currentClip.SelectNextPotentialClips();
        currentClip.LoadNextPotentialVideos();

        // Handle the condition that the clip is not yet loaded.
        if (clip.resourceRequest == null)
        {
            currentClipLoaded = false;
            clip.LoadVideo();
        }

        if (!clip.resourceRequest.isDone)
        {
            VideoPlayer.instance.PauseToLoadCurrentClip();
        }
        else
        {
            SendCurrentClipToVideoPlayer(forcePlay);
            currentClipLoaded = true;
        }

        // Update the text on the narrative option buttons.
        optionAText.text = currentClip.optionAText;
        optionBText.text = currentClip.optionBText;
        optionAText.GetComponent<Outline>().enabled = false;
        optionBText.GetComponent<Outline>().enabled = false;

        // Make the narrative option buttons invisible until the choices are presented in the video.
        optionAButton.gameObject.SetActive(false);
        optionBButton.gameObject.SetActive(false);
    }

    void SendCurrentClipToVideoPlayer(bool forcePlay)
    {
        VideoPlayer.instance.SetCurrentVideo(currentClip.resourceRequest.asset as MovieTexture, forcePlay);

        if (previousClip != null)
        {
            previousClip.Unload();
        }
    }

    public void SelectOptionA()
    {
        currentClip.decision = StoryClip.Decision.OptionA;
        optionAText.GetComponent<Outline>().enabled = true;
        optionBText.GetComponent<Outline>().enabled = false;
    }

    public void SelectOptionB()
    {
        currentClip.decision = StoryClip.Decision.OptionB;
        optionAText.GetComponent<Outline>().enabled = false;
        optionBText.GetComponent<Outline>().enabled = true;
    }

    public void SaveStoryToSharedData()
    {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(Application.persistentDataPath + "/LastPlaythrough"))
        {
            file.WriteLine("Yu is " + (joinedForcesWithProf ? "EVIL" : "GOOD"));
            file.WriteLine("Ancestors were " + (ancestorsWereEvil ? "EVIL" : "GOOD"));
            file.WriteLine("Parfity is " + (parfityWasDeterred ? "GOOD" : "EVIL"));

            file.WriteLine("Yu is " + (yuDied ? "DEAD" : "ALIVE"));
            file.WriteLine("The Prof is " + (profDied ? "DEAD" : "ALIVE"));
            file.WriteLine("Parfity is " + (parfityDied ? "DEAD" : "ALIVE"));

            file.WriteLine("Yu " + (metClasper ? "MET" : "DID NOT MEET") + " Clasper");
        }
    }
}
