using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using GameAnalyticsSDK;

[Serializable]
public class ConsumableItem
{
    public string Name;
    public string Id;
    public string desc;
    public float price;
}

public class InAppShop : MonoBehaviour, IDetailedStoreListener
{
    IStoreController storeController;

    [SerializeField] ConsumableItem[] consumables;
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] RectTransform rectTransform;

    public Data data;
    public Payload payload;
    public PayloadData payloadData;
    public SkuDetails skuDetails;

    // Start is called before the first frame update
    void Start()
    {
        SetupBuilder();
    }

    void OnEnable()
    {
        //verticalLayoutGroup.enabled = true;
        StartCoroutine(VerticalLayout());
    }

    IEnumerator VerticalLayout()
    {
        //verticalLayoutGroup.enabled = false;
        yield return new WaitForSeconds(0.05f);
        rectTransform.anchoredPosition = new Vector3(381, -1092);
        //verticalLayoutGroup.enabled = true;
    }

    //void OnDisable()
    //{
    //    verticalLayoutGroup.enabled = false;
    //}

    void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(consumables[0].Id, ProductType.Consumable);//copper
        builder.AddProduct(consumables[1].Id, ProductType.Consumable);//bronze
        builder.AddProduct(consumables[2].Id, ProductType.Consumable);//silver
        builder.AddProduct(consumables[3].Id, ProductType.Consumable);//gold
        builder.AddProduct(consumables[4].Id, ProductType.Consumable);//platinum

        UnityPurchasing.Initialize(this, builder);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void Consumable_Btn_Copper_Pressed()
    {
        //AddCoins(500);
        storeController.InitiatePurchase(consumables[0].Id);
    }

    public void Consumable_Btn_Bronze_Pressed()
    {
        //AddCoins(1000);
        storeController.InitiatePurchase(consumables[1].Id);
    }

    public void Consumable_Btn_Silver_Pressed()
    {
        //AddCoins(1500);
        storeController.InitiatePurchase(consumables[2].Id);
    }

    public void Consumable_Btn_Gold_Pressed()
    {
        //AddCoins(2000);
        storeController.InitiatePurchase(consumables[3].Id);
    }
    public void Consumable_Btn_Platinum_Pressed()
    {
        //AddCoins(2000);
        storeController.InitiatePurchase(consumables[4].Id);
    }

    void AddCoins(int _coin, int xPerk)
    {
        Loader.Instance.coinCount += _coin;
        PlayerPrefs.SetInt("CoinCount", Loader.Instance.coinCount);

        Loader.Instance.magnetCount += xPerk;
        PlayerPrefs.SetInt("MagnetCount", Loader.Instance.magnetCount);

        Loader.Instance.undoCount += xPerk;
        PlayerPrefs.SetInt("UndoCount", Loader.Instance.undoCount);

        Loader.Instance.fanCount += xPerk;
        PlayerPrefs.SetInt("FanCount", Loader.Instance.fanCount);

        Loader.Instance.freezeCount += xPerk;
        PlayerPrefs.SetInt("FreezeCount", Loader.Instance.freezeCount);

        Loader.Instance.energyCount += xPerk;
        PlayerPrefs.SetInt("EnergyCount", Loader.Instance.energyCount);

        Loader.Instance.timerCount += xPerk;
        PlayerPrefs.SetInt("TimerCount", Loader.Instance.timerCount);

        MainMenu.Instance.CoinCounter();
    }

    public void ClosePanel()
    {
        AudioManager.instance.PlayClickEffects();
        gameObject.SetActive(false);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Initialize failed" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("initialize failed" + error + message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;

        Debug.Log("Purchase Complete" + product.definition.id);
        string receipt = product.receipt;
        data = JsonUtility.FromJson<Data>(receipt);
        payload = JsonUtility.FromJson<Payload>(data.Payload);
        payloadData = JsonUtility.FromJson<PayloadData>(payload.json);
        //skuDetails = JsonUtility.FromJson<SkuDetails>(payload.skuDetails);
        string _currency = string.Empty;
        int _price = 0;
        string _productId = string.Empty;
        foreach (SkuDetails skuDetail in payload.skuDetails)
        {
            _currency = skuDetail.price_currency_code;
            _price = int.Parse(skuDetail.price);
            _productId = skuDetail.productId;
        }
        GameAnalytics.NewBusinessEvent(_currency, _price, "itemType", _productId, "cartType");
        int quantity = payloadData.quantity;
        if (product.definition.id == consumables[0].Id)//copper
        {
            for (int i = 0; i < quantity; i++)
                AddCoins(5000, 1);
        }
        else if (product.definition.id == consumables[1].Id)//bronze
        {
            for (int i = 0; i < quantity; i++)
                AddCoins(10000, 3);
        }
        else if (product.definition.id == consumables[2].Id)//silver
        {
            for (int i = 0; i < quantity; i++)
                AddCoins(25000, 6);
        }
        else if (product.definition.id == consumables[3].Id)//gold
        {
            for (int i = 0; i < quantity; i++)
                AddCoins(50000, 12);
        }
        else if (product.definition.id == consumables[4].Id)//platinum
        {
            //for (int i = 0; i < quantity; i++)
                AddCoins(65000, 24);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("purchase failed" + failureReason);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Initialized");
        storeController = controller;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        print("purchase failed" + failureDescription);
    }
}

[Serializable]
public class SkuDetails
{
    public string productId;
    public string type;
    public string title;
    public string name;
    public string iconUrl;
    public string description;
    public string price;
    public long price_amount_micros;
    public string price_currency_code;
    public string skuDetailsToken;
}

[Serializable]
public class PayloadData
{
    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public string purchaseToken;
    public int quantity;
    public bool acknowledged;
}

[Serializable]
public class Payload
{
    public string json;
    public string signature;
    public List<SkuDetails> skuDetails;
    public PayloadData payloadData;
}

[Serializable]
public class Data
{
    public string Payload;
    public string Store;
    public string TransactionID;
}
