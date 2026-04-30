using System;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using DG.Tweening;

public class Loader : MonoBehaviour, IGameAnalyticsATTListener
{
    public static Loader Instance;

    public int currLevel = 1;

    public int currAreaProgress = 0;
    public int areaConsumeStar = 0;
    //public bool[] totalAreaProgression = new bool[13]; 

    public int coinCount = 500;
    public int starCount = 500;
    public int lifeCount = 5;

    public int magnetCount = 5;
    public int undoCount = 5;
    public int fanCount = 5;
    public int freezeCount = 5;
    public int energyCount = 5;
    public int timerCount = 5;

    public int magnetUnlockLevel = 2;
    public int undoUnlockLevel = 3;
    public int fanUnlockLevel = 4;
    public int freezeUnlockLevel = 4;
    public int  energyUnlockLevel = 0;
    public int timerUnlockLevel = 0;

    public bool isEnergySelect = false;
    public bool isTimerSelect = false;
    public bool isGotCoin = false;
    public bool isGotStar = false;
    public bool isHardLevel = false;
    public bool gotCoinChest = false;

    public bool IsMusic = true;
    public bool IsSound = true;
    public bool IsVibration = true;

    public bool isMainButton = true;

    [SerializeField] public int timeRemaining;

    [SerializeField] bool isTest = false;
    [SerializeField] int levelToTest = 10;

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
        DontDestroyOnLoad(gameObject);
        //PlayerPrefs.DeleteAll();
        LoadData();

        if (isTest)
            currLevel = levelToTest;


        Application.targetFrameRate = 60;

       // Invoke("LoadMainMenu", 1.5f);

        GameAnalytics.Initialize();
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    void LoadData()
    {
        string version = PlayerPrefs.GetString("Version", string.Empty);
        if (string.IsNullOrWhiteSpace(version))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("Version", Application.version);
        }
        else
        {
            if (version != Application.version)
            {
                // => THIS IS A VERSION MISMATCH -> UPDATED
                PlayerPrefs.DeleteAll();

                PlayerPrefs.SetString("Version", Application.version);
            }
        }


        if (PlayerPrefs.GetInt("FirstLoad") == 1)
        {
            PlayerPrefs.SetInt("FirstLoad", 1);
            if (PlayerPrefs.GetInt("CurrentLevel") == 0)
                currLevel = 1;
            else
                currLevel = PlayerPrefs.GetInt("CurrentLevel");

            coinCount = PlayerPrefs.GetInt("CoinCount");
            starCount = PlayerPrefs.GetInt("StarCount");
            lifeCount = PlayerPrefs.GetInt("LifeCount");

            magnetCount = PlayerPrefs.GetInt("MagnetCount");
            undoCount = PlayerPrefs.GetInt("UndoCount");
            fanCount = PlayerPrefs.GetInt("FanCount");
            freezeCount = PlayerPrefs.GetInt("FreezeCount");
            energyCount = PlayerPrefs.GetInt("EnergyCount");
            timerCount = PlayerPrefs.GetInt("TimerCount");

            currAreaProgress = PlayerPrefs.GetInt("CurrAreaProgress");
            areaConsumeStar = PlayerPrefs.GetInt("AreaConsumeStar");

            if (PlayerPrefs.GetInt("Music") == 1)
                IsMusic = true;
            else
                IsMusic = false;

            if (PlayerPrefs.GetInt("Sound") == 1)
                IsSound = true;
            else
                IsSound = false;

            if (PlayerPrefs.GetInt("Vibration") == 1)
                IsVibration = true;
            else
                IsVibration = false;

        }
        else
        {
            PlayerPrefs.SetInt("FirstLoad", 1);
            PlayerPrefs.SetInt("CoinCount", coinCount);
            PlayerPrefs.SetInt("StarCount", starCount);
            PlayerPrefs.SetInt("LifeCount", lifeCount);

            PlayerPrefs.SetInt("MagnetCount", magnetCount);
            PlayerPrefs.SetInt("UndoCount", undoCount);
            PlayerPrefs.SetInt("FanCount", fanCount);
            PlayerPrefs.SetInt("FreezeCount", freezeCount);
            PlayerPrefs.SetInt("EnergyCount", energyCount);
            PlayerPrefs.SetInt("TimerCount", timerCount);

            PlayerPrefs.SetInt("CurrAreaProgress", currAreaProgress);
            PlayerPrefs.SetInt("AreaConsumeStar", areaConsumeStar);

            PlayerPrefs.SetInt("Music", 1);
            PlayerPrefs.SetInt("Sound", 1);
            PlayerPrefs.SetInt("Vibration", 1);
        }

        //for (int i = 0; i < totalAreaProgression.Length; i++)
        //{
        //    string temp = "Area" + i.ToString();
        //    if (PlayerPrefs.GetInt(temp) == 0 || !PlayerPrefs.HasKey(temp))
        //        totalAreaProgression[i] = false;
        //    else
        //        totalAreaProgression[i] = true;
        //}
        LifeData();
    }

    
    void SaveData()
    {
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.SetInt("StarCount", starCount);
        PlayerPrefs.SetInt("LifeCount", lifeCount);

        PlayerPrefs.SetInt("MagnetCount", magnetCount);
        PlayerPrefs.SetInt("UndoCount", undoCount);
        PlayerPrefs.SetInt("FanCount", fanCount);
        PlayerPrefs.SetInt("FreezeCount", freezeCount);
        PlayerPrefs.SetInt("EnergyCount", energyCount);
        PlayerPrefs.SetInt("TimerCount", timerCount);

        //PlayerPrefs.SetInt("CurrAreaProgress", currAreaProgress);
        PlayerPrefs.SetInt("AreaConsumeStar", areaConsumeStar);
    }

    public void LoadMainMenu()
    {
        DOVirtual.DelayedCall(1f,() => { SceneManager.LoadScene(1); });
        
    }

    public void LifeData()
    {
        if (!PlayerPrefs.HasKey("LifeLostTime") || lifeCount > 4)
            return;
        DateTime currentDate = System.DateTime.Now;
        long temp = Convert.ToInt64(PlayerPrefs.GetString("LifeLostTime"));
        DateTime oldDate = DateTime.FromBinary(temp);
        TimeSpan difference = currentDate.Subtract(oldDate);
        if (difference.TotalSeconds > 600)
        {
            lifeCount = 5;
            PlayerPrefs.SetInt("LifeCount", lifeCount);
        }
    }

    //private void OnApplicationQuit()
    //{
    //    Debug.LogError(PlayerPrefs.GetInt("CoinCount"));
    //    Debug.LogError(PlayerPrefs.GetInt("StarCount"));
    //    Debug.LogError(PlayerPrefs.GetInt("LifeCount"));
    //}
    public void GameAnalyticsATTListenerNotDetermined()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerRestricted()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerDenied()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerAuthorized()
    {
        GameAnalytics.Initialize();
    }
}
