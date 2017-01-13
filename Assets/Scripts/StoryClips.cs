using System;
using UnityEngine;

public abstract class StoryClip
{
    public string clipName = "INVALID";

    public string optionAText = "OPTION A TEXT";
    public string optionBText = "OPTION B TEXT";

    public float optionATime = 5f;
    public float optionBTime = 10f;
    public float answerStartTime = 15f;
    public float answerCutoffTime = 1000000f;

    public StoryClip nextClipA;
    public StoryClip nextClipB;
    public StoryClip nextClipNoInput;

    public bool noDecisionInThisClip = false;

    public ResourceRequest resourceRequest { get; private set; }

    public enum Decision
    {
        None,
        OptionA,
        OptionB
    }
    public Decision decision = Decision.None;

    public void LoadVideo()
    {
        resourceRequest = Resources.LoadAsync<MovieTexture>("Video/" + clipName);
    }

    public abstract void SelectNextPotentialClips();

    public void LoadNextPotentialVideos()
    {
        if (nextClipA != null)
        {
            nextClipA.LoadVideo();
        }

        if (nextClipB != null)
        {
            nextClipB.LoadVideo();
        }

        if (nextClipNoInput != null)
        {
            nextClipNoInput.LoadVideo();
        }
    }

    public virtual void OnEndOfClip() { }

    public void Unload()
    {
        Debug.LogWarning("StoryClip.Unload() has not been implemented!");
    }
}

public class StoryClip01_Intro : StoryClip
{
    public StoryClip01_Intro()
    {
        clipName = "PupetGame001";

        optionAText = "Look in the back";
        optionBText = "Refuse his request";

        optionATime = 160f;
        optionBTime = 160f;
        answerStartTime = 166f;
        answerCutoffTime = 182f;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipA = new StoryClip02_LookInTheBack();
        nextClipB = new StoryClip03_RefuseToLookInBack();
        nextClipNoInput = new StoryClip04_ParfityEntersTheFridge();
    }
}

public class StoryClip02_LookInTheBack : StoryClip
{
    public StoryClip02_LookInTheBack()
    {
        clipName = "PupetGame002";

        noDecisionInThisClip = true;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip04_ParfityEntersTheFridge();
    }
}

public class StoryClip03_RefuseToLookInBack : StoryClip
{
    public StoryClip03_RefuseToLookInBack()
    {
        clipName = "PupetGame003";

        noDecisionInThisClip = true;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip04_ParfityEntersTheFridge();
    }
}

public class StoryClip04_ParfityEntersTheFridge : StoryClip
{
    public StoryClip04_ParfityEntersTheFridge()
    {
        clipName = "PupetGame004";

        optionAText = "Find another way!";
        optionBText = "Just keep smashing!";

        optionATime = 195f;
        optionBTime = 195f;
        answerStartTime = 10000000f;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip05_DiscoveringYourAncestors();
    }

    public override void OnEndOfClip()
    {
        StoryManager.instance.parfityWasDeterred = (decision == Decision.OptionA);
    }
}

public class StoryClip05_DiscoveringYourAncestors : StoryClip
{
    public StoryClip05_DiscoveringYourAncestors()
    {
        clipName = "PupetGame005";

        optionAText = "EVIL";
        optionBText = "GOOD";

        optionATime = 35f;
        optionBTime = 37.75f;
        answerStartTime = 52;
    }

    public override void SelectNextPotentialClips()
    {
        if (StoryManager.instance.parfityWasDeterred)
        {
            nextClipNoInput = new StoryClip07_ParfityFindsAnotherWayIn();
        }
        else
        {
            nextClipNoInput = new StoryClip06_ProfRecruitsParfity();
        }
    }

    public override void OnEndOfClip()
    {
        StoryManager.instance.ancestorsWereEvil = (decision != Decision.OptionB);
    }
}

public class StoryClip06_ProfRecruitsParfity : StoryClip
{
    public StoryClip06_ProfRecruitsParfity()
    {
        clipName = "PupetGame006";

        noDecisionInThisClip = true;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip08_TheLion();
    }
}

public class StoryClip07_ParfityFindsAnotherWayIn : StoryClip
{
    public StoryClip07_ParfityFindsAnotherWayIn()
    {
        clipName = "PupetGame007";

        noDecisionInThisClip = true;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip08_TheLion();
    }
}

public class StoryClip08_TheLion : StoryClip
{
    public StoryClip08_TheLion()
    {
        clipName = "PupetGame008";

        optionAText = "FIGHT!";
        optionBText = "RUN AWAY!";

        optionATime = 46;
        optionBTime = 47;
        answerStartTime = 59f;
        answerCutoffTime = 76.5f;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipA = new StoryClip10_LionFightAndRiddle();
        nextClipB = new StoryClip09_RunFromTheLion();
        nextClipNoInput = new StoryClip_Credits();
    }

    public override void OnEndOfClip()
    {
        if (decision == Decision.OptionA)
        {
            StoryManager.instance.metClasper = true;
        }
        else if (decision == Decision.None)
        {
            StoryManager.instance.yuDied = true;
            StoryManager.instance.profDied = true;
        }        
    }
}

public class StoryClip09_RunFromTheLion : StoryClip
{
    public StoryClip09_RunFromTheLion()
    {
        clipName = "PupetGame009";

        optionAText = "Help the Prof";
        optionBText = "Stand against him";

        optionATime = 100f;
        optionBTime = 100f;
        answerStartTime = 110f;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipA = new StoryClip14_TeamUpWithProf();
        nextClipNoInput = new StoryClip13_RunFromTheProf();
    }

    public override void OnEndOfClip()
    {
        StoryManager.instance.joinedForcesWithProf = (decision == Decision.OptionA);
    }
}

public class StoryClip10_LionFightAndRiddle : StoryClip
{
    public StoryClip10_LionFightAndRiddle()
    {
        clipName = "PupetGame010";

        optionAText = "Yes...?";
        optionBText = "No...?";

        optionATime = 116.5f;
        optionBTime = 117.5f;
        answerStartTime = 124f;
        answerCutoffTime = 153.5f;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipA = new StoryClip11_RiddleAnswerYES();
        nextClipB = new StoryClip12_RiddleAnswerNO();
        nextClipNoInput = new StoryClip_Ending();
    }
}

public class StoryClip11_RiddleAnswerYES : StoryClip
{
    public StoryClip11_RiddleAnswerYES()
    {
        clipName = "PupetGame011";

        noDecisionInThisClip = true;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip_Ending();
    }
}

public class StoryClip12_RiddleAnswerNO : StoryClip
{
    public StoryClip12_RiddleAnswerNO()
    {
        clipName = "PupetGame012";

        noDecisionInThisClip = true;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip_Ending();
    }
}

public class StoryClip13_RunFromTheProf : StoryClip
{
    public StoryClip13_RunFromTheProf()
    {
        clipName = "PupetGame013";

        noDecisionInThisClip = true;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip_Ending();
    }
}

public class StoryClip14_TeamUpWithProf : StoryClip
{
    public StoryClip14_TeamUpWithProf()
    {
        clipName = "PupetGame014";

        noDecisionInThisClip = true;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip_Ending();
    }
}

public class StoryClip_Ending : StoryClip
{
    public StoryClip_Ending()
    {
        int endingNumber = 0;

        if (StoryManager.instance.parfityWasDeterred)
        {
            if (!StoryManager.instance.ancestorsWereEvil)
            {
                if (StoryManager.instance.joinedForcesWithProf)
                {
                    endingNumber = 1;
                }
                else
                {
                    endingNumber = 2;
                }
            }
            else
            {
                if (StoryManager.instance.joinedForcesWithProf)
                {
                    endingNumber = 3;
                }
                else
                {
                    endingNumber = 4;
                }
            }
        }
        else
        {
            if (!StoryManager.instance.ancestorsWereEvil)
            {
                if (StoryManager.instance.joinedForcesWithProf)
                {
                    endingNumber = 5;
                }
                else
                {
                    endingNumber = 6;
                }
            }
            else
            {
                if (StoryManager.instance.joinedForcesWithProf)
                {
                    endingNumber = 7;
                }
                else
                {
                    endingNumber = 8;
                }
            }
        }

        clipName = "PupetGameEnd" + endingNumber;

        noDecisionInThisClip = true;
    }

    public override void SelectNextPotentialClips()
    {
        nextClipNoInput = new StoryClip_Credits();
    }

    public override void OnEndOfClip()
    {
        // The Prof and Parfity will each die in any ending wherein they are working against Yu.
        StoryManager.instance.profDied = !StoryManager.instance.joinedForcesWithProf;
        StoryManager.instance.parfityDied = StoryManager.instance.joinedForcesWithProf == StoryManager.instance.parfityWasDeterred;
    }
}

public class StoryClip_Credits : StoryClip
{
    public StoryClip_Credits()
    {
        clipName = "PupetGameCredits";

        optionAText = "Play Again?";
        optionATime = 0f;
        optionBTime = float.MaxValue;
        answerStartTime = 0f;
    }

    public override void SelectNextPotentialClips()
    {
    }
}