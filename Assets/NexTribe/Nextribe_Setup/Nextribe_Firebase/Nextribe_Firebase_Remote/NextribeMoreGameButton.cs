
using Nextribe.Analytics;
using NexTribe;
using UnityEngine;
using UnityEngine.UI;

namespace Nextribe.Firebase
{
    public class NextribeMoreGameButton : MonoBehaviour
    {
        private Button button;
        [Header("URL Setup")]
        [SerializeField]
        private URLFrom uRLFrom;
        [SerializeField]
        private string LocalURLLink;
        private RemoteConfigURLLinkDataSO remoteConfigURLLinkDataSO;
        [SerializeField]
        private RemoteConfigURLLinkDataSO remoteConfigURLLinkDataSOAndroid;
        [SerializeField]
        private RemoteConfigURLLinkDataSO remoteConfigURLLinkDataSOIOS;

        private void Awake()
        {
            button = GetComponent<Button>();
            Init();
        }
        private void OnEnable()
        {
            button.onClick.AddListener(OnMoreGamesButton);
        }
        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }

        private void Init()
        {
#if UNITY_ANDROID              
            remoteConfigURLLinkDataSO = remoteConfigURLLinkDataSOAndroid;
#elif UNITY_IOS
            remoteConfigURLLinkDataSO = remoteConfigURLLinkDataSOIOS;
#endif
            if (remoteConfigURLLinkDataSO == null)
            {
                Debug.Log("remoteConfigURLLinkDataSO is not given");
            }
        }

        private void OnMoreGamesButton()
        {
            NextribeAnalyticsManager.DesignEventAnalytics("OnRateUsButton");
           // HapticTouchManager.PlayHaptics(HapticTypes.MediumImpact);
         //   AudioManager.Instance.PlaySound("Button_Click");
            string tempURL = "";
            switch (uRLFrom)
            {
                case URLFrom.FromRemoteConfig:
#if NEXTRIBE_FB_REMOTE_CONFIG && NEXTRIBE_FIREBASE
                    tempURL = NextribeFirebaseManager.Instance.GetFirebaseModuleObject<NextribeFirebaseRemoteConfigHandler>().RemoteConfigData.moreGamesURL;
#endif
                    break;
                case URLFrom.FromLocalURL:
                    tempURL = LocalURLLink;
                    break;
                case URLFrom.FromLocalRandomURL:
                    tempURL = remoteConfigURLLinkDataSO.GetRandomURL();
                    break;
                case URLFrom.FromRemoteRandomURL:
#if NEXTRIBE_FB_REMOTE_CONFIG && NEXTRIBE_FIREBASE
                    FirebaseRemoteConfigMoreGamesData temp = NextribeFirebaseManager.Instance.GetFirebaseModuleObject<NextribeFirebaseRemoteConfigHandler>().RemoteConfigMoreGamesData;
                    if (temp.moreGamesURLs.Count >= 1)
                    {
                        string currrentURL = $"https://play.google.com/store/apps/details?id={Application.identifier}";
                        if (temp.moreGamesURLs.Contains(currrentURL))
                        {
                            temp.moreGamesURLs.Remove(currrentURL);
                        }
                        tempURL = temp.moreGamesURLs[Random.Range(0, temp.moreGamesURLs.Count)];
                    }
#endif
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(tempURL))
            {
               // VaultPopUpsManager.Instance.CreateFlyPopUpToast("Please come back later", 0);
            }
            else
            {
                Application.OpenURL(tempURL);
            }
        }
    }
    enum URLFrom
    {
        FromRemoteConfig,
        FromLocalURL,
        FromLocalRandomURL,
        FromRemoteRandomURL
    }
}