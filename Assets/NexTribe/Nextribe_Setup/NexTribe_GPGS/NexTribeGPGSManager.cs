using Google.Play.AppUpdate;
using Google.Play.Common;
using Google.Play.Review;
using GooglePlayGames;
using System.Collections;
using UnityEngine;
using GooglePlayGames.BasicApi;
using System;

namespace Nextribe.GPGS
{
    public class NexTribeGPGSManager : Singleton<NexTribeGPGSManager>
    {

        // Play Service
        public bool IsInitialised { get; private set; }
        public string UserPlayID { get; private set; }

        // In-App update
        private AppUpdateManager appUpdateManager;
        private float checkInterval = 2.0f; // Check interval
        private float timeSinceLastCheck = 0f;
        private bool updatingApp;
        [System.Flags]
        public enum InitOptions
        {
            None = 0,
            ActivatePlayGames = 1 << 0,
            CheckForUpdate = 1 << 1,
            ManualSignIn = 1 << 2
        }

        [SerializeField] private InitOptions initOptions;

        public void Initialize()
        {
#if UNITY_ANDROID

            if (initOptions.HasFlag(InitOptions.ActivatePlayGames))
            {
                PlayGamesPlatform.Activate();
            }

            if (initOptions.HasFlag(InitOptions.CheckForUpdate))
            {
                CheckForAppUpdate();
            }

            if (initOptions.HasFlag(InitOptions.ManualSignIn))
            {
                ManuallySignIn();
            }

#endif
        }

#if UNITY_ANDROID
        #region GOOGLE SIGN IN
        // Initialize Google Play Games configuration
        private void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                UserPlayID = PlayGamesPlatform.Instance.localUser.id;

#if VAULT_FB_FIRESTORE

                VaultFirebaseManager.Instance.GetFirebaseModuleObject<VaultFireStoreDataHandler>()?.SetDBCollectionName(UserPlayID);
                VaultFirebaseManager.Instance.GetFirebaseModuleObject<VaultFireStoreDataHandler>()?.LoadFromFireStore<GameSaveDataCloud>();
#endif
                Debug.Log("Google Sign In Successful, ID:" + UserPlayID);
            }
            else
            {
                Debug.Log("Google Sign In Failed, Status:" + status.ToString());
            }
        }

        public void ManuallySignIn()
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate((status =>
            {
                ProcessAuthentication(status);
            }));
        }
        #endregion
#endif

#if UNITY_ANDROID
        #region PlayStore RateUS
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;

        public void OpenRateUsGoogle()
        {
            StartCoroutine(RequestReviews());
        }

        IEnumerator RequestReviews()
        {
            _reviewManager = new ReviewManager();

            //Request the review Info Object
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                PlayerPrefs.SetInt("RateUsPopUp", 0);
                PlayerPrefs.Save();
                Debug.Log("Request Error " + requestFlowOperation.Error.ToString());

                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();

            //Launch the InApp review flow

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                PlayerPrefs.SetInt("RateUsPopUp", 0);
                PlayerPrefs.Save();
                Debug.Log("Request Error " + launchFlowOperation.Error.ToString());

                yield break;
            }
            Debug.Log("Review flow end");
            //SaveDataHandler.Instance.GameProgressionSaveData.AppRated = true;
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
        }
        #endregion

        #region In-App Update
        private void CheckForAppUpdate()
        {
            updatingApp = false;
            DisplayDebug("CheckForAppUpdate");
            if (appUpdateManager == null)
            {
                InitialiseInAppUpdate();
                if (appUpdateManager == null)
                {
                    DisplayDebug("Unable to initialise");
                    return;
                }
            }

            StopAllCoroutines();
            StartCoroutine(nameof(CheckForUpdateCoroutine));
        }
        private void InitialiseInAppUpdate()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                appUpdateManager = new AppUpdateManager();
                DisplayDebug("Initialised in app update");
            }
            else
                DisplayDebug("Unsupported platform : " + Application.platform);
        }
        private IEnumerator CheckForUpdateCoroutine()
        {
            PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
              appUpdateManager.GetAppUpdateInfo();
            DisplayDebug("CheckForUpdateCoroutine");
            // Wait until the asynchronous operation completes.
            yield return appUpdateInfoOperation;

            if (appUpdateInfoOperation.IsSuccessful)
            {
                var appUpdateInfo = appUpdateInfoOperation.GetResult();
                // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
                #region Miscellaneous
                //check how much time has passed since the user was last notified of an update through the Play Store
                var stalenessDays = appUpdateInfo.ClientVersionStalenessDays;
                //To check the update priority
                int priority = appUpdateInfo.UpdatePriority;
                #endregion

                // Creates an AppUpdateOptions for a flexible flow that allows asset pack deletion.
                var appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions(allowAssetPackDeletion: false);

                // Check if there s an update available and it s allowed to use the flexible update
                bool isUpgradeAllowed = appUpdateInfo.IsUpdateTypeAllowed(appUpdateOptions);

                DisplayDebug("UpdateAvailability:" + appUpdateInfo.UpdateAvailability + " ,isUpgradeAllowed:" + isUpgradeAllowed);
                switch (appUpdateInfo.UpdateAvailability)
                {
                    case UpdateAvailability.UpdateNotAvailable:
                        DisplayDebug("[IN-APP UPDATE] : Update not available!");
                        break;

                    case UpdateAvailability.UpdateAvailable:
                        if (appUpdateOptions.AppUpdateType == AppUpdateType.Flexible)// && isUpgradeAllowed)
                            StartCoroutine(StartUpdate(appUpdateInfo, appUpdateOptions));
                        else if (appUpdateOptions.AppUpdateType == AppUpdateType.Immediate)
                            StartCoroutine(StartUpdate(appUpdateInfo, AppUpdateOptions.ImmediateAppUpdateOptions()));
                        break;

                    case UpdateAvailability.DeveloperTriggeredUpdateInProgress:
                        if (appUpdateInfo.AppUpdateStatus == AppUpdateStatus.Downloaded)
                            CompleteUpdate();
                        else
                            DisplayDebug("[IN-APP UPDATE] : Update already in progress.");
                        break;

                    case UpdateAvailability.Unknown:
                    default:
                        DisplayDebug("[IN-APP UPDATE] : Update condition failed!");
                        break;
                }
            }
            else
            {
                // Log appUpdateInfoOperation.Error.
                DisplayDebug("Failed to get update info: " + appUpdateInfoOperation.Error);
            }
        }

        private IEnumerator StartUpdate(AppUpdateInfo appUpdateInfo, AppUpdateOptions appUpdateOptions)
        {
            var appUpdateTask = appUpdateManager.StartUpdate(appUpdateInfo, appUpdateOptions);
            updatingApp = true;
            DisplayDebug("StartUpdate of type : " + appUpdateOptions.AppUpdateType);
            appUpdateTask.Completed += (appUpdateRequest) =>
            {
                if (appUpdateRequest.IsDone)
                {
                    if (appUpdateInfo.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
                    {
                        DisplayDebug("DeveloperTriggeredUpdateInProgress...");
                    }
                    else if (appUpdateInfo.AppUpdateStatus == AppUpdateStatus.Downloaded)
                    {
                        DisplayDebug("AppUpdateStatus.Downloaded -- StartFlexibleUpdate");
                        CompleteUpdate();
                    }
                    else
                        DisplayDebug("Something happened to update");
                }
                else
                {
                    DisplayDebug("StartFlexibleUpdate -- Failed to get update status: " + appUpdateRequest.Error);
                }
            };
            while (!appUpdateTask.IsDone)
            {
                // For flexible flow,the user can continue to use the app while
                // the update downloads in the background. You can implement a
                // progress bar showing the download status during this time.
                DisplayDebug("Update is in progress...");
                yield return null;
            }
        }

        private void Update()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(updatingApp)
            {
                timeSinceLastCheck += Time.deltaTime;
                if (timeSinceLastCheck >= checkInterval)
                {
                    timeSinceLastCheck = 0f;
                    CheckUpdateStatus();
                }
            }
#endif
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && appUpdateManager != null)
            {
                CheckUpdateStatus();
            }
        }

        private void CheckUpdateStatus()
        {
            var appUpdateInfoTask = appUpdateManager.GetAppUpdateInfo();
            DisplayDebug("[IN-APP UPDATE] : Checking update status");
            appUpdateInfoTask.Completed += (task) =>
            {
                if (task.IsSuccessful)
                {
                    var appUpdateInfo = task.GetResult();

                    var updateAvailability = appUpdateInfo.UpdateAvailability;
                    var updateStatus = appUpdateInfo.AppUpdateStatus;

                    DisplayDebug("CheckUpdateStatus -- UpdateAvailability:" + updateAvailability + " ,AppUpdateStatus:" + updateStatus);
                    if ((updateAvailability == UpdateAvailability.UpdateAvailable || updateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
                        && updateStatus == AppUpdateStatus.Downloaded)
                    {
                        DisplayDebug("AppUpdateStatus.Downloaded -- OnApplicationFocus");
                        CompleteUpdate();
                    }
                }
                else
                {
                    DisplayDebug("<color=red>CheckUpdateStatus -- Failed to get update info on focus:</color> " + task.Error);
                }
            };
        }

        private void CompleteUpdate()
        {
            updatingApp = false;
            var result = appUpdateManager.CompleteUpdate();
            DisplayDebug("[IN-APP UPDATE] : Update complete !");
            // If the update completes successfully, then the app restarts and this line
            // is never reached. If this line is reached, then handle the failure (e.g. by
            // logging result.Error or by displaying a message to the user)
        }

        private void DisplayDebug(string log)
        {
            Debug.Log(log);
            //debugText.text += "\n[" + DateTime.Now + "]\n" + log;
        }
        #endregion
#endif
    }
}
