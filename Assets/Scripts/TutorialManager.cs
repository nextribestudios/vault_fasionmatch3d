using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] GameObject[] tutorialHand;
    [SerializeField] GameObject[] tutorialObject;
    [SerializeField] Texture[] itemTexture;
    [SerializeField] Transform[] slotPos;
    [SerializeField] ItemCounter[] itemCounters;
    [SerializeField] Transform[] itemCountersPos;
    [SerializeField] GameObject[] starArray;
    [SerializeField] GameObject[] tutorialArray;
    [SerializeField] TextMeshProUGUI winTimeText;
    [SerializeField] TextMeshProUGUI levelNumberText;
    [SerializeField] GameObject gameVictory;
    [SerializeField] GameObject gameFail;
    [SerializeField] GameObject gameVictoryConfetti;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject levelPanel;
    [SerializeField] TimeCounter timeCounter;
    [SerializeField] RawImage[] slotImages;
    public Transform[] slotArray;
    [SerializeField] ParticleSystem[] matchVFXs;
    [SerializeField] int matchVFXCount = 0;
    private GameObject selectedItem;
    private Camera camera;
    RaycastHit hit;
    int layerMask = 1 << 6;
    Ray ray;
    bool isTutorial = true;
    bool isInput = true;
    int tutorialIndex = 0;
    int totalCounter = 9;
    public List<GameObject> selectedItemList = new List<GameObject>();

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
        camera = Camera.main;
        tutorialHand[0].SetActive(true);
        isTutorial = true;
        isInput = true;
        tutorialObject[0].tag = "Player";
        tutorialIndex = 0;
        timeCounter.totalTime = 300;
        timeCounter.InitTimer();
        tutorialArray[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (totalCounter <= 0 || !isInput)
            return;
        if (Input.GetMouseButton(0))
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.CompareTag("Player"))
                    ClickToSelectItem();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectedItem != null && selectedItem.GetComponent<Items>().isClickable)
            {
                AddItemToList();
            }

            if (isTutorial)
            {
                tutorialHand[tutorialIndex].SetActive(true);
            }
            //selectedItem?.GetComponent<Items>().SetNormalMaterial();
        }
    }

    void ClickToSelectItem()
    {
        if (!hit.transform.GetComponent<Items>().isClickable)
            return;
        if (selectedItem != null)
        {
            if (selectedItem != hit.transform.gameObject)
                selectedItem.GetComponent<Items>().SetNormalMaterial();
            tutorialHand[tutorialIndex].SetActive(false);
        }
        selectedItem = hit.transform.gameObject;
        selectedItem.GetComponent<Rigidbody>().isKinematic = false;
        selectedItem.GetComponent<Items>().SetOutLineMaterial();
    }

    void AddItemToList()
    {
        if (selectedItemList.Count == 0)
        {
            selectedItemList.Add(selectedItem);
            selectedItem.GetComponent<Items>().posIndex = 0;
            MoveObjectToTarget(selectedItem, slotArray[0].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
        }
        else if (selectedItemList.Count == 1)
        {
            selectedItemList.Add(selectedItem);
            selectedItem.GetComponent<Items>().posIndex = 1;
            MoveObjectToTarget(selectedItem, slotArray[1].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
        }
        else
        {
            int temp = 0;
            int posInd = 0;
            for (int i = 0; i < selectedItemList.Count; i++)
            {
                if (selectedItemList[i].GetComponent<Items>().itemID == selectedItem.GetComponent<Items>().itemID)
                {
                    temp++;
                    posInd = i + 1;
                }
            }
            if (temp == 0)
            {
                selectedItemList.Add(selectedItem);
                selectedItem.GetComponent<Items>().posIndex = selectedItemList.Count - 1;
                MoveObjectToTarget(selectedItem, slotArray[selectedItemList.Count - 1].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
            }
            else
            {
                selectedItemList.Insert(posInd, selectedItem);
                selectedItem.GetComponent<Items>().posIndex = posInd;
                for (int i = posInd + 1; i < selectedItemList.Count; i++)
                {
                    selectedItemList[i].GetComponent<Items>().posIndex = i;
                    MoveObjectToTarget(selectedItemList[i], slotArray[i].position, 0.1f, 0, iTween.EaseType.easeOutQuad);
                }
                MoveObjectToTarget(selectedItem, slotArray[posInd].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
                if (temp == 2)
                {
                    GameObject[] tempArray = new GameObject[3];
                    for (int i = 0; i < 3; i++)
                    {
                        tempArray[i] = selectedItemList[posInd - i];
                        //selectedItemList.RemoveAt(posInd - i);
                    }
                    for (int i = 0; i < selectedItemList.Count; i++)
                        selectedItemList[i].GetComponent<Items>().posIndex = i;
                    RemoveElement(tempArray);
                    //StartCoroutine(RemoveCombileItems(tempArray));
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (selectedItem.GetComponent<Items>().itemID == itemCounters[i].itemID)
            {
                itemCounters[i].itemCount -= 1;
                totalCounter--;
                itemCounters[i].SetItemCount();
                if (totalCounter <= 0)
                    tutorialArray[1].SetActive(false);
            }
        }
        RotateObjectToTarget(selectedItem, Vector3.zero, 0.3f, iTween.EaseType.easeOutQuad);
        selectedItem.GetComponent<Items>().DisableClickable();
        selectedItem.GetComponent<Items>().SetNormalMaterial();
        selectedItem.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        AudioManager.instance.PlayItemEffects();
        if (isTutorial)
        {
            if (tutorialIndex >= 2)
            {
                isTutorial = false;
                foreach (GameObject go in tutorialHand)
                    go.SetActive(false);
                foreach (GameObject go in tutorialObject)
                    go.tag = "Player";
                tutorialArray[0].SetActive(false);
                tutorialArray[1].SetActive(true);
            }
            else
            {
                tutorialHand[tutorialIndex].SetActive(false);
                tutorialIndex++;
                tutorialHand[tutorialIndex].SetActive(true);
                tutorialObject[tutorialIndex].tag = "Player";
            }        
        }
    }
    bool isCoroutine1 = false;
    bool isCoroutine2 = false;
    int slotIndex = 0;
    IEnumerator RemoveCombileItems1(GameObject[] _tempArray)
    {
        int tempID = _tempArray[1].GetComponent<Items>().posIndex;
        isCoroutine1 = true;
        yield return new WaitForSeconds(0.6f);
        AudioManager.instance.PlayMatchEffects();
        slotImages[slotIndex].texture = itemTexture[_tempArray[1].GetComponent<Items>().itemID];
        slotImages[slotIndex].transform.localPosition = slotPos[tempID].localPosition;
        slotImages[slotIndex].gameObject.SetActive(true);
        foreach (ItemCounter counter in itemCounters)
        {
            if (counter.itemID == _tempArray[1].GetComponent<Items>().itemID)
            {
                MoveObjectToTarget(slotImages[slotIndex].gameObject, counter.transform.position, 0.3f, 0, iTween.EaseType.easeOutQuad);
                break;
            }
        }
        
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < 3; i++)
        {
            GameObject go = _tempArray[i];
            selectedItemList.Remove(go);
            Destroy(go);
        }
        if (!isCoroutine2)
        {
            for (int i = 0; i < selectedItemList.Count; i++)
            {
                MoveObjectToTarget(selectedItemList[i], slotArray[i].position, 0.2f, 0f, iTween.EaseType.easeOutQuad);
                selectedItemList[i].GetComponent<Items>().posIndex = i;
            }

            if (totalCounter <= 0)
            {
                yield return new WaitForEndOfFrame();
                OnGameOverPanel(true);
            }
        }
        isCoroutine1 = false;
        slotIndex++;
    }

    IEnumerator RemoveCombileItems2(GameObject[] _tempArray)
    {
        int tempID = _tempArray[1].GetComponent<Items>().posIndex;
        isCoroutine2 = true;
        yield return new WaitForSeconds(0.6f);
        AudioManager.instance.PlayMatchEffects();
        slotImages[slotIndex].texture = itemTexture[_tempArray[1].GetComponent<Items>().itemID];
        slotImages[slotIndex].transform.localPosition = slotPos[tempID].localPosition;
        slotImages[slotIndex].gameObject.SetActive(true);
        foreach (ItemCounter counter in itemCounters)
        {
            if (counter.itemID == _tempArray[1].GetComponent<Items>().itemID)
            {
                MoveObjectToTarget(slotImages[slotIndex].gameObject, counter.transform.position, 0.3f, 0, iTween.EaseType.easeOutQuad);
                break;
            }
        }
        
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < 3; i++)
        {
            GameObject go = _tempArray[i];
            selectedItemList.Remove(go);
            Destroy(go);
        }

        for (int i = 0; i < selectedItemList.Count; i++)
        {
            MoveObjectToTarget(selectedItemList[i], slotArray[i].position, 0.2f, 0f, iTween.EaseType.easeOutQuad);
            selectedItemList[i].GetComponent<Items>().posIndex = i;
        }

        if (totalCounter <= 0)
        {
            yield return new WaitForEndOfFrame();
            OnGameOverPanel(true);
        }
        isCoroutine2 = false;
        slotIndex++;
    }
    void RemoveElement(GameObject[] removeObjects)
    {
        int tempID = removeObjects[1].GetComponent<Items>().posIndex;
        MoveObjectToTarget(removeObjects[0], slotArray[tempID].position, 0.2f, 0.3f, iTween.EaseType.easeInBack);
        MoveObjectToTarget(removeObjects[2], slotArray[tempID].position, 0.2f, 0.3f, iTween.EaseType.easeInBack);
        matchVFXs[matchVFXCount].transform.localPosition = slotArray[tempID].localPosition;
        matchVFXs[matchVFXCount].Stop();
        //slotimages.texture = itemtexture[removeobjects[1].getcomponent<items>().itemid];
        //slotimages.transform.localposition = slotpos[tempid].localposition;
        //foreach (itemcounter counter in itemcounters)
        //{
        //    if (counter.itemid == removeobjects[1].getcomponent<items>().itemid)
        //    {
        //        moveobjecttotarget(slotimages.gameobject, counter.transform.position, 0.3f, 0.5f, itween.easetype.easeoutquad);
        //        break;
        //    }
        //}
        removeObjects[1].GetComponent<Items>().FinishAnimation();
        //yield return new WaitForSeconds(0.1f);
        matchVFXs[matchVFXCount].Play();
        matchVFXCount++;
        if (!isCoroutine1)
            StartCoroutine(RemoveCombileItems1(removeObjects));
        else if (!isCoroutine2)
            StartCoroutine(RemoveCombileItems2(removeObjects));
    }

    public void MoveObjectToTarget(GameObject thisGameOBJ, Vector3 targetPos, float durationMove, float delayTime, iTween.EaseType easeType)
    {
        iTween.MoveTo(thisGameOBJ, iTween.Hash("position", targetPos, "time", durationMove, "delay", delayTime, "easeType", easeType));
    }

    public void RotateObjectToTarget(GameObject thisGameOBJ, Vector3 targetRot, float durationRot, iTween.EaseType easeType)
    {
        iTween.RotateTo(thisGameOBJ, iTween.Hash("rotation", targetRot, "time", durationRot, "easeType", easeType));
    }

    public void OnGameOverPanel(bool stat)
    {
        isInput = false;
        for (int i = 0; i < 3; i++)
            starArray[i].SetActive(false);
        if (stat)
        {
            winTimeText.text = timeCounter.GetTime();
            if (timeCounter.timeRemaining > timeCounter.totalTime * .75)
                StartCoroutine(ShowStar(3));
            else if (timeCounter.timeRemaining > timeCounter.totalTime * .50)
                StartCoroutine(ShowStar(2));
            else if (timeCounter.timeRemaining > timeCounter.totalTime * .25)
                StartCoroutine(ShowStar(1));
            else
                StartCoroutine(ShowStar(0));
        }
        else
        {
            AudioManager.instance.PlayFailEffects();
            Loader.Instance.lifeCount--;
            PlayerPrefs.SetInt("LifeCount", Loader.Instance.lifeCount);
            TutorialGameScreen.Instance.InitGameFailPanel();
            gameFail.SetActive(true);
            //loseTimeText.text = timeCounter.GetTime();
        }
    }

    IEnumerator ShowStar(int index)
    {
        AudioManager.instance.PlayWinEffects();
        PlayerPrefs.SetInt("AreaButtonTut", 1);
        Loader.Instance.isGotStar = true;
        //Loader.Instance.currLevel++;
        //PlayerPrefs.SetInt("CurrentLevel", Loader.Instance.currLevel);
        //Loader.Instance.starCount++;
        //PlayerPrefs.SetInt("StarCount", Loader.Instance.starCount);

        gameVictoryConfetti.SetActive(true);
        yield return new WaitForSeconds(1);
        gameVictory.SetActive(true);
        //yield return new WaitForEndOfFrame();
        //for (int i = 0; i < index; i++)
        //{
        //    starArray[i].SetActive(true);
        //    yield return new WaitForSeconds(0.5f);
        //}
    }

    public void ReArrangeCounter()
    {
        int j = 0;
        for (int i = 0; i < 3; i++)
        {
            if (itemCounters[i].itemCount > 0)
            {
                MoveObjectToTarget(itemCounters[i].gameObject, itemCountersPos[j].position, 0.1f, 0, iTween.EaseType.easeInBack);
                j++;
            }
        }
    }

    public void PlayAgainTutorial()
    {
        AudioManager.instance.PlayClickEffects();
        if (Loader.Instance.lifeCount <= 0)
            SceneManager.LoadScene(1);
        else
            SceneManager.LoadScene(2);
        //SceneManager.LoadScene(2);
    }

    public void PauseButtonTapped(bool stat)
    {
        AudioManager.instance.PlayClickEffects();
        pausePanel.SetActive(stat);
        timeCounter.timerIsRunning = !stat;
        isInput = !stat;
    }

    public void QuitGamePlay()
    {
        AudioManager.instance.PlayClickEffects();
        Loader.Instance.lifeCount--;
        PlayerPrefs.SetInt("LifeCount", Loader.Instance.lifeCount);
        SceneManager.LoadScene(1);
    }

    public void FailToHome()
    {
        AudioManager.instance.PlayClickEffects();
        SceneManager.LoadScene(1);
    }

    bool isButton = false;
    public void NextScene()
    {
        if (isButton)
            return;
        isButton = true;
        //Loader.Instance.currLevel++;
        //PlayerPrefs.SetInt("CurrentLevel", Loader.Instance.currLevel);
        //AudioManager.instance.PlayClickEffects();
        //SceneManager.LoadScene(1);

        Loader.Instance.currLevel++;
        Loader.Instance.starCount++;
        PlayerPrefs.SetInt("CurrentLevel", Loader.Instance.currLevel);
        PlayerPrefs.SetInt("StarCount", Loader.Instance.starCount);
        //gameVictory.GetComponent<Animator>().SetTrigger("PanelOut");
        gameVictory.SetActive(false);
        levelNumberText.text = "LEVEL " + Loader.Instance.currLevel;
        levelPanel.SetActive(true);
        AudioManager.instance.PlayClickEffects();
    }

    public void NextLevelTapped()
    {
        SceneManager.LoadScene(3);
    }
}
