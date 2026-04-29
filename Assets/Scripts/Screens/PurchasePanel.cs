using UnityEngine.UI;
using TMPro;
using UnityEngine;
using GameAnalyticsSDK;

public class PurchasePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] RawImage iconImage;
    [SerializeField] Texture[] powerUpImage;
    [SerializeField] GameObject outOfCoin;

    int currPurchaseIndex = 0;

    private void OnEnable()
    {
        GameManager.Instance.isInput = false;
    }

    public void ShowPanel(int index)
    {
        currPurchaseIndex = index;
        if (currPurchaseIndex == 0)
        {
            titleText.text = "Magnet";
            descText.text = "Matches identical items";
        }
        else if (currPurchaseIndex == 1)
        {
            titleText.text = "Undo";
            descText.text = "regain the power to reverse your last move";
        }
        else if (currPurchaseIndex == 2)
        {
            titleText.text = "Shuffle";
            descText.text = "Use the Shuffle Power-Up to mix things up and find hidden items";
        }
        else if (currPurchaseIndex == 3)
        {
            titleText.text = "Freeze Time";
            descText.text = "Activate the Freeze Time power-up to temporarily halt the timer";
        }
        iconImage.texture = powerUpImage[currPurchaseIndex];
    }

    public void OnPurchase()
    {
        AudioManager.instance.PlayClickEffects();
        if (Loader.Instance.coinCount >= 290)
        {
            if (currPurchaseIndex == 0)
                PurchazeMagnet();
            else if (currPurchaseIndex == 1)
                PurchazeUndo();
            else if (currPurchaseIndex == 2)
                PurchazeShuffle();
            else if (currPurchaseIndex == 3)
                PurchazeFreeze();
            gameObject.SetActive(false);
        }
        else
        {
            outOfCoin.SetActive(true);
        }
        GameManager.Instance.isInput = true;
    }

    void PurchazeMagnet()
    {
        Loader.Instance.magnetCount += 3;
        PlayerPrefs.SetInt("MagnetCount", Loader.Instance.magnetCount);
        Loader.Instance.coinCount -= 290;
        PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);
        GamePlayMenuPanel.Instance.SetMagnet();
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gold", 290, "Magnet", "MagnetPowerUp");
    }
    void PurchazeUndo()
    {
        Loader.Instance.undoCount += 3;
        PlayerPrefs.SetInt("UndoCount", Loader.Instance.undoCount);
        Loader.Instance.coinCount -= 290;
        PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);
        GamePlayMenuPanel.Instance.SetUndo();
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gold", 290, "Undo", "UndoPowerUp");
    }
    void PurchazeShuffle()
    {
        Loader.Instance.fanCount += 3;
        PlayerPrefs.SetInt("ShuffleCount", Loader.Instance.fanCount);
        Loader.Instance.coinCount -= 290;
        PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);
        GamePlayMenuPanel.Instance.SetShuffle();
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gold", 290, "Shuffle", "ShufflePowerUp");
    }
    void PurchazeFreeze()
    {
        Loader.Instance.freezeCount += 3;
        PlayerPrefs.SetInt("FreezeCount", Loader.Instance.freezeCount);
        Loader.Instance.coinCount -= 290;
        PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);
        GamePlayMenuPanel.Instance.SetFreeze();
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Gold", 290, "Freeze", "FreezePowerUp");
    }

    public void ClosePurchasePanel()
    {
        AudioManager.instance.PlayClickEffects();
        GameManager.Instance.isInput = true;
        gameObject.SetActive(false);
        GamePlayMenuPanel.Instance.StartTimer();
    }

}
