using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFailPanel : MonoBehaviour
{
    [SerializeField] GameObject purchasePanel;
    [SerializeField] GameObject energyBoosterSelector;
    [SerializeField] GameObject timerBoosterSelector;
    [SerializeField] GameObject lockEnergyBooster;
    [SerializeField] GameObject lockTimerBooster;
    [SerializeField] GameObject energyAdd;
    [SerializeField] GameObject timerAdd;
    [SerializeField] TextMeshProUGUI energyBoosterCountText;
    [SerializeField] TextMeshProUGUI timerBoosterCountText;

    int currPurchaseIndex = 0;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] RawImage iconImage;
    [SerializeField] Texture[] powerUpImage;
    [SerializeField] GameObject outOfCoin;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        Loader.Instance.isEnergySelect = false;
        Loader.Instance.isTimerSelect = false;

        if (Loader.Instance.energyUnlockLevel <= Loader.Instance.currLevel)
        {
            if (Loader.Instance.energyCount <= 0)
            {
                energyAdd.SetActive(true);
                energyBoosterCountText.gameObject.SetActive(false);
            }
            else
            {
                energyAdd.SetActive(false);
                energyBoosterCountText.gameObject.SetActive(true);
                energyBoosterCountText.text = Loader.Instance.energyCount.ToString();
            }
            lockEnergyBooster.SetActive(false);
        }
        else
        {
            lockEnergyBooster.SetActive(true);
        }

        if (Loader.Instance.timerUnlockLevel <= Loader.Instance.currLevel)
        {
            if (Loader.Instance.timerCount <= 0)
            {
                timerAdd.SetActive(true);
                timerBoosterCountText.gameObject.SetActive(false);
            }
            else
            {
                timerAdd.SetActive(false);
                timerBoosterCountText.gameObject.SetActive(true);
                timerBoosterCountText.text = Loader.Instance.timerCount.ToString();
            }
            lockTimerBooster.SetActive(false);
        }
        else
        {
            lockTimerBooster.SetActive(true);
        }
    }

    public void SelectEnergyBooster()
    {
        if (Loader.Instance.energyUnlockLevel > Loader.Instance.currLevel)
            return;

        if (Loader.Instance.energyCount <= 0)
        {
            purchasePanel.SetActive(true);
            ShowPanel(0);
        }
        else
        {
            if (!Loader.Instance.isEnergySelect)
            {
                Loader.Instance.isEnergySelect = true;
                energyBoosterCountText.gameObject.SetActive(false);
                energyBoosterSelector.SetActive(true);
            }
            else
            {
                Loader.Instance.isEnergySelect = false;
                energyBoosterCountText.gameObject.SetActive(true);
                energyBoosterSelector.SetActive(false);
            }
        }
    }

    public void SelectTimerBooster()
    {
        if (Loader.Instance.timerUnlockLevel > Loader.Instance.currLevel)
            return;

        if (Loader.Instance.timerCount <= 0)
        {
            purchasePanel.SetActive(true);
            ShowPanel(1);
        }
        else
        {
            if (!Loader.Instance.isTimerSelect)
            {
                Loader.Instance.isTimerSelect = true;
                timerBoosterCountText.gameObject.SetActive(false);
                timerBoosterSelector.SetActive(true);
            }
            else
            {
                Loader.Instance.isTimerSelect = false;
                timerBoosterCountText.gameObject.SetActive(true);
                timerBoosterSelector.SetActive(false);
            }
        }
    }

    void ShowPanel(int index)
    {
        currPurchaseIndex = index;
        if (currPurchaseIndex == 0)
        {
            titleText.text = "Energy";
        }
        else if (currPurchaseIndex == 1)
        {
            titleText.text = "Timer";
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

            Init();
            purchasePanel.SetActive(false);
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
    }
    void PurchazeTimer()
    {
        Loader.Instance.timerCount += 3;
        PlayerPrefs.SetInt("TimerCount", Loader.Instance.timerCount);
        Loader.Instance.coinCount -= 290;
        PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);
    }
    public void ClosePurchasePanel()
    {
        purchasePanel.SetActive(false);
    }

    public void FailToHomeScene()
    {
        if (Loader.Instance.currLevel == 3)
            SceneManager.LoadScene(3);
        else
            SceneManager.LoadScene(1);

    }

    public void TryAgain()
    {
        if (Loader.Instance.lifeCount <= 0)
            SceneManager.LoadScene(1);
        else
            SceneManager.LoadScene(3);
    }
}
