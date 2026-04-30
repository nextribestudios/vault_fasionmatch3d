#if NEXTRIBE_FIREBASE
using Firebase.Crashlytics;

namespace Nextribe.Firebase
{
    public class NextribeFirebaseCrashlytics : AbstractNextribeFirebaseModule
    {
        public override void InitModule()
        {
            Module = NextribeFirebaseModules.Crashlytics;
            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
            OnModuleInitEvent?.Invoke(Module,true);
        }

        public override void SubscribeListner()
        {
            
        }

        public override void UnsubscribeListner()
        {
            
        }
    }
}
#endif