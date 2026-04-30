#if NEXTRIBE_FIREBASE
using Firebase.Messaging;
using Nextribe.Firebase;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Nextribe.Firebase
{
    public class NextribeFirebaseMessaging : AbstractNextribeFirebaseModule
	{
        private const string POST_NOTIFICATIONS_PERMISSION = "android.permission.POST_NOTIFICATIONS";
        public override void InitModule()
        {
            RequestPermission();
            Module = NextribeFirebaseModules.Messaging;
            SubscribeListner();
            GetTokenAsync();
        }
        private void RequestPermission()
        {
#if UNITY_EDITOR
#elif UNITY_ANDROID && !UNITY_EDITOR
            if (AndroidPermissionRequired())
            {
                if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(POST_NOTIFICATIONS_PERMISSION))
                {
                    UnityEngine.Android.Permission.RequestUserPermission(POST_NOTIFICATIONS_PERMISSION);
                }
                else
                {
                    Debug.Log("Android Notification Permission Already Granted.");
                }
            }
            else
            {
                Debug.Log("Lower then Android 13");
            }
#elif UNITY_IOS
          /*  FirebaseMessaging.RequestPermissionAsync().ContinueWith(permissionTask =>
                   {
                       if (permissionTask.IsCompleted)
                       {
                           Debug.Log("Notification permission request completed.");
                           if (permissionTask.Result == Firebase.Messaging.AuthorizationStatus.Authorized)
                           {
                               Debug.Log("Notification permissions granted.");
                           }
                           else if (permissionTask.Result == Firebase.Messaging.AuthorizationStatus.Denied)
                           {
                               Debug.Log("Notification permissions denied.");
                           }
                           else if (permissionTask.Result == Firebase.Messaging.AuthorizationStatus.Provisional)
                           {
                               Debug.Log("Provisional notification permissions granted.");
                           }
                       }
                       else
                       {
                           Debug.LogError("Notification permission request failed.");
                       }
                   });*/
#endif
        }

        private bool AndroidPermissionRequired()
        {
            int apiLevel = GetAndroidAPILevel();
            // Android 13+ (API 33+) needs permission
            return (apiLevel >= 33);
        }

        private int GetAndroidAPILevel()
        {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
        }


        public override void SubscribeListner()
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        public override void UnsubscribeListner()
        {
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        private void OnDestroy()
        {
            UnsubscribeListner();
        }

        private async Task GetTokenAsync()
        {
            try
            {
                // Get the token asynchronously
                var token = await FirebaseMessaging.GetTokenAsync();

                // Log the result
                Debug.Log("GetTokenAsync:" + token);

                // Store the token
                NextribeFirebaseManager.Instance.Token = token;
                OnModuleInitEvent?.Invoke(Module,true);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error getting token: " + ex.Message);
                OnModuleInitEvent?.Invoke(Module,false);
            }
        }


		#region LISTNER

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.Log("From: " + e.Message.From + ",Message ID: " + e.Message.MessageId);
        }

        private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            //The app will receive the token with the OnTokenReceived event, which should be cached for later use.
            // You'll need this token if you want to target this specific device for messages.
            NextribeFirebaseManager.Instance.Token = token.Token;
            Debug.Log("Received Registration Token: " + token.Token);
        }
		#endregion
    }
}
#endif