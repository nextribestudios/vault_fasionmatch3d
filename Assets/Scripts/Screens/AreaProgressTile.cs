using UnityEngine.UI;
using UnityEngine;

public class AreaProgressTile : MonoBehaviour
{
    [SerializeField] int areaID;
    public int needStarCount;
    [SerializeField] GameObject progressDone;
    [SerializeField] Animator anim;
    [SerializeField] GameObject grayShade;
    [SerializeField] Button upgaradeButton;
    public bool IsActive = false;

    private void OnEnable()
    {
        if (Loader.Instance.starCount < needStarCount || !IsActive)
        {
            grayShade.SetActive(true);
            upgaradeButton.enabled = false;
        }
        else
        {
            grayShade.SetActive(false);
            upgaradeButton.enabled = true;
        }
    }

    public void BuildArea()
    {
        AudioManager.instance.PlayClickEffects();
        AreaPanel.Instance.UpdateArea(areaID, needStarCount);
    }

    public void ProgressIndication()
    {
        anim.enabled = true;
        Invoke("DisableButtonObject", 1);
    }

    void DisableButtonObject()
    {
        anim.gameObject.SetActive(false);
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void UpgradeDoneAnim()
    {
        progressDone.SetActive(true);
    }

    public void DisableButton()
    {
        anim.GetComponent<Button>().enabled = false;
    }

    public void GrayShadeActivate(bool stat)
    {
        grayShade.SetActive(stat);
        upgaradeButton.enabled = !stat;
        IsActive = !stat;
    }

    public void GrayShadeStarCount()
    {
        if (Loader.Instance.starCount < needStarCount)
        {
            grayShade.SetActive(true);
            upgaradeButton.enabled = false;
        }
        else
        {
            grayShade.SetActive(false);
            upgaradeButton.enabled = true;
        }
    }
}
