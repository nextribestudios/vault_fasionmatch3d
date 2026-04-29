using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using GameAnalyticsSDK;

public class LifePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;

    private void Update()
    {
        if (Loader.Instance.lifeCount < 5)
        {
            float diff = LifeData();
            if (diff > 0)
            {
                float minutes = Mathf.FloorToInt(diff / 60);
                float seconds = Mathf.FloorToInt(diff % 60);
                timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                Loader.Instance.lifeCount = 5;
                PlayerPrefs.SetInt("LifeCount", Loader.Instance.lifeCount);
                MainMenu.Instance.LifeCounter();
                ClosePanel();
            }
        }
    }

    float LifeData()
    {
        DateTime currentDate = System.DateTime.Now;

        long temp = Convert.ToInt64(PlayerPrefs.GetString("LifeLostTime"));
        DateTime oldDate = DateTime.FromBinary(temp);
        TimeSpan difference = currentDate.Subtract(oldDate);
        float _diff = 600 - (float)difference.TotalSeconds;
        return _diff;
    }

    public void TapRefilLife()
    {
        AudioManager.instance.PlayClickEffects();
        if (Loader.Instance.coinCount < 100 || Loader.Instance.lifeCount >=5)
            return;

            Loader.Instance.lifeCount = 5;
            PlayerPrefs.SetInt("LifeCount", Loader.Instance.lifeCount);
            Loader.Instance.coinCount -= 100;
            PlayerPrefs.SetInt("LifeCount", Loader.Instance.coinCount);
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gold", 100, "Life", "Lives");
        MainMenu.Instance.CoinCounter();
        MainMenu.Instance.LifeCounter();
    }

    public void CloseWindow()
    {
        AudioManager.instance.PlayClickEffects();
        gameObject.GetComponent<Animator>().SetTrigger("PanelOut");
        Invoke("ClosePanel", 0.2f);
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
