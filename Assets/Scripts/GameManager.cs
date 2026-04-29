using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public GeneratingItems generatingItems;

    private Camera camera;
    [SerializeField] GameObject gameVictory;
    [SerializeField] GameObject gameFail;
    [SerializeField] GameObject gameVictoryConfetti;
    [SerializeField] GameObject[] starArray;
    [SerializeField] GameObject lightningVFX;
    [SerializeField] GameObject freezeVFX;
    [SerializeField] GameObject fanObject;
    [SerializeField] GameObject fanBlocker;
    [SerializeField] ParticleSystem matchVFX;
    [SerializeField] TextMeshProUGUI winTimeText;
    [SerializeField] TextMeshProUGUI loseTimeText;
    [SerializeField] TextMeshProUGUI levelTitleText;
    [SerializeField] RawImage slotImages;
    [SerializeField] Transform[] slotPos;
    [SerializeField] Texture[] itemTexture;
    [SerializeField] TimeCounter timeCounter;
    [SerializeField] Animator outSpaceAnim;
    [SerializeField] Button magnetButton;
    public GameObject selectedItem;
    public Transform[] slotArray;
    public int currItemNum = 0;
    public List<GameObject> selectedItemList = new List<GameObject>();
    public List<Items> totalItemList = new List<Items>();
    public ItemCounter[] itemCounters;
    [SerializeField] Transform[] itemCountersPos;
    [SerializeField] Transform itemResetTrans;
    [SerializeField] Transform selectedParent;
    [SerializeField] Transform unselectedParent;
    [SerializeField] List<GameObject> itemStack = new List<GameObject>();
    [SerializeField] GameObject[] upperBounary;
    public int totalCounter = 0;
    [HideInInspector] public bool isInput = true;
    [HideInInspector] public bool isTutorial = false;
    bool isCoroutine = false;

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

    void Start()
    {
        camera = Camera.main;
        generatingItems.GenerateItems();
        levelTitleText.text = "Level " + Loader.Instance.currLevel.ToString();
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelTitleText.text);
        if (Loader.Instance.isEnergySelect)
            StartCoroutine(LightningBlast());
    }
    RaycastHit hit;
    int layerMask = 1 << 6;
    Ray ray;
    // Update is called once per frame
    void Update()
    {
        if (!isInput)
            return;
        if (Input.GetMouseButton(0))
        {
            if (selectedItemList.Count >= 7)
                return;
            ray = camera.ScreenPointToRay(Input.mousePosition);
            //generatingItems.DisableBoundary();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                ClickToSelectItem();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectedItemList.Count >= 7)
                return;
            if (selectedItem != null && selectedItem.GetComponent<Items>().isClickable)
            {
                AddItemToList();
            }
        }

        if (selectedItemList.Count > 6 && !isCoroutine)
        {
            OnGameOverPanel(false);
        }


        //if (isClicked && selectedItem != null)
        //    selectedItem.transform.position = new Vector3(selectedItem.transform.position.x, 1.0f, selectedItem.transform.position.z);
    }

    void ClickToSelectItem()
    {
        if (!hit.transform.GetComponent<Items>().isClickable)
            return;
        if (selectedItem != null)
        {
            if (selectedItem != hit.transform.gameObject)
                selectedItem.GetComponent<Items>().SetNormalMaterial();
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
            itemStack.Add(selectedItem);
            selectedItem.GetComponent<Items>().posIndex = 0;
            MoveObjectToTarget(selectedItem, slotArray[0].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
            //ChangePosition();
        }
        else if (selectedItemList.Count == 1)
        {
            selectedItemList.Add(selectedItem);
            itemStack.Add(selectedItem);
            selectedItem.GetComponent<Items>().posIndex = 1;
            MoveObjectToTarget(selectedItem, slotArray[1].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
            //ChangePosition();
        }
        else
        {
            int temp = 0;
            int posInd = 0;
            for (int i = 0; i < selectedItemList.Count; i++)
            {
                if (selectedItemList[i] == null)
                    continue;
                if (selectedItemList[i].GetComponent<Items>().itemID == selectedItem.GetComponent<Items>().itemID)
                {
                    temp++;
                    posInd = i + 1;
                }
            }
            //Debug.LogError(temp);
            if (temp == 0 || temp == 3)
            {
                selectedItemList.Add(selectedItem);
                itemStack.Add(selectedItem);
                selectedItem.GetComponent<Items>().posIndex = selectedItemList.Count - 1;
                MoveObjectToTarget(selectedItem, slotArray[selectedItemList.Count - 1].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
                //ChangePosition();
                if (selectedItemList.Count == 6 && !isCoroutine)
                {
                    outSpaceAnim.SetTrigger("OutSpaceEnter");
                    isOutSpace = true;
                }
            }
            else
            {
                selectedItemList.Insert(posInd, selectedItem);
                itemStack.Add(selectedItem);
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
                    RemoveElement(tempArray);//StartCoroutine(RemoveElement(tempArray));

                }
                else
                {
                    if (selectedItemList.Count == 6 && !isCoroutine)
                    {
                        outSpaceAnim.SetTrigger("OutSpaceEnter");
                        isOutSpace = true;
                    }
                }
            }
        }
        for (int i = 0; i < 5; i++)
        {
            if (selectedItem.GetComponent<Items>().itemID == itemCounters[i].itemID)
            {
                itemCounters[i].itemCount -= 1;
                totalCounter--;
                itemCounters[i].SetItemCount();
            }
        }
        RotateObjectToTarget(selectedItem, Vector3.zero, 0.3f, 0, iTween.EaseType.easeOutQuad);
        currItemNum = selectedItemList.Count;
        selectedItem.GetComponent<Items>().DisableClickable();
        selectedItem.GetComponent<Items>().SetNormalMaterial();
        selectedItem.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        selectedItem.transform.parent = selectedParent;
        AudioManager.instance.PlayItemEffects();

        if (unselectedParent.childCount < 10 && !boundaryMove)
            BoundaryMove();
    }
    bool isOutSpace = false;

    IEnumerator RemoveCombileItems(GameObject[] _tempArray)
    {
        isCoroutine = true;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 3; i++)
        {
            GameObject go = _tempArray[i];
            selectedItemList.Remove(_tempArray[i]);
            itemStack.Remove(_tempArray[i]);
            Destroy(_tempArray[i]);
        }

        for (int i = 0; i < selectedItemList.Count; i++)
        {
            MoveObjectToTarget(selectedItemList[i], slotArray[i].position, 0.2f, 0f, iTween.EaseType.easeOutQuad);
            selectedItemList[i].GetComponent<Items>().posIndex = i;
        }

        if (totalCounter <= 0)
        {
            OnGameOverPanel(true);
        }
        AudioManager.instance.PlayMatchEffects();
        isCoroutine = false; 
    }

    void RemoveElement(GameObject[] removeObjects)
    {
        GameObject[] _removeObjects = removeObjects;
        outSpaceAnim.SetTrigger("OutSpaceExit");
        isOutSpace = false;
        int tempID = removeObjects[1].GetComponent<Items>().posIndex;
        MoveObjectToTarget(removeObjects[0], slotArray[tempID].position, 0.2f, 0.3f, iTween.EaseType.easeInBack);
        MoveObjectToTarget(removeObjects[2], slotArray[tempID].position, 0.2f, 0.3f, iTween.EaseType.easeInBack);
        matchVFX.transform.localPosition = slotArray[tempID].localPosition;
        matchVFX.Stop();
        slotImages.texture = itemTexture[removeObjects[1].GetComponent<Items>().itemID];
        slotImages.transform.localPosition = slotPos[tempID].localPosition;
        foreach (ItemCounter counter in itemCounters)
        {
            if (counter.itemID == removeObjects[1].GetComponent<Items>().itemID)
            {
                removeObjects[1].GetComponent<Items>().ActivateSlotImage();
                MoveObjectToTarget(slotImages.gameObject, counter.transform.position, 0.3f, 0f, iTween.EaseType.easeOutQuad);
                break;
            }
        }
        removeObjects[1].GetComponent<Items>().FinishAnimation();
        matchVFX.Play();
        StartCoroutine(RemoveCombileItems(_removeObjects));
        if (totalCounter <= 0)
        {
            OnGameOverPanel(true);
        }
    }

    List<GameObject> matchElement = new List<GameObject>();
    public void CheckMatchItems(int itemID)
    {
        matchElement.Clear();
        if (currItemNum > 2)
        {
            int temp = 0;
            for (int i = currItemNum - 1; i >= currItemNum - 3; i--)
            {
                if (selectedItemList[i].GetComponent<Items>().itemID == selectedItem.GetComponent<Items>().itemID)
                {
                    temp++;
                    matchElement.Add(selectedItemList[i]);
                }
            }
            if (temp >= 3)
            {
                MatchAndRemove();//StartCoroutine(MatchAndRemove());
            }
            else
            {
                if (selectedItemList.Count > 6)
                {
                    OnGameOverPanel(false);
                    Debug.LogError(selectedItemList.Count);
                    return;
                }
                else
                    return;
            }
        }
    }
    GameObject currentCounter;
    void MatchAndRemove()
    {
        outSpaceAnim.SetTrigger("OutSpaceExit");
        isOutSpace = false;
        int posInd = matchElement[1].GetComponent<Items>().posIndex;

        MoveObjectToTarget(matchElement[0], slotArray[posInd].position, 0.2f, 0.4f, iTween.EaseType.easeInBack);
        MoveObjectToTarget(matchElement[2], slotArray[posInd].position, 0.2f, 0.4f, iTween.EaseType.easeInBack);
        matchVFX.transform.localPosition = slotArray[posInd].localPosition;
        matchVFX.Stop();

        slotImages.texture = itemTexture[matchElement[1].GetComponent<Items>().itemID];
        slotImages.transform.localPosition = slotPos[posInd].localPosition;
        foreach (ItemCounter counter in itemCounters)
        {
            if (counter.itemID == matchElement[1].GetComponent<Items>().itemID)
            {
                matchElement[1].GetComponent<Items>().ActivateSlotImage();
                MoveObjectToTarget(slotImages.gameObject, counter.transform.position, 0.3f, 0.3f, iTween.EaseType.easeOutQuad);
                currentCounter = counter.gameObject;
                break;
            }
        }
        matchElement[1].GetComponent<Items>().FinishAnimation();
        matchVFX.Play();

        StartCoroutine(RemoveCombileItems(matchElement.ToArray()));
    }

    void ChangePosition()
    {
        for (int i = 0; i < selectedItemList.Count; i++)
        {
            //selectedItemList[i].GetComponent<Items>().ChangePosion(slotArray[i].position);
            MoveObjectToTarget(selectedItemList[i], slotArray[i].position, 0.2f, 0.3f, iTween.EaseType.easeOutQuad);
            selectedItemList[i].GetComponent<Items>().posIndex = i;
        }
    }

    void EnableMagnet()
    {
        magnetButton.enabled = true;
    }
    public void PowerUpAutoMatch()
    {
        if (!isInput)
            return;
        Items[] itemList = FindObjectsOfType<Items>();
        if (itemList.Length < 1)
            return;
        magnetButton.enabled = false;
        Invoke("EnableMagnet", 0.5f);
        GameObject[] autoSelectList = new GameObject[3];
        int j = 0;

        int autoIndex = 10;
        int autoID = 100;
        IEnumerable<IGrouping<int, GameObject>> GroupByMS = selectedItemList.GroupBy(x => x.GetComponent<Items>().itemID);
        foreach (IGrouping<int, GameObject> group in GroupByMS)
        {
            //Debug.LogError(group.Key + " : " + group.Count());
            if (group.Count() >= 3)
            {
                autoID = group.Key;
                if (selectedItemList.Count == 3)
                {
                    autoIndex = 0;
                }
                else if (selectedItemList.Count == 4)
                {
                    autoIndex = 1;
                }
                else if (selectedItemList.Count >= 5)
                {
                    autoIndex = 2;
                }
            }
        }

        if (selectedItemList.Count == 0 || autoIndex == 0)
        {
            //AutoMatch();
            ItemCounter _itemCounter;
            for (int m = 0; m < 5; m++)
            {
                if (itemCounters[m].itemCount > 0)
                {
                    _itemCounter = itemCounters[m];
                    for (int i = 0; i < itemList.Length; i++)
                    {
                        if (_itemCounter.itemID == itemList[i].itemID && autoID != itemList[i].itemID)
                        {
                            //selectedItemList.Add(item.gameObject);
                            itemList[i].DisableClickable();
                            selectedItemList.Add(itemList[i].gameObject);
                            itemStack.Add(itemList[i].gameObject);
                            autoSelectList[j] = itemList[i].gameObject;
                            MoveObjectToTarget(autoSelectList[j], slotArray[j].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
                            RotateObjectToTarget(autoSelectList[j], Vector3.zero, 0.3f, 0, iTween.EaseType.easeOutQuad);
                            autoSelectList[j].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                            autoSelectList[j].GetComponent<Items>().posIndex = j;
                            _itemCounter.itemCount -= 1;
                            totalCounter--;
                            _itemCounter.SetItemCount();
                            //    }
                            //}
                            if (j >= 2)
                                break;

                            j++;
                        }
                    }
                    break;
                }
            }
            AudioManager.instance.PlayMagnetEffects();
            RemoveElement(autoSelectList);
            if (PlayerPrefs.GetInt("MagnetTutorial") == 0)
            {
                PlayerPrefs.SetInt("MagnetTutorial", 1);
                isInput = true;
                GamePlayMenuPanel.Instance.SetMagnet();
            }
            else
            {
                Loader.Instance.magnetCount--;
                PlayerPrefs.SetInt("MagnetCount", Loader.Instance.magnetCount);
                GamePlayMenuPanel.Instance.SetMagnet();
            }
        }
        else if (selectedItemList.Count == 1 || autoIndex == 1)
        {
            autoSelectList[j] = selectedItemList[0];
            j++;
            for (int k = 0; k < itemList.Length; k++)
            {
                if (selectedItemList[0].GetComponent<Items>().itemID == itemList[k].itemID && itemList[k].isClickable && itemList[k].itemID != autoID)
                {
                    autoSelectList[j] = itemList[k].gameObject;
                    selectedItemList.Add(autoSelectList[j]);
                    itemList[k].DisableClickable();
                    itemStack.Add(autoSelectList[j]);
                    MoveObjectToTarget(autoSelectList[j], slotArray[j].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
                    RotateObjectToTarget(autoSelectList[j], Vector3.zero, 0.3f, 0, iTween.EaseType.easeOutQuad);
                    autoSelectList[j].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    autoSelectList[j].GetComponent<Items>().posIndex = j;
                    for (int i = 0; i < 5; i++)
                    {
                        if (itemList[k].itemID == itemCounters[i].itemID)
                        {
                            itemCounters[i].itemCount -= 1;
                            totalCounter--;
                            itemCounters[i].SetItemCount();
                        }
                    }
                    if (j >= 2)
                        break;
                    j++;
                }
            }
            AudioManager.instance.PlayMagnetEffects();
            RemoveElement(autoSelectList);// StartCoroutine(RemoveElement(autoSelectList));
                                          //StartCoroutine(RemoveCombileItems(autoSelectList));
            Loader.Instance.magnetCount--;
            PlayerPrefs.SetInt("MagnetCount", Loader.Instance.magnetCount);
            GamePlayMenuPanel.Instance.SetMagnet();

        }
        else
        {
            for (int i = 0; i < selectedItemList.Count - 1; i++)
            {
                if (selectedItemList[i].GetComponent<Items>().itemID == selectedItemList[i + 1].GetComponent<Items>().itemID && autoID != selectedItemList[i + 1].GetComponent<Items>().itemID)
                {
                    foreach (Items item in itemList)
                    {
                        if (selectedItemList[i].GetComponent<Items>().itemID == item.itemID && item.isClickable)
                        {
                            if (selectedItemList.Count > 6)
                                return;
                            selectedItemList.Insert(i + 2, item.gameObject);
                            itemStack.Add(item.gameObject);
                            item.DisableClickable();
                            for (int k = 0; k < 5; k++)
                            {
                                if (item.itemID == itemCounters[k].itemID)
                                {
                                    itemCounters[k].itemCount -= 1;
                                    totalCounter--;
                                    itemCounters[k].SetItemCount();
                                }
                            }
                            break;
                        }
                    }
                    SelectOne(i);//StartCoroutine(SelectOne(i));

                    return;
                }
            }
            if (selectedItemList.Count > 5)
                return;
            SelectRandom();//StartCoroutine(SelectRandom());
        }

        //for (int i = 0; i < selectedItemList.Count; i++)
        //{
        //    //selectedItemList[i].GetComponent<Items>().ChangePosion(slotArray[i].position);
        //    MoveObjectToTarget(selectedItemList[i], slotArray[i].position, 0.2f, 0.4f, iTween.EaseType.easeOutQuad);
        //    selectedItemList[i].GetComponent<Items>().posIndex = i;
        //}
    }

    void AutoMatch()
    {
        Items[] itemList = FindObjectsOfType<Items>();
        if (itemList.Length < 1)
            return;

        GameObject[] autoSelectList = new GameObject[3];
        int j = 0;
        ItemCounter _itemCounter;
        for (int m = 0; m < 5; m++)
        {
            if (itemCounters[m].itemCount > 0)
            {
                _itemCounter = itemCounters[m];
                for (int i = 0; i < itemList.Length; i++)
                {
                    if (_itemCounter.itemID == itemList[i].itemID)
                    {
                        //selectedItemList.Add(item.gameObject);
                        itemList[i].DisableClickable();
                        autoSelectList[j] = itemList[i].gameObject;
                        itemStack.Add(autoSelectList[j]);
                        MoveObjectToTarget(autoSelectList[j], slotArray[j].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
                        RotateObjectToTarget(autoSelectList[j], Vector3.zero, 0.3f, 0, iTween.EaseType.easeOutQuad);
                        autoSelectList[j].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        autoSelectList[j].GetComponent<Items>().posIndex = j;
                        _itemCounter.itemCount -= 1;
                        totalCounter--;
                        _itemCounter.SetItemCount();
                        //    }
                        //}
                        if (j >= 2)
                            break;

                        j++;
                    }
                }
                break;
            }
        }

        RemoveElement(autoSelectList);// StartCoroutine(RemoveElement(autoSelectList));
    }

    void SelectOne(int _index)
    {
        //isInput = false;
        int index = _index + 3;
        for (int i = index; i < selectedItemList.Count; i++)
        {
            //selectedItemList[i].GetComponent<Items>().ChangePosion(slotArray[i].position);
            MoveObjectToTarget(selectedItemList[i], slotArray[i].position, 0.1f, 0, iTween.EaseType.easeOutQuad);
            selectedItemList[i].GetComponent<Items>().posIndex = i;
            //selectedItemList[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        //selectedItemList[index - 1].GetComponent<Items>().ChangePosion(slotArray[index - 1].position);
        MoveObjectToTarget(selectedItemList[index - 1], slotArray[index - 1].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
        selectedItemList[index - 1].GetComponent<Items>().posIndex = index - 1;
        RotateObjectToTarget(selectedItemList[index - 1], Vector3.zero, 0.3f, 0, iTween.EaseType.easeOutQuad);
        selectedItemList[index - 1].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //yield return new WaitForSeconds(0.3f);
        currItemNum = index;
        selectedItem = selectedItemList[index - 3];
        CheckMatchItems(selectedItem.GetComponent<Items>().itemID);
        AudioManager.instance.PlayMagnetEffects();
        Loader.Instance.magnetCount--;
        PlayerPrefs.SetInt("MagnetCount", Loader.Instance.magnetCount);
        GamePlayMenuPanel.Instance.SetMagnet();
    }

    void SelectRandom()
    {
        //isInput = false;
        int index = Random.Range(0, selectedItemList.Count);
        for (int i = index + 1; i < selectedItemList.Count; i++)
        {
            MoveObjectToTarget(selectedItemList[i], slotArray[i + 2].position, 0.1f, 0, iTween.EaseType.easeOutQuad);
            selectedItemList[i].GetComponent<Items>().posIndex = i + 1;
        }
        //yield return new WaitForSeconds(0.2f);
        Items[] itemList = FindObjectsOfType<Items>();
        List<Items> items = new List<Items>();
        foreach (Items item in itemList)
        {
            if (selectedItemList[index].GetComponent<Items>().itemID == item.itemID && item.isClickable)
            {
                items.Add(item);
                item.DisableClickable();
                for (int i = 0; i < 5; i++)
                {
                    if (item.itemID == itemCounters[i].itemID)
                    {
                        itemCounters[i].itemCount -= 1;
                        totalCounter--;
                        itemCounters[i].SetItemCount();
                    }
                }
            }

            if (items.Count >= 2)
                break;
        }
        for (int i = 1; i < 3; i++)
            selectedItemList.Insert(index + i, items[i - 1].gameObject);

        for (int i = index + 1; i < index + 3; i++)
        {
            MoveObjectToTarget(selectedItemList[i], slotArray[i].position, 0.3f, 0, iTween.EaseType.easeOutQuad);
            RotateObjectToTarget(selectedItemList[i], Vector3.zero, 0.3f, 0, iTween.EaseType.easeOutQuad);
            selectedItemList[i].GetComponent<Items>().posIndex = i;
            selectedItemList[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        //yield return new WaitForSeconds(0.3f);
        currItemNum = index + 3;
        selectedItem = selectedItemList[index];
        CheckMatchItems(selectedItem.GetComponent<Items>().itemID);
        AudioManager.instance.PlayMagnetEffects();
        Loader.Instance.magnetCount--;
        PlayerPrefs.SetInt("MagnetCount", Loader.Instance.magnetCount);
        GamePlayMenuPanel.Instance.SetMagnet();
    }

    void TestFunction()
    {
        int temp = 0;
        int tempID = 0;
        for (int i = 0; i < itemCounters.Length; i++)
        {
            temp = 0;
            tempID = 0;
            for (int j = 0; j < selectedItemList.Count; j++)
            {
                if (selectedItemList[j].GetComponent<Items>().itemID == itemCounters[i].itemID)
                {
                    temp++;
                    tempID = itemCounters[i].itemID;
                }
            }
            if (temp == 2)
            {
                //SelectOne(tempID);
                return;
            }
            else if (temp == 1)
            {
                //SelectTwo(tempID);
                return;
            }
        }
        if (temp == 0)
        {
            //SelectThree();
            return;
        }
    }

    public void PowerUPUndo()
    {
        if (itemStack.Count == 0 || isCoroutine)
            return;

        AudioManager.instance.PlayUndoEffects();
        for (int i = itemStack.Count - 1; i >= 0; i++)
        {
            if (itemStack[i] != null)
            {
                itemStack[i].transform.parent = unselectedParent;
                ResetLastItem(itemStack[i]);// StartCoroutine(ResetLastItem(itemStack[i]));
                itemStack.RemoveAt(i);
                if (PlayerPrefs.GetInt("UndoTutorial") == 0)
                {
                    PlayerPrefs.SetInt("UndoTutorial", 1);
                    isInput = true;
                    GamePlayMenuPanel.Instance.SetUndo();
                }
                else
                {
                    Loader.Instance.undoCount--;
                    PlayerPrefs.SetInt("UndoCount", Loader.Instance.undoCount);
                    GamePlayMenuPanel.Instance.SetUndo();
                }
                break;
            }
        }
    }


    IEnumerator LightningBlast()
    {
        
        isInput = false;
        if (Loader.Instance.energyUnlockLevel != Loader.Instance.currLevel)
        {
            Loader.Instance.energyCount--;
            PlayerPrefs.SetInt("EnergyCount", Loader.Instance.energyCount);
        }
        yield return new WaitForSeconds(1.5f);
        bool isInclude = false;
        int junkCount = 0;
        GameObject[] junkItemArray = new GameObject[6];
        GameObject[] vfxArray = new GameObject[6];
        for (int i = 0; i < totalItemList.Count; i++)
        {
            isInclude = false;
            for (int j = 0; j < itemCounters.Length; j++)
            {
                if (totalItemList[i].itemID == itemCounters[j].itemID)
                {
                    isInclude = true;
                    break;
                }
            }
            if (!isInclude)
            {
                if (totalItemList[i].isClickable)
                {
                    junkItemArray[junkCount] = totalItemList[i].gameObject;
                    junkCount++;
                    if (junkCount >= 6)
                        break;
                }
            }
        }
        for (int i = 0; i < junkItemArray.Length; i++)
        {
            GameObject go = Instantiate(lightningVFX);
            vfxArray[i] = go;
            Vector3 tempVect = new Vector3(junkItemArray[i].transform.position.x, junkItemArray[i].transform.position.y + 0.5f, junkItemArray[i].transform.position.z);
            go.transform.position = tempVect;
            go.GetComponent<ParticleSystem>().Play();
        }
        AudioManager.instance.PlayEnergyEffects();
        //yield return new WaitForSeconds(0.6f);
        //for (int i = 0; i < junkItemArray.Length; i++)
        //    junkItemArray[i].GetComponent<Items>().BurnAnimation();

        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < junkItemArray.Length; i++)
        {
            Destroy(junkItemArray[i]);
            Destroy(vfxArray[i]);
        }
        System.GC.Collect();
        isInput = true;
    }

    [SerializeField] Animator platformAnim;
    bool fan = false;
    public void PowerUPFan()
    {
        isInput = true;
        fan = true;
        AudioManager.instance.PlayShuffleEffects();
        if (PlayerPrefs.GetInt("ShuffleTutorial") == 0)
        {
            PlayerPrefs.SetInt("ShuffleTutorial", 1);
            GamePlayMenuPanel.Instance.SetShuffle();
        }
        else
        {
            Loader.Instance.fanCount--;
            PlayerPrefs.SetInt("FanCount", Loader.Instance.fanCount);
            GamePlayMenuPanel.Instance.SetShuffle();
        }
        platformAnim.SetTrigger("platform");
        foreach (Items item in totalItemList)
        {
            if (item != null && item.isClickable)
            {
                item.rb.isKinematic = false;
                item.rb.angularDrag = 0.05f;
                item.SetShuffle();
                //item.isFan = true;
                //fanObject.SetActive(true);

            }
        }
        //fanBlocker.SetActive(true);
        //Vector3 _tarPos = new Vector3(unselectedParent.position.x, unselectedParent.position.y+1, unselectedParent.position.z);
        //MoveObjectToTarget(unselectedParent.gameObject, _tarPos, 1, 0, iTween.EaseType.easeInQuad);
        Invoke("DisableFan", 0.05f);
        //StopCoroutine(DisableFan());
        //StartCoroutine(DisableFan());
    }

    void DisableFan()
    {
        //yield return new WaitForFixedUpdate();
        foreach (Items item in totalItemList)
        {
            if (item != null && item.isClickable)
            {
                item.isFan = false;
                item.rb.angularDrag = 2;
                item.rb.drag = 2;
                item.rb.mass = 5;
            }
        }
        //fan = false;
        //yield return new WaitForSeconds(4);
        //if (!fan)
        //{
        //    foreach (Items item in totalItemList)
        //    {
        //        if (item != null && item.isClickable)
        //        {
        //            item.rb.isKinematic = true;
        //        }
        //    }
        //}
        //fanBlocker.SetActive(false);
        //fanObject.SetActive(false);
        GamePlayMenuPanel.Instance.isShuffle = false;
    }

    public void FreezeTime()
    {
        isInput = true;
        if (PlayerPrefs.GetInt("FreezeTutorial") == 0)
        {
            PlayerPrefs.SetInt("FreezeTutorial", 1);
            GamePlayMenuPanel.Instance.SetFreeze();
        }
        else
        {
            Loader.Instance.freezeCount--;
            PlayerPrefs.SetInt("FreezeCount", Loader.Instance.freezeCount);
            GamePlayMenuPanel.Instance.SetFreeze();
        }
        AudioManager.instance.PlayFreezeEffects();
        timeCounter.timerIsRunning = false;
        freezeVFX.SetActive(true);
        Invoke("UnFreazeGame", 10);
    }
    void UnFreazeGame()
    {
        timeCounter.timerIsRunning = true;
        freezeVFX.SetActive(false);
        GamePlayMenuPanel.Instance.isFreeze = false;
    }

    public void SetUpUndoTutorial()
    {
        foreach (Items item in totalItemList)
        {
            int tempCount = 0;
            if (item != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (item.itemID == itemCounters[i].itemID)
                        tempCount++;
                }
                if (tempCount == 0)
                {
                    selectedItemList.Add(item.gameObject);
                    itemStack.Add(item.gameObject);
                    item.DisableClickable();
                    item.transform.position = slotArray[0].position;
                    item.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    return;
                }
            }
            
        }
    }

    void ResetLastItem(GameObject go)
    {
        go.transform.GetChild(0).GetComponent<Animator>().enabled = false;
        MoveObjectToTarget(go, itemResetTrans.position, 0.3f, 0, iTween.EaseType.easeOutQuad);
        if (go.GetComponent<Items>().posIndex < selectedItemList.Count - 1)
        {
            for (int i = go.GetComponent<Items>().posIndex; i < selectedItemList.Count - 1; i++)
            {
                MoveObjectToTarget(selectedItemList[i + 1], slotArray[i].position, 0.2f, 0, iTween.EaseType.easeOutQuad);
                selectedItemList[i + 1].GetComponent<Items>().posIndex = i;
            }
        }
        selectedItemList.RemoveAt(selectedItemList.Count - 1);
        currItemNum = selectedItemList.Count;
        go.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        selectedItem = null;
        //yield return new WaitForSeconds(0.2f);
        go.GetComponent<Items>().EnableClickable();
        for (int i = 0; i < 5; i++)
        {
            if (go.GetComponent<Items>().itemID == itemCounters[i].itemID)
            {
                itemCounters[i].itemCount += 1;
                totalCounter++;
                itemCounters[i].SetItemCount();
            }
        }
        //if (selectedItemList.Count == 6)
        //{
        //    outSpaceAnim.SetTrigger("OutSpaceEnter");
        //    isOutSpace = true;
        //}
        if (selectedItemList.Count < 6 && isOutSpace)
        {
            outSpaceAnim.SetTrigger("OutSpaceExit");
            isOutSpace = false;
        }
    }

    public void OnGameOverPanel(bool stat)
    {
        isInput = false;
        ChangePosition();
        if (stat)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelTitleText.text);
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
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelTitleText.text);
            AudioManager.instance.PlayFailEffects();
            if (Loader.Instance.lifeCount > 4)
                PlayerPrefs.SetString("LifeLostTime", System.DateTime.Now.ToBinary().ToString());

            Loader.Instance.lifeCount--;
            if (Loader.Instance.lifeCount <= 0)
                Loader.Instance.lifeCount = 0;
            PlayerPrefs.SetInt("LifeCount", Loader.Instance.lifeCount);
            GamePlayMenuPanel.Instance.InitGameFailPanel();
            gameFail.SetActive(true);
            loseTimeText.text = timeCounter.GetTime();
        }
    }

    IEnumerator ShowStar(int index)
    {
        AudioManager.instance.PlayWinEffects();
        Loader.Instance.isGotStar = true;
        //Loader.Instance.currLevel++;
        //PlayerPrefs.SetInt("CurrentLevel", Loader.Instance.currLevel);
        //Loader.Instance.starCount++;
        //PlayerPrefs.SetInt("StarCount", Loader.Instance.starCount);

        if (Loader.Instance.isHardLevel)
        {
            //Loader.Instance.coinCount += 500;
            //PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);
            Loader.Instance.gotCoinChest = true;
        }
        gameVictoryConfetti.SetActive(true);
        yield return new WaitForSeconds(1);
        //if (Loader.Instance.currLevel > 20)
        //    Loader.Instance.currLevel = 20;

        //SceneManager.LoadScene(1);
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
        for (int i = 0; i < 5; i++)
        {
            if (itemCounters[i].itemCount > 0)
            {
                MoveObjectToTarget(itemCounters[i].gameObject, itemCountersPos[j].position, 0.1f, 0, iTween.EaseType.easeInBack);
                j++;
            }
        }
    }

    public void AtivateSlotImage()
    {
        slotImages.gameObject.SetActive(true);
    }

    bool boundaryMove = false;
    void BoundaryMove()
    {
        boundaryMove = true;
        MoveObjectToTarget(upperBounary[0], upperBounary[0].transform.position+new Vector3(0.5f,0,0), 0.5f, 0, iTween.EaseType.linear);
        MoveObjectToTarget(upperBounary[1], upperBounary[1].transform.position + new Vector3(-0.5f, 0, 0), 0.5f, 0, iTween.EaseType.linear);
        MoveObjectToTarget(upperBounary[2], upperBounary[2].transform.position + new Vector3(0, -0.5f, 0), 0.5f, 0, iTween.EaseType.linear);
        MoveObjectToTarget(upperBounary[3], upperBounary[3].transform.position + new Vector3(0, 0.5f, 0), 0.5f, 0, iTween.EaseType.linear);

        MoveObjectToTarget(upperBounary[0], upperBounary[0].transform.position + new Vector3(-0.5f, 0, 0), 0.5f, 0.5f, iTween.EaseType.linear);
        MoveObjectToTarget(upperBounary[1], upperBounary[1].transform.position + new Vector3(0.5f, 0, 0), 0.5f, 0.5f, iTween.EaseType.linear);
        MoveObjectToTarget(upperBounary[2], upperBounary[2].transform.position + new Vector3(0, 0.5f, 0), 0.5f, 0.5f, iTween.EaseType.linear);
        MoveObjectToTarget(upperBounary[3], upperBounary[3].transform.position + new Vector3(0, -0.5f, 0), 0.5f, 0.5f, iTween.EaseType.linear);
    }

    public void MoveObjectToTarget(GameObject thisGameOBJ, Vector3 targetPos, float durationMove, float delayTime, iTween.EaseType easeType)
    {
        iTween.MoveTo(thisGameOBJ, iTween.Hash("position", targetPos, "time", durationMove, "delay", delayTime, "easeType", easeType));
    }

    public void RotateObjectToTarget(GameObject thisGameOBJ, Vector3 targetRot, float durationRot, float delayTime, iTween.EaseType easeType)
    {
        iTween.RotateTo(thisGameOBJ, iTween.Hash("rotation", targetRot, "time", durationRot, "delay", delayTime, "easeType", easeType));
    }

    public void PauseGamePlay(bool stat)
    {
        if (stat)
        {
            timeCounter.timerIsRunning = false;
            isInput = false;
        }
        else
        {
            timeCounter.timerIsRunning = true;
            isInput = true;
        }
    }

    public void QuitGamePlay()
    {
        if (Loader.Instance.lifeCount > 4)
            PlayerPrefs.SetString("LifeLostTime", System.DateTime.Now.ToBinary().ToString());

        Loader.Instance.lifeCount--;
        PlayerPrefs.SetInt("LifeCount", Loader.Instance.lifeCount);
        SceneManager.LoadScene(1);
    }

    bool isButton = false;
    public void NextScene()
    {
        if (isButton)
            return;
        isButton = true;
        if (Loader.Instance.currLevel > 20)
            Loader.Instance.currLevel = 5;

        if (Loader.Instance.currLevel < 3)
        {
            Loader.Instance.currLevel++;
            Loader.Instance.starCount++;
            PlayerPrefs.SetInt("CurrentLevel", Loader.Instance.currLevel);
            PlayerPrefs.SetInt("StarCount", Loader.Instance.starCount);
            //gameVictory.GetComponent<Animator>().SetTrigger("PanelOut"); 
            gameVictory.SetActive(false);
            GamePlayMenuPanel.Instance.ShowLevelPanel();
            AudioManager.instance.PlayClickEffects();
            return;
        }
        else
        {
            if (Loader.Instance.currLevel == 3)
            {
                //Loader.Instance.currLevel++;
                Loader.Instance.starCount++;
                //PlayerPrefs.SetInt("CurrentLevel", Loader.Instance.currLevel);
                PlayerPrefs.SetInt("StarCount", Loader.Instance.starCount);
            }
            AudioManager.instance.PlayClickEffects();
            gameVictory.GetComponent<Animator>().SetTrigger("PanelOut");
            SceneManager.LoadScene(1);
        }




    }

}
