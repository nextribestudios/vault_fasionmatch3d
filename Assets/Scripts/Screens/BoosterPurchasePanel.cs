using GameAnalyticsSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterPurchasePanel : MonoBehaviour
{
    [SerializeField] LevelPanel levelPanel;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] RawImage iconImage;
    [SerializeField] Texture[] powerUpImage;
    [SerializeField] GameObject outOfCoin;

    int currPurchaseIndex = 0;

    public void ShowPanel(int index)
    {
        currPurchaseIndex = index;
        if (currPurchaseIndex == 0)
        {
            titleText.text = "Energy";
            descText.text = "Get more time to match and play with the Extra Time Booster";
        }
        else if (currPurchaseIndex == 1)
        {
            titleText.text = "Timer";
            descText.text = "Use the Lightning Booster to zap away unwanted obstacles at start";
        }
        iconImage.texture = powerUpImage[currPurchaseIndex];
    }
    public void OnPurchase()
    {
        if (Loader.Instance.coinCount >= 290)
        {
            if (currPurchaseIndex == 0)
                PurchazeEnergy();
            else if (currPurchaseIndex == 1)
                PurchazeTimer();

            gameObject.SetActive(false);
        }
        else
        {
            outOfCoin.SetActive(true);
        }
    }

    void PurchazeEnergy()
    {
        Loader.Instance.energyCount += 3;
        PlayerPrefs.SetInt("EnergyCount", Loader.Instance.energyCount);
        Loader.Instance.coinCount -= 290;
        PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);
        levelPanel.Init();
        MainMenu.Instance.Initialize();
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gold", 290, "Energy", "EnergyBooster");
    }
    void PurchazeTimer()
    {
        Loader.Instance.timerCount += 3;
        PlayerPrefs.SetInt("TimerCount", Loader.Instance.timerCount);
        Loader.Instance.coinCount -= 290;
        PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);
        levelPanel.Init();
        MainMenu.Instance.Initialize();
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gold", 290, "Timer", "TimerBooster");
    }

    public void ClosePurchasePanel()
    {
        gameObject.SetActive(false);
    }
}
