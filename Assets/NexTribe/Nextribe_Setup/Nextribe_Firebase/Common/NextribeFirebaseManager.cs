#if NEXTRIBE_FIREBASE
using Firebase;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

#if NEXTRIBE_FB_FIRESTORE
using Firebase.Firestore;
#endif

namespace Nextribe.Firebase
{
	public delegate void SimpleDelegate();
	public delegate void SimpleDelegate<T>(T value);

	public class NextribeFirebaseManager : Singleton<NextribeFirebaseManager>
    {
        public string remoteConfigJsonMoreGamesAndroid;
        public string remoteConfigJsonMoreGamesIOS;
        public string remoteConfigJsonAndroid;
        public string remoteConfigJsonIOS;
        public string remoteConfigInterstitalCoolDown;

		#region MODULES
		[SerializeField]List<NextribeFirebaseModules> modules;

        List<AbstractNextribeFirebaseModule> firebaseModules;
        Dictionary<NextribeFirebaseModules, bool> moduleInitStatusDictionary;
        bool moduleInitialized;
        #endregion

        public bool Initialized {  get; private set; }
        public FirebaseApp FirebaseApp { get; private set; }
#if NEXTRIBE_FB_FIRESTORE
        public FirebaseFirestore Firestore { get; private set; }
#endif
        public string Token { get; set; }
        public static event SimpleDelegate<bool> OnInitializeEvent;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        public void Initialize()
        {
            Initialized = moduleInitialized = false;
            firebaseModules = new List<AbstractNextribeFirebaseModule>();
            moduleInitStatusDictionary = new Dictionary<NextribeFirebaseModules, bool>();
            PrepareModules();
            InitFirebase();
        }

        private void PrepareModules()
        {
            foreach (NextribeFirebaseModules module in modules)
            {
                switch (module)
                {
                    case NextribeFirebaseModules.Crashlytics:
#if NEXTRIBE_FB_CRASHLYTICS 
                        firebaseModules.Add(new NextribeFirebaseCrashlytics());
						NextribeFirebaseCrashlytics.OnModuleInitEvent += OnCrashlyticInitCallback;
#endif
                        break;
                    case NextribeFirebaseModules.RemoteConfig:
#if NEXTRIBE_FB_REMOTE_CONFIG
                        firebaseModules.Add(new NextribeFirebaseRemoteConfigHandler());
						NextribeFirebaseRemoteConfigHandler.OnModuleInitEvent += OnRemoteConfigInitCallback;
#endif
                        break;
                    case NextribeFirebaseModules.Messaging:
#if NEXTRIBE_FB_MESSAGING
                        firebaseModules.Add(new NextribeFirebaseMessaging());
						NextribeFirebaseMessaging.OnModuleInitEvent += OnCouldMessagingInitCallback;
#endif
                        break;
                    case NextribeFirebaseModules.FireStore:
#if NEXTRIBE_FB_FIRESTORE
                        firebaseModules.Add(new NextribeFireStoreDataHandler());
                        NextribeFireStoreDataHandler.OnModuleInitEvent += OnFireStoreCallback;
#endif
						break;
                    case NextribeFirebaseModules.Analytics:
#if NEXTRIBE_FB_ANALYTICS
                        firebaseModules.Add(new NextribeFirebaseAnalyticHandler());
                        NextribeFirebaseAnalyticHandler.OnModuleInitEvent += OnAnalyticsInitCallback;
#endif
						break;
                    case NextribeFirebaseModules.None:
                    default:
                        break;
                }
            }
        }
        private void InitFirebase()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
					// Create and hold a reference to your FirebaseApp,
					// where app is a Firebase.FirebaseApp property of your application class.
					FirebaseApp = FirebaseApp.DefaultInstance;
#if NEXTRIBE_FB_FIRESTORE
                    Firestore = FirebaseFirestore.DefaultInstance;
#endif

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                 
                    Debug.Log("VaultFirebaseManager -- INITIALIZED");
                    Initialized = true;

                    if (firebaseModules.Count > 0)
                    {
                        foreach (AbstractNextribeFirebaseModule module in firebaseModules)
                        {
                            module.InitModule();
                            Debug.Log("INITIALING Module:"+module.Module);
                        }
                    }
                }
                else
                {
                    // Firebase Unity SDK is not safe to use here.
                    Debug.LogError(String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    OnInitializeEvent?.Invoke(false);
                }
            });
        }

        private void CheckModulesInitStatus(NextribeFirebaseModules moduleType, bool result)
        {
            if (moduleInitialized) return;
            if(firebaseModules.Count>0)
            {
                Debug.Log("CheckModulesInitStatus:"+moduleType+","+result);
                if (!moduleInitStatusDictionary.ContainsKey(moduleType))
                    moduleInitStatusDictionary.Add(moduleType, result);

                if (moduleInitStatusDictionary.Count == firebaseModules.Count)
                {
                    // All modules are initialized.
                    // TODO : Handle each module failed init scenario.
                    OnInitializeEvent?.Invoke(true);
                    moduleInitialized = true;
                    UnsubscribeModuleInitEvent();
                }
                else
                    Debug.Log(firebaseModules.Count - moduleInitStatusDictionary.Count+" more modules to be initialize...");
            }
            else
                OnInitializeEvent?.Invoke(false);
        }

        public T GetFirebaseModuleObject<T>() where T : AbstractNextribeFirebaseModule
        {
            foreach(AbstractNextribeFirebaseModule module in firebaseModules)
            {
                if (module is T)
                    return module as T;
            }
            return null;
        }

        private void OnDestroy()
        {
            if (firebaseModules.Count > 0)
            {
                foreach (AbstractNextribeFirebaseModule module in firebaseModules)
                    module.UnsubscribeListner();
            }
            if(!moduleInitialized)
                UnsubscribeModuleInitEvent();
        }

        #region MODULE INITIALIZATION CALLBACK
        private void OnCrashlyticInitCallback(NextribeFirebaseModules module, bool status) 
        {
            if (module != NextribeFirebaseModules.Crashlytics) return;
            Debug.Log("Module Init, Crashlytic:" + status);
            CheckModulesInitStatus(NextribeFirebaseModules.Crashlytics, status);
        }
        private void OnAnalyticsInitCallback(NextribeFirebaseModules module, bool status) 
        {
            if (module != NextribeFirebaseModules.Analytics) return;
            Debug.Log("Module Init, Firebase analytics:" + status);
            CheckModulesInitStatus(NextribeFirebaseModules.Analytics, status);
        }
        private void OnCouldMessagingInitCallback(NextribeFirebaseModules module, bool status) 
        {
            if (module != NextribeFirebaseModules.Messaging) return;
            Debug.Log("Module Init, Cloud messaging:" + status);
            CheckModulesInitStatus(NextribeFirebaseModules.Messaging, status);
        }
        private void OnRemoteConfigInitCallback(NextribeFirebaseModules module, bool status) 
        {
            if (module != NextribeFirebaseModules.RemoteConfig) return;
            Debug.Log("Module Init, Remote config:" + status);
            CheckModulesInitStatus(NextribeFirebaseModules.RemoteConfig, status);
        }
        private void OnFireStoreCallback(NextribeFirebaseModules module, bool status) 
        {
            if (module != NextribeFirebaseModules.FireStore) return;
            Debug.Log("Module Init, Fire store:" + status);
            CheckModulesInitStatus(NextribeFirebaseModules.FireStore, status);
        }
        private void UnsubscribeModuleInitEvent()
        {
#if NEXTRIBE_FB_CRASHLYTICS
			NextribeFirebaseCrashlytics.OnModuleInitEvent -= OnCrashlyticInitCallback;
#endif
#if NEXTRIBE_FB_ANALYTICS
            NextribeFirebaseAnalyticHandler.OnModuleInitEvent -= OnAnalyticsInitCallback;
#endif
#if NEXTRIBE_FB_MESSAGING
			NextribeFirebaseMessaging.OnModuleInitEvent -= OnCouldMessagingInitCallback;
#endif
#if NEXTRIBE_FB_REMOTE_CONFIG
			NextribeFirebaseRemoteConfigHandler.OnModuleInitEvent -= OnRemoteConfigInitCallback;
#endif
#if NEXTRIBE_FB_FIRESTORE
            NextribeFireStoreDataHandler.OnModuleInitEvent -= OnFireStoreCallback;
#endif
		}
		#endregion

#if NEXTRIBE_FB_REMOTE_CONFIG
		public bool CanShowRateUs()
        {
            return GetFirebaseModuleObject<NextribeFirebaseRemoteConfigHandler>().RemoteConfigData.IsRateUSOn;
        }
#endif
    }
}
#endif