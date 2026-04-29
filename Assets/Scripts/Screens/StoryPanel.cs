using UnityEngine;
using TMPro;
using GameAnalyticsSDK;

public class StoryPanel : MonoBehaviour
{
    [SerializeField] AudioSource textSource;
    [SerializeField] TMP_Text storyText;
    [TextArea]
    [SerializeField] string[] storyArray;
    [SerializeField] float speedPerCharactor = 0.05f;
    [SerializeField] Animator tapAnim;
    [SerializeField] Animator noteAnim;
    [SerializeField] GameObject[] storyImages;
    [SerializeField] GameObject storyTextBase;
    [SerializeField] Transform entryPos;
    [SerializeField] Transform exitPos;
    int nextStoryIndex = 0;
    int charIndex;
    int tapCount;
    float timer = 0;
    bool isStartText = false;

    private void Start()
    {
        nextStoryIndex = 0;
        charIndex = 0;
        tapCount = 0;
        //storyText.text = string.Empty;
        storyText.text = storyArray[nextStoryIndex];
        isStartText = true;
        //AudioManager.instance.PlayStoyEffect(textSource);
        noteAnim.SetTrigger("Anim");
    }

    public void TapToNextStory()
    {
        /*
        if (nextStoryIndex < storyImages.Length-1)
        {
            if (tapCount == 0)
            {
                isStartText = false;
                tapCount = 1;
                storyText.text = storyArray[nextStoryIndex];
                AudioManager.instance.StopStoryEffect(textSource);
            }
            else
            {
                nextStoryIndex++;
                if (nextStoryIndex < storyArray.Length)
                {


                    charIndex = 0;
                    tapCount = 0;
                    storyText.text = string.Empty;
                    isStartText = true;
                    AudioManager.instance.PlayStoyEffect(textSource);
                }
                else
                    storyTextBase.SetActive(false);
                iTween.MoveTo(storyImages[nextStoryIndex - 1], iTween.Hash("position", exitPos.position, "time", .5f, "easeType", "linear"));
                iTween.MoveTo(storyImages[nextStoryIndex], iTween.Hash("position", entryPos.position, "time", .5f, "easeType", "linear"));
                noteAnim.SetTrigger("Anim");
            }
        }
        else
        {
            nextStoryIndex++;
        }

        if (storyImages.Length <= nextStoryIndex)
        {
            isStartText = false;
            PlayerPrefs.SetInt("FirstStory", 1);
            MainMenu.Instance.ShowChatBox();
            gameObject.SetActive(false);
            AudioManager.instance.StopStoryEffect(textSource);
            return;
        }*/
        nextStoryIndex++;
        if (storyImages.Length <= nextStoryIndex)
        {
            isStartText = false;
            PlayerPrefs.SetInt("FirstStory", 1);
            MainMenu.Instance.ShowChatBox();
            //MainMenu.Instance.AfterStory();
            gameObject.SetActive(false);
            AudioManager.instance.StopStoryEffect(textSource);
            return;
        }

        if (storyArray.Length > nextStoryIndex)
            storyText.text = storyArray[nextStoryIndex];
        else
            storyTextBase.SetActive(false);

        iTween.MoveTo(storyImages[nextStoryIndex - 1], iTween.Hash("position", exitPos.position, "time", .5f, "easeType", "linear"));
        iTween.MoveTo(storyImages[nextStoryIndex], iTween.Hash("position", entryPos.position, "time", .5f, "easeType", "linear"));
        noteAnim.SetTrigger("Anim");
        
        AudioManager.instance.PlayClickEffects();
        tapAnim.SetTrigger("Color");
    }

    //private void Update()
    //{
    //    if (!isStartText)
    //        return;

    //    timer -= Time.deltaTime;
    //    if (timer <= 0)
    //    {
    //        timer += speedPerCharactor;
    //        charIndex++;
    //        storyText.text = storyArray[nextStoryIndex].Substring(0, charIndex);

    //        if (charIndex >= storyArray[nextStoryIndex].Length)
    //        {
    //            AudioManager.instance.StopStoryEffect(textSource);
    //            isStartText = false;
    //            tapCount = 1;
    //        }
    //    }
    //}

    public void SkipStory()
    {
        isStartText = false;
        PlayerPrefs.SetInt("FirstStory", 1);
        MainMenu.Instance.ShowChatBox();
        //MainMenu.Instance.AfterStory();
        gameObject.SetActive(false);
        AudioManager.instance.StopStoryEffect(textSource);
        AudioManager.instance.PlayClickEffects();
        GameAnalytics.NewDesignEvent("Skip Story");


    }
}
