using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameAnalyticsSDK;
using UnityEngine.SceneManagement;

public class GamePlayMenuPanel : MonoBehaviour
{
    public static GamePlayMenuPanel Instance;

    [SerializeField] GameObject pausePanel;
    [SerializeField] PurchasePanel purchasePanel;
    [SerializeField] TimeCounter timeCounter;

    [SerializeField] RawImage magnetIcon;
    [SerializeField] RawImage undoIcon;
    [SerializeField] RawImage fanIcon;
    [SerializeField] RawImage freezeIcon;
    [SerializeField] Texture magnetTexture;
    [SerializeField] Texture undoTexture;
    [SerializeField] Texture fanTexture;
    [SerializeField] Texture freezeTexture;
    [SerializeField] Texture lockTexture;
    [SerializeField] GameObject magnetCountOBJ;
    [SerializeField] GameObject undoCountOBJ;
    [SerializeField] GameObject fanCountOBJ;
    [SerializeField] GameObject freezeCountOBJ;
    [SerializeField] TextMeshProUGUI magnetCountText;
    [SerializeField] TextMeshProUGUI undoCountText;
    [SerializeField] TextMeshProUGUI fanCountText;
    [SerializeField] TextMeshProUGUI freezeCountText;
    [SerializeField] GameObject magnetAdd;
    [SerializeField] GameObject undoAdd;
    [SerializeField] GameObject fanAdd;
    [SerializeField] GameObject freezeAdd;

    //[SerializeField] GameObject energyBoosterSelector;
    //[SerializeField] GameObject timerBoosterSelector;
    [SerializeField] TextMeshProUGUI levelNumberText;
    [SerializeField] TextMeshProUGUI energyBoosterCountText;
    [SerializeField] TextMeshProUGUI timerBoosterCountText;
    int energyBoosterCount = 0;
    int timerBoosterCount = 0;
    //bool isEnergySelect = false;
    //bool isTimerSelect = false;

    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject levelPanel;

    [SerializeField] Animator magnetTutButton;
    [SerializeField] Animator undoTutButton;
    [SerializeField] Animator fanTutButton;
    [SerializeField] Animator freezeTutButton;

    public bool isShuffle = false;
    public bool isFreeze = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBackGroundMusic(1);
        SetMagnet();
        SetUndo();
        SetShuffle();
        SetFreeze();
    }

    public void SetMagnet()
    {
        if (Loader.Instance.magnetUnlockLevel > Loader.Instance.currLevel)
        {
            magnetIcon.texture = lockTexture;
            magnetCountOBJ.SetActive(false);
        }
        else if (Loader.Instance.magnetUnlockLevel == Loader.Instance.currLevel && PlayerPrefs.GetInt("MagnetTutorial") == 0)
        {
            GameManager.Instance.isInput = false;
            tutorialPanel.SetActive(true);
            magnetTutButton.enabled = true;
            GameManager.Instance.isTutorial = true;
        }
        else
        {
            timeCounter.timerIsRunning = true;
            GameManager.Instance.isTutorial = false;
            tutorialPanel.SetActive(false);
            magnetIcon.texture = magnetTexture;
            magnetCountOBJ.SetActive(true);
            if (Loader.Instance.magnetCount <= 0)
            {
                magnetAdd.SetActive(true);
                magnetCountText.gameObject.SetActive(false);
            }
            else
            {
                magnetAdd.SetActive(false);
                magnetCountText.gameObject.SetActive(true);
                magnetCountText.text = Loader.Instance.magnetCount.ToString();
            }
        }
    }

    public void SetUndo()
    {
        if (Loader.Instance.undoUnlockLevel > Loader.Instance.currLevel)
        {
            undoIcon.texture = lockTexture;
            undoCountOBJ.SetActive(false);
        }
        else if (Loader.Instance.undoUnlockLevel == Loader.Instance.currLevel && PlayerPrefs.GetInt("UndoTutorial") == 0)
        {
            GameManager.Instance.isInput = false;
            tutorialPanel.SetActive(true);
            undoTutButton.enabled = true;
            GameManager.Instance.isTutorial = true;
        }
        else
        {
            timeCounter.timerIsRunning = true;
            GameManager.Instance.isTutorial = false;
            tutorialPanel.SetActive(false);
            undoIcon.texture = undoTexture;
            undoCountOBJ.SetActive(true);
            if (Loader.Instance.undoCount <= 0)
            {
                undoAdd.SetActive(true);
                undoCountText.gameObject.SetActive(false);
            }
            else
            {
                undoAdd.SetActive(false);
                undoCountText.gameObject.SetActive(true);
                undoCountText.text = Loader.Instance.undoCount.ToString();
            }
        }
    }

    public void SetShuffle()
    {
        if (Loader.Instance.fanUnlockLevel > Loader.Instance.currLevel)
        {
            fanIcon.texture = lockTexture;
            fanCountOBJ.SetActive(false);
        }
        else if (Loader.Instance.fanUnlockLevel == Loader.Instance.currLevel && PlayerPrefs.GetInt("ShuffleTutorial") == 0)
        {
            GameManager.Instance.isInput = false;
            tutorialPanel.SetActive(true);
            fanTutButton.enabled = true;
            GameManager.Instance.isTutorial = true;
        }
        else
        {
            timeCounter.timerIsRunning = true;
            GameManager.Instance.isTutorial = false;
            tutorialPanel.SetActive(false);
            fanIcon.texture = fanTexture;
            fanCountOBJ.SetActive(true);
            if (Loader.Instance.fanCount <= 0)
            {
                fanAdd.SetActive(true);
                fanCountText.gameObject.SetActive(false);
            }
            else
            {
                fanAdd.SetActive(false);
                fanCountText.gameObject.SetActive(true);
                fanCountText.text = Loader.Instance.fanCount.ToString();
            }
        }
    }

    public void SetFreeze()
    {
        if (Loader.Instance.freezeUnlockLevel > Loader.Instance.currLevel)
        {
            freezeIcon.texture = lockTexture;
            freezeCountOBJ.SetActive(false);
        }
        else if (Loader.Instance.freezeUnlockLevel == Loader.Instance.currLevel && PlayerPrefs.GetInt("FreezeTutorial") == 0)
        {
            GameManager.Instance.isInput = false;
            tutorialPanel.SetActive(true);
            freezeTutButton.enabled = true;
            GameManager.Instance.isTutorial = true;
        }
        else
        {
            timeCounter.timerIsRunning = true;
            GameManager.Instance.isTutorial = false;
            tutorialPanel.SetActive(false);
            freezeIcon.texture = freezeTexture;
            freezeCountOBJ.SetActive(true);
            if (Loader.Instance.freezeCount <= 0)
            {
                freezeAdd.SetActive(true);
                freezeCountText.gameObject.SetActive(false);
            }
            else
            {
                freezeAdd.SetActive(false);
                freezeCountText.gameObject.SetActive(true);
                freezeCountText.text = Loader.Instance.freezeCount.ToString();
            }
        }
    }

    public void InitGameFailPanel()
    {
        //isEnergySelect = false;
        //isTimerSelect = false;
        energyBoosterCount = PlayerPrefs.GetInt("energyBooster");
        timerBoosterCount = PlayerPrefs.GetInt("timerBooster");
        energyBoosterCountText.text = energyBoosterCount.ToString();
        timerBoosterCountText.text = timerBoosterCount.ToString();
    }

    public void MagnetButtonTapped()
    {
        if (Loader.Instance.magnetUnlockLevel > Loader.Instance.currLevel)
            return;

        if (Loader.Instance.magnetCount > 0)
        {
            GameManager.Instance.isInput = true;
            GameManager.Instance.PowerUpAutoMatch();
        }
        else
            ShowPurchasePanel(0);

        GameAnalytics.NewDesignEvent("Magnet Button Tapped");

    }

    public void UndoButtonTapped()
    {
        if (Loader.Instance.undoUnlockLevel > Loader.Instance.currLevel)
            return;

        if (Loader.Instance.undoCount > 0)
            GameManager.Instance.PowerUPUndo();
        else
            ShowPurchasePanel(1);

        GameAnalytics.NewDesignEvent("Undo Button Tapped");
    }

    public void FanButtonTapped()
    {
        if (Loader.Instance.fanUnlockLevel > Loader.Instance.currLevel || isShuffle)
            return;

        if (Loader.Instance.fanCount > 0)
        {
            //isShuffle = true;
            GameManager.Instance.PowerUPFan();
        }
        else
            ShowPurchasePanel(2);

        GameAnalytics.NewDesignEvent("Fan Button Tapped");
    }

    public void FreazeButtonTapped()
    {
        if (Loader.Instance.fanUnlockLevel > Loader.Instance.currLevel || isFreeze)
            return;

        if (Loader.Instance.freezeCount > 0)
        {
            isFreeze = true;
            GameManager.Instance.FreezeTime();
        }
        else
            ShowPurchasePanel(3);

        GameAnalytics.NewDesignEvent("Freeze Button Tapped");
    }

    public void PauseButtonTapped(bool stat)
    {
        AudioManager.instance.PlayClickEffects();
        pausePanel.SetActive(stat);
        GameManager.Instance.PauseGamePlay(stat);

        GameAnalytics.NewDesignEvent("Pause Button Tapped");
    }

    void ShowPurchasePanel(int index)
    {
        AudioManager.instance.PlayClickEffects();
        purchasePanel.gameObject.SetActive(true);
        purchasePanel.ShowPanel(index);
        timeCounter.timerIsRunning = false;

        GameAnalytics.NewDesignEvent("Show Purchase Panel");
    }

    public void StartTimer()
    {
        timeCounter.timerIsRunning = true;
    }

    public void ShowLevelPanel()
    {
        levelNumberText.text = "LEVEL " + Loader.Instance.currLevel;
        levelPanel.SetActive(true);
    }

    public void NextLevelTapped()
    {
        SceneManager.LoadScene(3);
    }
}
