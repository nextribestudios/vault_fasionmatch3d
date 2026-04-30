#if NEXTRIBE_FB_ANALYTICS
using Firebase.Analytics;
#endif
//using GameAnalyticsSDK;
using UnityEngine;

namespace Nextribe.Analytics
{
    public class NextribeAnalyticsManager : Singleton<NextribeAnalyticsManager>
    {

        #region Unity
        private void Start()
        {
            // GameAnalytics.Initialize();
        }
        #endregion

        #region Level Start
        public static void OnLevelStartAnalytics(string levelNumber)
        {
            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Level_" + levelNumber);
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent("Start_Level_" + levelNumber);
#endif
            Debug.Log("Analytics:" + "Level Start" + "|" + "Level_" + levelNumber);
        }
        public static void OnLevelStartAnalytics(string levelNumber, string parameterName, int value)
        {
            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Level_" + levelNumber, parameterName, value);
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent("Start_Level_" + levelNumber, parameterName, value);
#endif
            Debug.Log("Analytics:" + "Level Start" + "|" + "Level_" + levelNumber + "|" + "Value:" + value);
        }
        #endregion

        #region LevelComplete
        public static void OnLevelCompleteAnalytics(string levelNumber)
        {
            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Level_" + levelNumber);
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent("Complete_Level_" + levelNumber);

#endif
            Debug.Log("Analytics:" + "Level Complete" + "|" + "Level_" + levelNumber);
        }
        public static void OnLevelCompleteAnalytics(string levelNumber, string parameterName, int value)
        {
            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Level_" + levelNumber, parameterName, value);
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent("Complete_Level_" + levelNumber, parameterName, value);
#endif
            Debug.Log("Analytics:" + "Level Complete" + "|" + "Level_" + levelNumber + "|" + "Value:" + value);
        }

        #endregion

        #region LevelFail
        public static void OnLevelFailAnalytics(string levelNumber)
        {
            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Level_" + levelNumber);
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent("Fail_Level_" + levelNumber);
#endif
            Debug.Log("Analytics:" + "Level Fail" + "|" + "Level_" + levelNumber);
        }
        public static void OnLevelFailAnalytics(string levelNumber, string parameterName, int value)
        {
            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Level_" + levelNumber, parameterName, value);
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent("Start_Level_" + levelNumber, parameterName, value);
#endif
            Debug.Log("Analytics:" + "Level Fail" + "|" + "Level_" + levelNumber + "|" + "Value:" + value);
        }
        #endregion

        #region DesignEvent
        public static void DesignEventAnalytics(string eventName)
        {
            // GameAnalytics.NewDesignEvent(eventName);
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent(eventName);
#endif
            Debug.Log("Analytics_" + "Event name:" + eventName);
        }
        public static void DesignEventAnalytics(string eventName, string parameterName, float eventValue)
        {
            //GameAnalytics.NewDesignEvent(eventName, eventValue);
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent(eventName, parameterName, eventValue);
#endif
            Debug.Log("Analytics:" + "Event name:" + eventName + "|" + "Event Value:" + eventValue);
        }
        #endregion

        #region Session
        public static void OnStartSessionAnalytics()
        {
            // GameAnalytics.StartSession();
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent("Start Session");
#endif
            Debug.Log("Analytics:" + "Start Session");
        }

        public static void OnEndSessionAnalytics()
        {
            // GameAnalytics.EndSession();
#if NEXTRIBE_FB_ANALYTICS
            FirebaseAnalytics.LogEvent("End Session");
#endif
            Debug.Log("Analytics:" + "End Session");
        }
        #endregion
    }
}