using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using TMPro;
using RDG;
using GameAnalyticsSDK;

public class AreaPanel : MonoBehaviour
{
    public static AreaPanel Instance;

    [SerializeField] Slider panelPlider;
    [SerializeField] Slider buttonSlider;
    [SerializeField] TextMeshProUGUI areaProgressText;
    [SerializeField] TextMeshProUGUI areaProgressButtonText;
    //[SerializeField] TextMeshProUGUI nextProgressText;
    //[SerializeField] int[] starToEachProgress;
    [SerializeField] AreaProgressTile[] areaTileArray;
    [SerializeField] int totalStarForCurrArea;
    [SerializeField] GameObject outOfCoin;
    [SerializeField] GameObject areaProgressScreen;
    [SerializeField] GameObject AreaButtonTutObj;
    [SerializeField] GameObject starTutObj;
    [SerializeField] GameObject AreaTileTutObj;
    [SerializeField] GameObject[] progressStarEffect;
    [SerializeField] GameObject grayShader;
    [SerializeField] Animator starCounterAnim;
    [SerializeField] GameObject starCounterVFX;
    [SerializeField] Transform tileTrans1;
    [SerializeField] Transform tileTrans2;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        InitProgression();
    }

    public void InitProgression()
    {
        //float fillAmount = Loader.Instance.areaConsumeStar / totalStarForCurrArea;
        //Debug.LogError(fillAmount);
        panelPlider.value = Loader.Instance.areaConsumeStar;
        buttonSlider.value = Loader.Instance.areaConsumeStar;

        areaProgressText.text = Loader.Instance.areaConsumeStar.ToString() + "/" + totalStarForCurrArea.ToString();
        areaProgressButtonText.text = Loader.Instance.areaConsumeStar.ToString() + "/" + totalStarForCurrArea.ToString();

        AreaTileTutObj.SetActive(false);

        if (Loader.Instance.currAreaProgress < 13)
        {
            if (Loader.Instance.starCount < areaTileArray[Loader.Instance.currAreaProgress].needStarCount || Loader.Instance.currLevel < 4)
                grayShader.SetActive(true);
            else
                grayShader.SetActive(false);
        }
        else
            grayShader.SetActive(true);
    }

    public void AreaProgressButton()
    {
        if (PlayerPrefs.GetInt("PlayButtonTut") == 1 || !Loader.Instance.isMainButton || Loader.Instance.currLevel < 4)
            return;
        if (Loader.Instance.currAreaProgress == 0)
            AreaTileTutObj.SetActive(true);
        PlayerPrefs.SetInt("AreaButtonTut", 2);
        AreaButtonTutObj.SetActive(false);
        for (int i = 0; i < Loader.Instance.currAreaProgress; i++)
            areaTileArray[i].gameObject.SetActive(false);

        if (Loader.Instance.currAreaProgress < 13)
        {
            areaTileArray[Loader.Instance.currAreaProgress].GetComponent<AreaProgressTile>().IsActive = true;
            areaTileArray[Loader.Instance.currAreaProgress].gameObject.SetActive(true);
            areaTileArray[Loader.Instance.currAreaProgress].transform.position = tileTrans1.position;
            if (Loader.Instance.currAreaProgress < 12 && Loader.Instance.currAreaProgress != 0)
            {
                areaTileArray[Loader.Instance.currAreaProgress + 1].GetComponent<AreaProgressTile>().IsActive = false;
                areaTileArray[Loader.Instance.currAreaProgress + 1].gameObject.SetActive(true);
                areaTileArray[Loader.Instance.currAreaProgress + 1].transform.position = tileTrans2.position;

            }
        }

        Vibration.Vibrate(50, 1);
        AudioManager.instance.PlayClickEffects();
        areaProgressScreen.SetActive(true);
    }

    public void CloseAreaButton()
    {
        AudioManager.instance.PlayClickEffects();
        areaProgressScreen.GetComponent<Animator>().SetTrigger("PanelOut");
        Invoke("ClosePanel", 0.2f);
    }

    void ClosePanel()
    {
        areaProgressScreen.SetActive(false);
        GameAnalytics.NewDesignEvent("Close upgrade panel tapped");
    }

    public void StarTutorial()
    {
        starTutObj.SetActive(true);
    }

    public void StarTutorialButton()
    {
        starTutObj.SetActive(false);
        //Loader.Instance.isMainButton = false;
        //MainMenu.Instance.ShowStory();
        MainMenu.Instance.UpgradeTutorial();
        PlayerPrefs.SetInt("AreaButtonTut", 2);
        GameAnalytics.NewDesignEvent("Star Tutorial");
        //AreaButtonTutObj.SetActive(true);
    }

    public void AreaButtonTutorial()
    {
        AreaButtonTutObj.SetActive(true);
        Loader.Instance.isMainButton = true;
    }

    int currStarCount = 0;
    public void UpdateArea(int _areaID, int _star)
    {
        currStarCount = _star;
        if (Loader.Instance.starCount >= currStarCount)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "World1", "AreaUpgade", Loader.Instance.currAreaProgress.ToString());
            Loader.Instance.isMainButton = false;
            Loader.Instance.areaConsumeStar += currStarCount;
            PlayerPrefs.SetInt("AreaConsumeStar", Loader.Instance.areaConsumeStar);
            Loader.Instance.starCount -= currStarCount;
            PlayerPrefs.SetInt("StarCount", Loader.Instance.starCount);
            Loader.Instance.currAreaProgress++;
            PlayerPrefs.SetInt("CurrAreaProgress", Loader.Instance.currAreaProgress);
            InitProgression();
            areaTileArray[Loader.Instance.currAreaProgress - 1].DisableButton();
            MainMenu.Instance.StarCounter();
            StartCoroutine(UpgradeAreaEffect());
        }
        else
            outOfCoin.SetActive(true);
    }

    IEnumerator UpgradeAreaEffect()
    {
        for (int i = 0; i < currStarCount; i++)
        {
            progressStarEffect[i].SetActive(true);
            starCounterAnim.SetTrigger("CounterImpact");
            starCounterVFX.SetActive(false);
            starCounterVFX.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.2f);
        AudioManager.instance.PlayStarEffects();
        areaTileArray[Loader.Instance.currAreaProgress - 1].ProgressIndication();
        yield return new WaitForSeconds(0.5f);
        areaProgressScreen.SetActive(false);
        MainMenu.Instance.ShowAreaPgress(Loader.Instance.currAreaProgress - 1);
        for (int i = 0; i < progressStarEffect.Length; i++)
            progressStarEffect[i].SetActive(false);
    }

    public void AfterUpgrade()
    {
        StartCoroutine(AfterUpgradation());
    }

    IEnumerator AfterUpgradation()
    {
        areaProgressScreen.SetActive(true);
        if (Loader.Instance.currAreaProgress == 1)
        {
            areaTileArray[Loader.Instance.currAreaProgress].GetComponent<AreaProgressTile>().IsActive = false;
            areaTileArray[Loader.Instance.currAreaProgress].gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        areaTileArray[Loader.Instance.currAreaProgress - 1].UpgradeDoneAnim();
        yield return new WaitForSeconds(1);
        areaTileArray[Loader.Instance.currAreaProgress - 1].GetComponent<Animator>().SetTrigger("TileOut");
        //yield return new WaitForSeconds(0.5f);
        if (Loader.Instance.currAreaProgress < 13)
        {
            areaTileArray[Loader.Instance.currAreaProgress].GetComponent<AreaProgressTile>().GrayShadeStarCount();
            areaTileArray[Loader.Instance.currAreaProgress].gameObject.SetActive(true);
            iTween.MoveTo(areaTileArray[Loader.Instance.currAreaProgress].gameObject, iTween.Hash("position", tileTrans1.position, "time", 0.5, "easeType", iTween.EaseType.easeInElastic));
            if (Loader.Instance.currAreaProgress < 12 || Loader.Instance.currAreaProgress != 1)
            {
                yield return new WaitForSeconds(0.5f);
                areaTileArray[Loader.Instance.currAreaProgress + 1].GetComponent<AreaProgressTile>().IsActive = false;
                areaTileArray[Loader.Instance.currAreaProgress+1].gameObject.SetActive(true);
                areaTileArray[Loader.Instance.currAreaProgress+1].transform.position = tileTrans2.position;

            }
        }
        
        Loader.Instance.isMainButton = true;
    }
}
