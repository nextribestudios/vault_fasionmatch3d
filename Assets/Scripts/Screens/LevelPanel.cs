using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;

public class LevelPanel : MonoBehaviour
{
    [SerializeField] BoosterPurchasePanel purchasePanel;

    [SerializeField] TextMeshProUGUI levelNumberText;
    [SerializeField] GameObject energyBoosterSelector;
    [SerializeField] GameObject timerBoosterSelector;
    [SerializeField] GameObject lockEnergyBooster;
    [SerializeField] GameObject lockTimerBooster;
    [SerializeField] GameObject energyAdd;
    [SerializeField] GameObject timerAdd;
    [SerializeField] TextMeshProUGUI energyBoosterCountText;
    [SerializeField] TextMeshProUGUI timerBoosterCountText;

    [SerializeField] GameObject boosterTutorialBanner;
    [SerializeField] GameObject energyTutorial;
    [SerializeField] GameObject timerTutorial;
    [SerializeField] GameObject energyTutorialSelect;
    [SerializeField] GameObject timerTutorialSelect;
    [SerializeField] TextMeshProUGUI energyTutorialCount;
    [SerializeField] TextMeshProUGUI timerTutorialCount;

    StringBuilder levelBuilder;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        levelNumberText.text = "Level " + Loader.Instance.currLevel.ToString();

        Loader.Instance.isEnergySelect = false;
        Loader.Instance.isTimerSelect = false;
       

        if (Loader.Instance.energyUnlockLevel <= Loader.Instance.currLevel)
        {
            if (Loader.Instance.energyUnlockLevel == Loader.Instance.currLevel)
            {
                boosterTutorialBanner.SetActive(true);
                energyTutorial.SetActive(true);
                energyTutorialSelect.SetActive(false);
                energyTutorialCount.text = Loader.Instance.energyCount.ToString();
            }
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
            if (Loader.Instance.timerUnlockLevel == Loader.Instance.currLevel)
            {
                boosterTutorialBanner.SetActive(true);
                timerTutorial.SetActive(true);
                timerTutorialSelect.SetActive(false);
                timerTutorialCount.text = Loader.Instance.timerCount.ToString();
            }
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

    public void LoadGamePlay()
    {
        AudioManager.instance.PlayClickEffects();
        if (Loader.Instance.currLevel == 1)
            SceneManager.LoadScene(2);
        else
            SceneManager.LoadScene(3);
    }

    public void SelectEnergyBooster()
    {
        AudioManager.instance.PlayClickEffects();
        if (Loader.Instance.energyUnlockLevel > Loader.Instance.currLevel)
            return;

        if (Loader.Instance.energyCount <= 0)
        {
            purchasePanel.gameObject.SetActive(true);
            purchasePanel.ShowPanel(0);
        }
        else
        {
            if (Loader.Instance.energyUnlockLevel == Loader.Instance.currLevel)
                energyTutorialSelect.SetActive(true);
            if (!Loader.Instance.isEnergySelect)
            {
                if (Loader.Instance.energyUnlockLevel == Loader.Instance.currLevel)
                {
                    energyTutorialSelect.SetActive(true);
                    energyTutorialCount.gameObject.SetActive(false);
                }
                else
                {
                    energyBoosterSelector.SetActive(true);
                    energyBoosterCountText.gameObject.SetActive(false);
                }
                Loader.Instance.isEnergySelect = true;
            }
            else
            {
                if (Loader.Instance.energyUnlockLevel == Loader.Instance.currLevel)
                {
                    energyTutorialSelect.SetActive(false);
                    energyTutorialCount.gameObject.SetActive(true);
                }
                else
                {
                    energyBoosterSelector.SetActive(false);
                    energyBoosterCountText.gameObject.SetActive(true);
                }
                Loader.Instance.isEnergySelect = false;
            }
        }
    }

    public void SelectTimerBooster()
    {
        AudioManager.instance.PlayClickEffects();
        if (Loader.Instance.timerUnlockLevel > Loader.Instance.currLevel)
            return;

        if (Loader.Instance.timerCount <= 0)
        {
            purchasePanel.gameObject.SetActive(true);
            purchasePanel.ShowPanel(1);
        }
        else
        {
            if (Loader.Instance.timerUnlockLevel == Loader.Instance.currLevel)
                timerTutorialSelect.SetActive(true);

            if (!Loader.Instance.isTimerSelect)
            {
                if (Loader.Instance.timerUnlockLevel == Loader.Instance.currLevel)
                {
                    timerTutorialSelect.SetActive(true);
                    timerTutorialCount.gameObject.SetActive(false);
                }
                else
                {
                    timerBoosterSelector.SetActive(true);
                    timerBoosterCountText.gameObject.SetActive(false);
                }
                Loader.Instance.isTimerSelect = true;
            }
            else
            {
                if (Loader.Instance.timerUnlockLevel == Loader.Instance.currLevel)
                {
                    timerTutorialSelect.SetActive(false);
                    timerTutorialCount.gameObject.SetActive(true);
                }
                else
                {
                    timerBoosterSelector.SetActive(false);
                    timerBoosterCountText.gameObject.SetActive(true);
                }
                Loader.Instance.isTimerSelect = false;
            }
        }
    }
}
