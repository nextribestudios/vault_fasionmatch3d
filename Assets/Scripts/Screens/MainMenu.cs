using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RDG;
using System.Linq;
using GameAnalyticsSDK;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [SerializeField] GameObject camera;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI starText;
    [SerializeField] TextMeshProUGUI lifeText;
    [SerializeField] GameObject lifeAddButton;
    [SerializeField] GameObject[] areaArray;
    [SerializeField] Transform[] camZoomInArray;
    [SerializeField] Transform[] camZoomOutArray;
    [SerializeField] GameObject[] coinChestVFX;
    [SerializeField] GameObject[] boosterChestVFX;
    [SerializeField] Animator starCounterAnim;
    [SerializeField] Animator coinCounterAnim;
    [SerializeField] Animator playButonAnim;
    [SerializeField] GameObject starCounterVFX;
    [SerializeField] GameObject coinCounterVFX;
    [SerializeField] GameObject starCounter;
    [SerializeField] GameObject levelPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject livesPanel;
    [SerializeField] GameObject storyPanel;
    [SerializeField] GameObject chatPanel;
    [SerializeField] GameObject coinChestPanel;
    [SerializeField] GameObject inAppShopPanel;
    [SerializeField] GameObject coinChestVFXParent;
    [SerializeField] GameObject outOfLife;
    [SerializeField] GameObject playButtonTut;
    [SerializeField] GameObject starAchieveEffect;
    [SerializeField] GameObject upgardeFinishEffect;
    [SerializeField] GameObject bigPlayButton;
    [SerializeField] GameObject smallPlayButton;
    [SerializeField] GameObject upgardeButton;
    int currProgressIndex;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        
    }
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        TempFunc();
    //    }
    //}

    void TempFunc()
    {
        List<int> list = new List<int>()
        { 1,2,3,4,5,2,2,2,1
        };

        //int a=0

        var a = list.GroupBy(x => x);

        foreach (var y in a)
        {
            Debug.LogError("The element" + y.Key);
            Debug.LogError(y.Count());
        }
    }

    public void Initialize()
    {
        AudioManager.instance.PlayBackGroundMusic(0);
        
        LifeCounter();

        ShowArea();
        if (Loader.Instance.isGotStar)
        {
            Loader.Instance.currLevel++;
            PlayerPrefs.SetInt("CurrentLevel", Loader.Instance.currLevel);
            if (Loader.Instance.currLevel > 4)
            {
                Loader.Instance.starCount++;
                PlayerPrefs.SetInt("StarCount", Loader.Instance.starCount);

                StarCounter();
                Loader.Instance.isGotStar = false;
                StartCoroutine(DisableStarPanel());
            }
            AreaPanel.Instance.InitProgression();
        }
        else
        {
            CoinCounter();
            StarCounter();
        }
        levelText.text = "Level " + Loader.Instance.currLevel.ToString();

        if (Loader.Instance.currLevel == 1)// && !PlayerPrefs.HasKey("FirstStory") || PlayerPrefs.GetInt("FirstStory") == 0)
        {
            bigPlayButton.SetActive(false);
            smallPlayButton.SetActive(false);
            upgardeButton.SetActive(false);
            //storyPanel.SetActive(true);
            starCounter.SetActive(false);
            Loader.Instance.isMainButton = true;
            ShowPlayButtonTutorial();
        }
        else if(Loader.Instance.currLevel < 4)// && !PlayerPrefs.HasKey("FirstStory") || PlayerPrefs.GetInt("FirstStory") == 0)
        {
            bigPlayButton.SetActive(false);
            smallPlayButton.SetActive(true);
            upgardeButton.SetActive(true);
            //starCounter.SetActive(true);
            //Loader.Instance.isMainButton = true;
            //playButtonTut.SetActive(false);
        }
        else if (Loader.Instance.currLevel == 4)
        {
            if (!PlayerPrefs.HasKey("FirstStory") || PlayerPrefs.GetInt("FirstStory") == 0)
            {
                Loader.Instance.isMainButton = true;
                bigPlayButton.SetActive(false);
                smallPlayButton.SetActive(false);
                upgardeButton.SetActive(false);
                storyPanel.SetActive(true);
                starCounter.SetActive(true);
            }
            else
            {
                bigPlayButton.SetActive(false);
                smallPlayButton.SetActive(true);
                upgardeButton.SetActive(true);
                starCounter.SetActive(true);
            }
        }
        else
        {
            bigPlayButton.SetActive(false);
            smallPlayButton.SetActive(true);
            upgardeButton.SetActive(true);
            starCounter.SetActive(true);
        }
    }


    public void UpgradeTutorial()
    {
        smallPlayButton.SetActive(true);
        upgardeButton.SetActive(true);
        starCounter.SetActive(true);
        AreaPanel.Instance.AreaButtonTutorial();
    }

    public void StarTutorial()
    {
        //Loader.Instance.starCount++;
        //PlayerPrefs.SetInt("StarCount", Loader.Instance.starCount);
        //Loader.Instance.currLevel++;
        //PlayerPrefs.SetInt("CurrentLevel", Loader.Instance.currLevel);
        StarCounter();
        Loader.Instance.isGotStar = false;
        StartCoroutine(DisableStarPanel());
    }

    public void StarCounter()
    {
        if (!Loader.Instance.isGotStar)
            starText.text = Loader.Instance.starCount.ToString();
        else
            starText.text = (Loader.Instance.starCount - 1).ToString();
    }

    public void LifeCounter()
    {
        Loader.Instance.LifeData();
        if (Loader.Instance.lifeCount >= 5)
        {
            lifeAddButton.SetActive(false);
            lifeText.text = "FULL";
        }
        else
        {
            lifeAddButton.SetActive(true);
            lifeText.text = Loader.Instance.lifeCount.ToString();
        }
    }

    public void CoinCounter()
    {
        if (!Loader.Instance.gotCoinChest)
            coinText.text = Loader.Instance.coinCount.ToString();
        else
            coinText.text = (Loader.Instance.coinCount-500).ToString();
    }

    public void PlayButtonTapped()
    {
        if (!Loader.Instance.isMainButton)
            return;

        Vibration.Vibrate(50, 1);
        AudioManager.instance.PlayClickEffects();
        if (Loader.Instance.lifeCount <= 0)
        {
            outOfLife.SetActive(true);
            return;
        }
        levelPanel.SetActive(true);
        if (playButtonTut.activeSelf)
            playButtonTut.SetActive(false);
        PlayerPrefs.SetInt("PlayButtonTut", 2);
        GameAnalytics.NewDesignEvent("Play Button Tapped");
    }

    public void settingsButtonTapped()
    {
        if (!Loader.Instance.isMainButton)
            return;
        Vibration.Vibrate(50, 1);
        AudioManager.instance.PlayClickEffects();
        settingsPanel.SetActive(true);
        GameAnalytics.NewDesignEvent("Settings Button Tapped");
    }

    public void LifeButtonTapped()
    {
        if (!Loader.Instance.isMainButton)
            return;
        AudioManager.instance.PlayClickEffects();
        Vibration.Vibrate(50, 1);
        livesPanel.SetActive(true);
        GameAnalytics.NewDesignEvent("Life Button Tapped");
    }

    public void ShowPlayButtonTutorial()
    {
        bigPlayButton.SetActive(true);
        playButtonTut.SetActive(true);
    }

    public void CoinClaimChestButton()
    {
        AudioManager.instance.PlayClickEffects();
        coinChestPanel.GetComponent<Animator>().SetTrigger("PanelOut");
        StartCoroutine(CoinChest());
        GameAnalytics.NewDesignEvent("Coin Claim Button Tapped");
    }

    IEnumerator CoinChest()
    {
        Vibration.Vibrate(1000, 1);
        coinChestVFXParent.SetActive(true);
        for (int i = 0; i < coinChestVFX.Length; i++)
        {
            coinChestVFX[i].SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        coinChestPanel.SetActive(false);
        if (Loader.Instance.currLevel > 9)
        {
            boosterChestVFX[0].SetActive(true);
            yield return new WaitForSeconds(0.2f);
            boosterChestVFX[1].SetActive(true);
            Loader.Instance.timerCount += 1;
            Loader.Instance.energyCount += 1;
        }
        yield return new WaitForSeconds(0.3f);
        CoinCounter();
        AudioManager.instance.PlayCoinCollectionEffects();
        playButonAnim.SetTrigger("BoosterChest");
        coinCounterAnim.SetTrigger("CounterImpact");
        coinCounterVFX.SetActive(true);
        Loader.Instance.isMainButton = true;
        yield return new WaitForSeconds(1);
        coinChestVFXParent.SetActive(false);
        boosterChestVFX[0].SetActive(false);
        boosterChestVFX[1].SetActive(false);
        
    }

    void ShowArea()
    {
        for (int i = 0; i < Loader.Instance.currAreaProgress; i++)
        {
            areaArray[i].SetActive(true);
            areaArray[i].GetComponent<AreaBuilder>().DisableEffects();
        }
        if (Loader.Instance.currAreaProgress == 0)
            camera.transform.position = camZoomOutArray[Loader.Instance.currAreaProgress].transform.position;
        else
            camera.transform.position = camZoomOutArray[Loader.Instance.currAreaProgress-1].transform.position;
    }

    public void ShowChatBox()
    {
        chatPanel.SetActive(true);
    }

    public void ShowAreaPgress(int _index)
    {
        currProgressIndex = _index;
        starText.text = Loader.Instance.starCount.ToString();
        StartCoroutine(ShowProgress());
    }

    IEnumerator ShowProgress()
    {
        MoveObjectToTarget(camera, camZoomInArray[Loader.Instance.currAreaProgress - 1].position,1, 0, iTween.EaseType.easeOutQuad);
        yield return new WaitForSeconds(0.9f);
        Vibration.Vibrate(1000, 10);
        AudioManager.instance.PlayConstructionEffects();
        areaArray[currProgressIndex].SetActive(true);
        areaArray[currProgressIndex].GetComponent<AreaBuilder>().EnableEffects();
        MoveObjectToTarget(camera, camZoomOutArray[Loader.Instance.currAreaProgress - 1].position, 1, 2.5f, iTween.EaseType.easeOutQuad);
        yield return new WaitForSeconds(3);
        if (Loader.Instance.currAreaProgress > 12)
        {
            upgardeFinishEffect.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        //if (Loader.Instance.currAreaProgress < 4)
        //    chatPanel.SetActive(true);
        //else
            AreaPanel.Instance.AfterUpgrade();
    }

    IEnumerator DisableStarPanel()
    {
        Vibration.Vibrate(1000, 1);
        AudioManager.instance.PlayStarEffects();
        starAchieveEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        starCounterAnim.SetTrigger("CounterImpact");
        starCounterVFX.SetActive(true);
        starText.text = Loader.Instance.starCount.ToString();
        if (PlayerPrefs.GetInt("AreaButtonTut") == 1)
            AreaPanel.Instance.StarTutorial();
        yield return new WaitForSeconds(0.5f);
        starAchieveEffect.SetActive(false);
        //starCounterAnim.enabled = false;
        starCounterVFX.SetActive(false);
        yield return null;
        if (Loader.Instance.gotCoinChest)
        {
            Loader.Instance.gotCoinChest = false;
            Loader.Instance.coinCount += 500;
            PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);
            coinChestPanel.SetActive(true);
            Loader.Instance.isMainButton = false;
        }
    }

    public void ShowShopPanel()
    {
        AudioManager.instance.PlayClickEffects();
        inAppShopPanel.SetActive(true);
    }

    void MoveObjectToTarget(GameObject thisGameOBJ, Vector3 targetPos, float durationMove, float delayTime, iTween.EaseType easeType)
    {
        iTween.MoveTo(thisGameOBJ, iTween.Hash("position", targetPos, "time", durationMove, "delay", delayTime, "easeType", easeType));
    }

    public void TapMainMenu()
    {
        AudioManager.instance.PlayClickEffects();
    }

    //public void TestVibration(int index)
    //{
    //    if (index == 0)
    //        Vibration.Vibrate(1, 1);
    //    else if (index == 1)
    //        Vibration.Vibrate(10, 1);
    //    else if (index == 2)
    //        Vibration.Vibrate(20, 1);
    //    else if (index == 3)
    //        Vibration.Vibrate(30, 1);
    //    else if (index == 4)
    //        Vibration.Vibrate(40, 1);
    //    else if (index == 5)
    //        Vibration.Vibrate(50, 1);
    //    else if (index == 6)
    //        Vibration.Vibrate(60, 1);
    //    else if (index == 7)
    //        Vibration.Vibrate(70, 1);
    //    else if (index == 8)
    //        Vibration.Vibrate(80, 1);
    //    else if (index == 9)
    //        Vibration.Vibrate(90, 1);
    //    else if (index == 10)
    //        Vibration.Vibrate(100, 1);
    //    else if (index == 11)
    //        Vibration.Vibrate(100, -1);
    //    else if (index == 12)
    //        Vibration.Vibrate(100, 0);
    //    else if (index == 13)
    //        Vibration.Vibrate(100, 1);
    //}
}
