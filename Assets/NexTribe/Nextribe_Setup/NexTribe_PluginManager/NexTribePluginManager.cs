using Nextribe.Firebase;
using Nextribe.GPGS;
using System.Collections;
//using GameAnalyticsSDK;
using UnityEngine;

namespace NexTribe.Plugin
{
    public class NexTribePluginManager : Singleton<NexTribePluginManager>
    {
        public Loader loadingMenuUI;
        #region Unity
        private bool isFirebaseInitialize;
        private bool waitTillFirebaseInitFinish;
        protected override void Awake()
        {
            base.Awake();
        }

#if NEXTRIBE_FIREBASE
        private void OnEnable()
        {
            NextribeFirebaseManager.OnInitializeEvent += NextribeFirebaseManager_OnInitializeEvent;
        }
        private void OnDisable()
        {
            NextribeFirebaseManager.OnInitializeEvent -= NextribeFirebaseManager_OnInitializeEvent;
        }
#endif

        private void NextribeFirebaseManager_OnInitializeEvent(bool value)
        {
            isFirebaseInitialize = value;
            waitTillFirebaseInitFinish = false;
        }

        private void Start()
        {
            //await NextribeIAPManager.Instance.InitializeIAP();
            StartCoroutine(PluginInitializeOrder());
        }
        #endregion

        private IEnumerator PluginInitializeOrder()
        {
           // SaveDataHandler.Instance.InitializeSavingSystem();

           // AudioManager.Instance.Initialize();

#if UNITY_ANDROID
            NexTribeGPGSManager.Instance.Initialize();
#endif

#if NEXTRIBE_FIREBASE
            waitTillFirebaseInitFinish = true;


            NextribeFirebaseManager.Instance.Initialize();

            while (waitTillFirebaseInitFinish)
            {
                yield return null;
            }

            if (!isFirebaseInitialize)
            {
                Debug.Log("No net");
                //add popupmanager fo no internet
                // yield break;
            }

#endif
            //  VaultAddressableManager.Instance.Initialize();

            //  NextribeAdsManager.Instance.Initialize();
            //  GameAnalytics.Initialize();
           // Invoke("loadingMenuUI.LoadMainMenu", 1f);
            loadingMenuUI.LoadMainMenu();
            yield return null;
        }
    }
}
