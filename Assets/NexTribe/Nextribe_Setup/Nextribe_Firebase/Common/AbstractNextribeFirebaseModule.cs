
namespace Nextribe.Firebase
{
    #region MODULE CONDITIONAL COMPILATION SYMBOL
    /*
     * Crashlytics : NEXTRIBE_FB_CRASHLYTICS
     * Remote config : NEXTRIBE_FB_REMOTE_CONFIG
     * Messaging : NEXTRIBE_FB_MESSAGING
     * Fire store : NEXTRIBE_FB_FIRESTORE
     * Analytics : NEXTRIBE_FB_ANALYTICS
     */
    #endregion

    public enum NextribeFirebaseModules
    {
        None = 0,
        Crashlytics,
        RemoteConfig,
        Messaging,
        FireStore,
        Analytics
    }

    public abstract class AbstractNextribeFirebaseModule
    {
        public static NextribeFirebaseModuleInitEvent OnModuleInitEvent;
        public NextribeFirebaseModules Module { get; protected set; }
        public abstract void InitModule();
        public abstract void SubscribeListner();
        public abstract void UnsubscribeListner();
    }

    public delegate void NextribeFirebaseModuleInitEvent(NextribeFirebaseModules module, bool success); 
}
