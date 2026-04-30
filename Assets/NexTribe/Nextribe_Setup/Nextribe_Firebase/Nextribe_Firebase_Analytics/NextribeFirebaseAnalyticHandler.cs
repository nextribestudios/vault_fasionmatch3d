#if NEXTRIBE_FIREBASE
using Firebase.Analytics;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Nextribe.Firebase
{
    public class NextribeFirebaseAnalyticHandler : AbstractNextribeFirebaseModule
    {
        public override void InitModule()
        {
            Module = NextribeFirebaseModules.Analytics;
            Debug.Log("VaultFirebaseAnalyticHandler Initialized");
            OnModuleInitEvent?.Invoke(Module, true);
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        }

        public override void SubscribeListner()
        {

        }

        public override void UnsubscribeListner()
        {

        }

        private void SendEvent_Firebase(string eventLog, Dictionary<string, object> paraDictionary)
        {
            Parameter[] parameters = ConvertDictionaryToParameters(paraDictionary);
            FirebaseAnalytics.LogEvent(eventLog, parameters);
        }

        public void SendEvent_Firebase(string eventLog, string parameterName, int value)
        {
            FirebaseAnalytics.LogEvent(eventLog, parameterName, value);
        }

        public void SendEvent_Firebase(string eventLog, string parameterName, float value)
        {
            FirebaseAnalytics.LogEvent(eventLog, parameterName, value);
        }

        public void SendEvent_Firebase(string eventLog, string parameterName, string value)
        {
            FirebaseAnalytics.LogEvent(eventLog, parameterName, value);
        }

        public void SendEvent_Firebase(string eventLog)
        {
            FirebaseAnalytics.LogEvent(eventLog);
        }

        private Parameter[] ConvertDictionaryToParameters(Dictionary<string, object> dictionary)
        {
            List<Parameter> parameterList = new List<Parameter>();
            foreach (var parameter in dictionary)
            {
                // Depending on the type of value, create a corresponding Parameter
                if (parameter.Value is int)
                    parameterList.Add(new Parameter(parameter.Key, (int)parameter.Value));
                else if (parameter.Value is float)
                    parameterList.Add(new Parameter(parameter.Key, (float)parameter.Value));
                else if (parameter.Value is double)
                    parameterList.Add(new Parameter(parameter.Key, Convert.ToSingle(parameter.Value)));
                else if (parameter.Value is string)
                    parameterList.Add(new Parameter(parameter.Key, (string)parameter.Value));
            }
            return parameterList.ToArray();
        }
    }
}
#endif