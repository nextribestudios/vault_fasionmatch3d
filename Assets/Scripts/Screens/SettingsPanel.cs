using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] GameObject[] musicOBJ;
    [SerializeField] GameObject[] soundOBJ;
    [SerializeField] GameObject[] vibrationOBJ;
    [SerializeField] Button privacyPolicy;

    private void OnPCButton()
    {
        Application.OpenURL("https://vaultgamesstudio.com/privacy-policy/");
    }
    private void OnDisable()
    {
        privacyPolicy.onClick.RemoveAllListeners();
    }
    private void OnEnable()
    {
        privacyPolicy.onClick.AddListener(OnPCButton);
        if (Loader.Instance.IsMusic)
        {
            musicOBJ[0].SetActive(false);
            musicOBJ[1].SetActive(true);
        }
        else
        {
            musicOBJ[0].SetActive(true);
            musicOBJ[1].SetActive(false);
        }
        if (Loader.Instance.IsSound)
        {
            soundOBJ[0].SetActive(false);
            soundOBJ[1].SetActive(true);
        }
        else
        {
            soundOBJ[0].SetActive(true);
            soundOBJ[1].SetActive(false);
        }
        if (Loader.Instance.IsVibration)
        {
            vibrationOBJ[0].SetActive(false);
            vibrationOBJ[1].SetActive(true);
        }
        else
        {
            vibrationOBJ[0].SetActive(true);
            vibrationOBJ[1].SetActive(false);
        }
    }

    public void MusicButton()
    {
        if (Loader.Instance.IsMusic)
        {
            Loader.Instance.IsMusic = false;
            PlayerPrefs.SetInt("Music", 0);
            musicOBJ[0].SetActive(true);
            musicOBJ[1].SetActive(false);
            AudioManager.instance.StopBGM();
        }
        else
        {
            Loader.Instance.IsMusic = true;
            PlayerPrefs.SetInt("Music", 1);
            musicOBJ[0].SetActive(false);
            musicOBJ[1].SetActive(true);
            AudioManager.instance.PlayBackGroundMusic(0);
            //AudioManager.instance.StopBGM();
        }
    }

    public void SoundButton()
    {
        if (Loader.Instance.IsSound)
        {
            Loader.Instance.IsSound = false;
            PlayerPrefs.SetInt("Sound", 0);
            soundOBJ[0].SetActive(true);
            soundOBJ[1].SetActive(false);
        }
        else
        {
            Loader.Instance.IsSound = true;
            PlayerPrefs.SetInt("Sound", 1);
            soundOBJ[0].SetActive(false);
            soundOBJ[1].SetActive(true);
        }
    }

    public void VibrationButton()
    {
        if (Loader.Instance.IsVibration)
        {
            Loader.Instance.IsVibration = false;
            PlayerPrefs.SetInt("Vibration", 0);
            vibrationOBJ[0].SetActive(true);
            vibrationOBJ[1].SetActive(false);
        }
        else
        {
            Loader.Instance.IsVibration = true;
            PlayerPrefs.SetInt("Vibration", 1);
            vibrationOBJ[0].SetActive(false);
            vibrationOBJ[1].SetActive(true);
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
}
