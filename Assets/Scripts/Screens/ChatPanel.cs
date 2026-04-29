using TMPro;
using UnityEngine;

public class ChatPanel : MonoBehaviour
{
    [SerializeField] AudioSource textSource;
    [SerializeField] TMP_Text chatText;
    [TextArea]
    [SerializeField] string[] chatArray1;
    [TextArea]
    [SerializeField] string[] chatArray2;
    [TextArea]
    [SerializeField] string[] chatArray3;
    [TextArea]
    [SerializeField] string[] chatArray4;
    [SerializeField] float speedPerCharactor = 0.05f;
    [SerializeField] Animator chatAnim;
    [SerializeField] Animator tapAnim;

    int nextChatIndex = 0;
    int charIndex;
    int tapCount = 0;
    float timer = 0;
    bool isStartText = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        nextChatIndex = 0;
        charIndex = 0;
        tapCount = 0;
        chatText.text = string.Empty;
        isStartText = true;
        Loader.Instance.isMainButton = false;
        AudioManager.instance.PlayStoyEffect(textSource);
    }

    public void TapToNextChat()
    {
        if (tapCount == 0)
        {
            isStartText = false;
            tapCount = 1;
            if (PlayerPrefs.GetInt("ChatterBox") == 0 || !PlayerPrefs.HasKey("ChatterBox"))
                chatText.text = chatArray1[nextChatIndex];
            else if (PlayerPrefs.GetInt("ChatterBox") == 1)
                chatText.text = chatArray2[nextChatIndex];
            else if (PlayerPrefs.GetInt("ChatterBox") == 2)
                chatText.text = chatArray3[nextChatIndex];
            else if (PlayerPrefs.GetInt("ChatterBox") == 3)
                chatText.text = chatArray4[nextChatIndex];

            AudioManager.instance.StopStoryEffect(textSource);
        }
        else
        {
            chatAnim.SetTrigger("ChatAnim");
            nextChatIndex++;
            charIndex = 0;
            tapCount = 0;
            chatText.text = string.Empty;
            isStartText = true;
            AudioManager.instance.PlayStoyEffect(textSource);
        }

        if (PlayerPrefs.GetInt("ChatterBox") == 0 || !PlayerPrefs.HasKey("ChatterBox"))
        {
            if (chatArray1.Length <= nextChatIndex)
            {
                isStartText = false;
                PlayerPrefs.SetInt("ChatterBox", 1);
                gameObject.SetActive(false);
                PlayerPrefs.SetInt("PlayButtonTut", 1);
                PlayerPrefs.SetInt("PlayButtonTut", 2);
                //Loader.Instance.isMainButton = true;
                //MainMenu.Instance.ShowPlayButton();
                MainMenu.Instance.StarTutorial();
                return;
            }
        }
        else if (PlayerPrefs.GetInt("ChatterBox") == 1)
        {
            if (chatArray2.Length <= nextChatIndex)
            {
                isStartText = false;
                PlayerPrefs.SetInt("ChatterBox", 2);
                gameObject.SetActive(false);
                //Loader.Instance.isMainButton = true;
                AreaPanel.Instance.AfterUpgrade();
                return;
            }
        }
        else if (PlayerPrefs.GetInt("ChatterBox") == 2)
        {
            if (chatArray3.Length <= nextChatIndex)
            {
                isStartText = false;
                PlayerPrefs.SetInt("ChatterBox", 3);
                gameObject.SetActive(false);
                //Loader.Instance.isMainButton = true;
                AreaPanel.Instance.AfterUpgrade();
                return;
            }
        }
        else if (PlayerPrefs.GetInt("ChatterBox") == 3)
        {
            if (chatArray4.Length <= nextChatIndex)
            {
                isStartText = false;
                PlayerPrefs.SetInt("ChatterBox", 4);
                gameObject.SetActive(false);
                //Loader.Instance.isMainButton = true;
                AreaPanel.Instance.AfterUpgrade();
                return;
            }
        }
        AudioManager.instance.PlayClickEffects();
        tapAnim.SetTrigger("Color");
    }

    private void Update()
    {
        if (!isStartText)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer += speedPerCharactor;
            charIndex++;
            if (PlayerPrefs.GetInt("ChatterBox") == 0 || !PlayerPrefs.HasKey("ChatterBox"))
            {
                chatText.text = chatArray1[nextChatIndex].Substring(0, charIndex);

                if (charIndex >= chatArray1[nextChatIndex].Length)
                {
                    isStartText = false;
                    tapCount = 1;
                    AudioManager.instance.StopStoryEffect(textSource);
                }
            }
            else if (PlayerPrefs.GetInt("ChatterBox") == 1)
            {
                chatText.text = chatArray2[nextChatIndex].Substring(0, charIndex);

                if (charIndex >= chatArray2[nextChatIndex].Length)
                {
                    isStartText = false;
                    tapCount = 1;
                    AudioManager.instance.StopStoryEffect(textSource);
                }
            }
            else if (PlayerPrefs.GetInt("ChatterBox") == 2)
            {
                chatText.text = chatArray3[nextChatIndex].Substring(0, charIndex);

                if (charIndex >= chatArray3[nextChatIndex].Length)
                {
                    isStartText = false;
                    tapCount = 1;
                    AudioManager.instance.StopStoryEffect(textSource);
                }
            }
            else if (PlayerPrefs.GetInt("ChatterBox") == 3)
            {
                chatText.text = chatArray4[nextChatIndex].Substring(0, charIndex);

                if (charIndex >= chatArray4[nextChatIndex].Length)
                {
                    isStartText = false;
                    tapCount = 1;
                    AudioManager.instance.StopStoryEffect(textSource);
                }
            }
        }
    }
}
