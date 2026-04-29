using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GeneratingItems : MonoBehaviour
{
    [Header("Items")]
    //public GameObject[] items;
    //[SerializeField] GameObject itemsCounterPref;
    //[SerializeField] Transform counterTrans;
    [SerializeField] Transform itemParent;
    [SerializeField] Texture[] counterTexture;
    [SerializeField] GameObject[] spawnBoundary;
    [SerializeField] TimeCounter timeCounter;
    [SerializeField] GameObject blackScreen;
    [SerializeField] GameObject platformCollider;
    //public GameObject disappear_VFX;

    void Start()
    {
        blackScreen.SetActive(true);
       // GenerateItems();
    }

    public void GenerateItems()
    {
        //StartCoroutine(RandomSpawnCO());
        StartCoroutine(DeSerializeLevel());
    }

    //IEnumerator RandomSpawnCO()
    //{
    //    //yield return new WaitForSeconds(0.1f);
    //    int j = 0;
    //    List<int> spawnSpot = new List<int>();
    //    for (int l = 0; l < 3; l++)
    //    {
    //        j = 0;
    //        for (int i = 0; i < items.Length; i++)
    //            spawnSpot.Add(i);
    //        for (int k = 0; k < items.Length; k++)
    //        {
    //            int random = Random.Range(0, spawnSpot.Count);
    //            j = spawnSpot[random];
    //            GameObject go1 = Instantiate(items[j], itemParent);
    //            go1.transform.localPosition = spawnBoundary[k].transform.position;
    //            GameManager.Instance.totalItemList.Add(go1.GetComponent<Items>());
    //            spawnSpot.RemoveAt(random);
    //        }

    //        for (int i = 0; i < items.Length; i++)
    //            spawnSpot.Add(i);
    //        for (int k = 16; k < items.Length+16; k++)
    //        {
    //            int random = Random.Range(0, spawnSpot.Count);
    //            j = spawnSpot[random];
    //            GameObject go2 = Instantiate(items[j], itemParent);
    //            go2.transform.localPosition = spawnBoundary[k].transform.position;
    //            GameManager.Instance.totalItemList.Add(go2.GetComponent<Items>());
    //            spawnSpot.RemoveAt(random);
    //        }

    //        for (int i = 0; i < items.Length; i++)
    //            spawnSpot.Add(i);
    //        for (int k = 32; k < items.Length+32; k++)
    //        {
    //            int random = Random.Range(0, spawnSpot.Count);
    //            j = spawnSpot[random];
    //            GameObject go3 = Instantiate(items[j], itemParent);
    //            go3.transform.localPosition = spawnBoundary[k].transform.position;
    //            GameManager.Instance.totalItemList.Add(go3.GetComponent<Items>());
    //            spawnSpot.RemoveAt(random);
    //        }
    //    }
    //    SetItemCounter();
    //    yield return new WaitForSeconds(2.0f);
    //    foreach (Items item in GameManager.Instance.totalItemList)
    //    {
    //        //Debug.LogError("here1");
    //        item.GetComponent<Rigidbody>().isKinematic = true;
    //    }

    //}

    IEnumerator DeSerializeLevel()
    {
        //string jsonPath = Application.dataPath + "/Json/Level_" + (Loader.Instance.currLevel-1).ToString() + ".json";
        //string content = File.ReadAllText(jsonPath);
        string jsonPath = "Json/Level_"+ Loader.Instance.currLevel.ToString();
        //Debug.LogError(jsonPath);
        var targetFile = Resources.Load<TextAsset>(jsonPath);
        LevelInfo levelInfo = JsonUtility.FromJson<LevelInfo>(targetFile.text);
        yield return null;
        //Item Spawn
        string itemPath = "Items/Item_0";
        List<int> spawnSpot = new List<int>();
        timeCounter.totalTime = levelInfo.timer;
        if (levelInfo.isHardLevel)
            Loader.Instance.isHardLevel = true;
        else
            Loader.Instance.isHardLevel = false;
        if (Loader.Instance.isTimerSelect)
        {
            if (Loader.Instance.timerUnlockLevel != Loader.Instance.currLevel)
            {
                Loader.Instance.timerCount--;
                PlayerPrefs.SetInt("TimerCount", Loader.Instance.timerCount);
            }
            Loader.Instance.isTimerSelect = false;
            timeCounter.totalTime += 30;
        }
        for (int k = 0; k < levelInfo.totalItemCount; k++)
            spawnSpot.Add(k);

        for (int i = 0; i < levelInfo.LevelItems.Length; i++)
        {
            itemPath = "Items/Item_" + levelInfo.LevelItems[i];
            for (int j = 0; j < levelInfo.LevelItemsCount[i]; j++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>(itemPath), itemParent);//Instantiate(items[levelInfo.LevelItems[i]], itemParent);
                GameManager.Instance.totalItemList.Add(go.GetComponent<Items>());
                int random = Random.Range(0, spawnSpot.Count);
                go.transform.localPosition = spawnBoundary[spawnSpot[random]].transform.position;
                //go.transform.rotation = Random.rotation;
                float rnd = Random.Range(0,360);
                go.transform.rotation = Quaternion.Euler(0, rnd, 0);
                spawnSpot.RemoveAt(random);
                go.SetActive(true);
            }
        }

        //Counter setting
        for (int i = 0; i < levelInfo.CounterItem.Length; i++)
        {
            GameManager.Instance.itemCounters[i].gameObject.SetActive(true);
            GameManager.Instance.itemCounters[i].itemTex.texture = counterTexture[levelInfo.CounterItem[i]];
            GameManager.Instance.itemCounters[i].itemID = levelInfo.CounterItem[i];
            GameManager.Instance.itemCounters[i].itemCount = levelInfo.CounterItemCount[i];
            GameManager.Instance.itemCounters[i].SetItemCount();
        }
        GameManager.Instance.totalCounter = levelInfo.totalCounterCount;
        yield return new WaitForSeconds(0.5f);

        if (Loader.Instance.undoUnlockLevel == Loader.Instance.currLevel && PlayerPrefs.GetInt("UndoTutorial") == 0)
            GameManager.Instance.SetUpUndoTutorial();

        platformCollider.GetComponent<Animator>().SetTrigger("platform");
        yield return new WaitForSeconds(0.5f);
        platformCollider.SetActive(false);
        yield return null;
        blackScreen.SetActive(false);
        timeCounter.InitTimer();
        if (GameManager.Instance.isTutorial)
            timeCounter.timerIsRunning = false;
        yield return new WaitForSeconds(1.0f);
        foreach (Items item in GameManager.Instance.totalItemList)
        {
            if (item != null)
            {
                item.rb.angularDrag = 10;
            }
        }
    }

    //public void DisableBoundary()
    //{
    //    foreach (Collider col in spawnBoundary)
    //        col.enabled = false;
    //}
    //public Items[] itemList;
    //void SetItemCounter()
    //{
    //    int counterNum = Random.Range(1, 5);
    //    itemList = FindObjectsOfType<Items>();
    //    for (int i = 0; i < counterNum; i++)
    //    {
    //        //int randomId = Random.Range(0, 13);
    //        GameManager.Instance.itemCounters[i].gameObject.SetActive(true);
    //        //go.transform.position =Vector3.zero;
    //        //go.transform.localPosition = new Vector3(0, 1, 0);
    //        GameManager.Instance.itemCounters[i].itemTex.texture = counterTexture[i];
    //        GameManager.Instance.itemCounters[i].itemID = i;
    //        for (int j = 0; j < itemList.Length; j++)
    //        {
    //            if (itemList[j].itemID == i)
    //            {
    //                GameManager.Instance.itemCounters[i].itemCount++;
    //                GameManager.Instance.totalCounter++;
    //            }
    //        }
    //        GameManager.Instance.itemCounters[i].SetItemCount();
    //    }

    //}

    //public void DestroyObjects()
    //{

    //    foreach (Transform child in transform)
    //    {
    //        GameObject VFX = Instantiate(disappear_VFX, child.position, Quaternion.identity);
    //        Destroy(child.gameObject);
    //        Destroy(VFX, 0.5f);
    //        // Debug.Log(child.name);
    //    }
    //}

    //  void OnDrawGizmosSelected()
    //void OnDrawGizmos()
    //{
    //    // Draw a semitransparent blue cube at the transforms position
    //    Gizmos.color = new Color(1, 0, 0, 0.5f);
    //    Gizmos.DrawCube(transform.position, new Vector3(3.2f, 1, 3f));
    //}
}
