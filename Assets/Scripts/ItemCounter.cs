using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCounter : MonoBehaviour
{
    public RawImage itemTex;
    [SerializeField] TextMeshProUGUI counterText;
    [SerializeField] GameObject completeSign;
    [SerializeField] Animator anim;
    public int itemCount;
    public int itemID;

    public void SetItemCount()
    {
        counterText.text = itemCount.ToString();
        if (itemCount == 0)
        {
            Invoke("CounterFinish", 0.5f);
        }
    }

    void CounterFinish()
    {
        completeSign.SetActive(true);
        anim.enabled = true;
        counterText.text = " ";
        Invoke("DisableObject", 1);
    }

    private void DisableObject()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
            TutorialManager.Instance.ReArrangeCounter();
        else
            GameManager.Instance.ReArrangeCounter();
        gameObject.SetActive(false);
    }
}
