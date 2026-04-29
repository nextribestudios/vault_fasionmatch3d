using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialGameScreen : MonoBehaviour
{
    public static TutorialGameScreen Instance;

    [SerializeField] GameObject energyBoosterSelector;
    [SerializeField] GameObject timerBoosterSelector;
    [SerializeField] TextMeshProUGUI energyBoosterCountText;
    [SerializeField] TextMeshProUGUI timerBoosterCountText;
    int energyBoosterCount = 0;
    int timerBoosterCount = 0;
    bool isEnergySelect = false;
    bool isTimerSelect = false;

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
        
    }

    public void InitGameFailPanel()
    {
        isEnergySelect = false;
        isTimerSelect = false;
        energyBoosterCount = PlayerPrefs.GetInt("energyBooster");
        timerBoosterCount = PlayerPrefs.GetInt("timerBooster");
        energyBoosterCountText.text = energyBoosterCount.ToString();
        timerBoosterCountText.text = timerBoosterCount.ToString();
    }

    public void TapEnergyBooster()
    {
        if (energyBoosterCount <= 0 || isEnergySelect)
            return;
        energyBoosterCount--;
        PlayerPrefs.SetInt("energyBooster", energyBoosterCount);
        isEnergySelect = true;
        energyBoosterCountText.gameObject.SetActive(false);
        energyBoosterSelector.SetActive(true);
    }

    public void TapTimerBooster()
    {
        if (timerBoosterCount <= 0 || isTimerSelect)
            return;
        timerBoosterCount--;
        PlayerPrefs.SetInt("timerBooster", timerBoosterCount);
        isTimerSelect = true;
        timerBoosterCountText.gameObject.SetActive(false);
        timerBoosterSelector.SetActive(true);
    }

}
