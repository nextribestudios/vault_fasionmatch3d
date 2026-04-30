#if NEXTRIBE_FIREBASE
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Nextribe.Firebase
{
    public class NextribeFirebaseRemoteConfigHandler : AbstractNextribeFirebaseModule
	{
		#region REMOTE CONFIG DATA CONTAINERS
        private string moreGamesJson;
        private string coolDownJson;
        private string remoteConfigJson;
		#endregion

        public FirebaseRemoteConfigData RemoteConfigData { get; private set; }
        public FirebaseRemoteConfigMoreGamesData RemoteConfigMoreGamesData { get; private set; }
        public FirebaseRemoteConfigInterstitalCoolDownData RemoteConfigInterstitalCoolDownData { get; private set; }
        public static Action RemoteConfigDataFetchEvent;
        
        public override void InitModule()
        {
            Module = NextribeFirebaseModules.RemoteConfig;
            Debug.Log("FirebaseRemoteConfigManager -- init");
            FetchRemoteConfigDataAsync();
            SubscribeListner();
            SetupRemoteConfigJsons();
        }

        private void SetupRemoteConfigJsons()
        {
#if UNITY_ANDROID
            if (string.IsNullOrEmpty(NextribeFirebaseManager.Instance.remoteConfigJsonAndroid))
            {
                Debug.LogError("Nextribe: RemoteConfigJsonAndroid is null add in firebaseManager");
            }
            else
            {
                remoteConfigJson = NextribeFirebaseManager.Instance.remoteConfigJsonAndroid;
            }
            if (string.IsNullOrEmpty(NextribeFirebaseManager.Instance.remoteConfigJsonMoreGamesAndroid))
            {
                Debug.LogError("Nextribe: remoteConfigJsonMoreGamesAndroid is null add in firebaseManager");
            }
            else
            {
                moreGamesJson = NextribeFirebaseManager.Instance.remoteConfigJsonMoreGamesAndroid;
            }
			if (string.IsNullOrEmpty(NextribeFirebaseManager.Instance.remoteConfigInterstitalCoolDown))
			{
				Debug.LogError("Nextribe: remoteConfigInterstitalCoolDown is null add in firebaseManager");
			}
			else
			{
				coolDownJson = NextribeFirebaseManager.Instance.remoteConfigInterstitalCoolDown;
			}
#elif UNITY_IOS
            if (string.IsNullOrEmpty(NextribeFirebaseManager.Instance.remoteConfigJsonMoreGamesIOS))
            {
                Debug.LogError("Nextribe: RemoteConfigJsonIOS is null add in firebaseManager");
            }
            else
            {
                remoteConfigJson = NextribeFirebaseManager.Instance.remoteConfigJsonMoreGamesIOS;
            }
            if (string.IsNullOrEmpty(NextribeFirebaseManager.Instance.remoteConfigJsonMoreGamesIOS))
            {
                Debug.LogError("Nextribe: remoteConfigJsonMoreGamesIOS is null add in firebaseManager");
            }
            else
            {
                moreGamesJson = NextribeFirebaseManager.Instance.remoteConfigJsonMoreGamesIOS;
            }
#endif
		}

        public override void SubscribeListner()
        {
            FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener += ConfigUpdateListenerEventHandler;
        }

        public override void UnsubscribeListner()
        {
            FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener -= ConfigUpdateListenerEventHandler;
        }

        private void OnDestroy()
        {
            UnsubscribeListner();
        }

        private Task FetchRemoteConfigDataAsync()
        {
            if (NextribeFirebaseManager.Instance.Initialized)
            {
                Debug.Log("Fetching data...");
                Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
                return fetchTask.ContinueWithOnMainThread(FetchComplete);
            }
            else
                return null;
        }

        private void FetchComplete(Task fetchTask)
        {
            if (!fetchTask.IsCompleted)
            {
                Debug.LogError("Retrieval hasn't finished.");
                return;
            }

            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
                RemoteConfigData = new FirebaseRemoteConfigData();
                RemoteConfigMoreGamesData = new FirebaseRemoteConfigMoreGamesData();
                OnModuleInitEvent?.Invoke(Module, false);
                return;
            }

            // Fetch successful. Parameter values must be activated to use.
            remoteConfig.ActivateAsync().ContinueWithOnMainThread(task =>
            {
                Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
                StoreConfigData(remoteConfig);
            });
        }

        private void StoreConfigData(FirebaseRemoteConfig remoteConfig)
        {
            // Read raw data
            string jString = remoteConfig.GetValue(remoteConfigJson).StringValue;
            string jStringMoreGamesURLs = remoteConfig.GetValue(moreGamesJson).StringValue;
            string jStringInterstitalCoolDown = remoteConfig.GetValue(coolDownJson).StringValue;
			// Store data

			RemoteConfigData = JsonUtility.FromJson<FirebaseRemoteConfigData>(jString);
            RemoteConfigMoreGamesData = JsonUtility.FromJson<FirebaseRemoteConfigMoreGamesData>(jStringMoreGamesURLs);
			RemoteConfigInterstitalCoolDownData = JsonUtility.FromJson<FirebaseRemoteConfigInterstitalCoolDownData>(jStringInterstitalCoolDown);
			// Send data received and stored event.
			RemoteConfigDataFetchEvent?.Invoke();
            OnModuleInitEvent?.Invoke(Module, true);
        }

        private void ConfigUpdateListenerEventHandler(object sender, ConfigUpdateEventArgs args)
        {
            if (args.Error != RemoteConfigError.None)
            {
                Debug.Log(String.Format("Error occurred while listening: {0}", args.Error));
                return;
            }

            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            Debug.Log("Updated keys: " + string.Join(", ", args.UpdatedKeys));
            // Activate all fetched values and then display a welcome message.
            remoteConfig.ActivateAsync().ContinueWithOnMainThread(
              task => {
                  StoreConfigData(remoteConfig);
              });
        }
    }
}
#endif