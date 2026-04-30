using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nextribe.Firebase
{
    [Serializable]
    public class FirebaseRemoteConfigData
    {
        private const string default_catalogURL = "";
        private const string default_moreGamesURL = "https://play.google.com/store/apps/dev?id=5254004012582833225&hl=en_IN";

        public string catalogURL;

        public string moreGamesURL;

        public bool isRateUSOn;
        public bool IsRateUSOn => isRateUSOn;

        [Header("Ads")]
        public bool isInterstitialAdsOn;
        public bool IsInterstitialAdsOn => isInterstitialAdsOn;

        public bool isRewardedAdsOn;
        public bool IsRewardedAdsOn => isRewardedAdsOn;
        public FirebaseRemoteConfigData()
        {
            catalogURL = default_catalogURL;
            moreGamesURL = default_moreGamesURL;
            isRateUSOn = false;

            isInterstitialAdsOn = true;
            isRewardedAdsOn = true;
        }
    }
    [Serializable]
    public class FirebaseRemoteConfigMoreGamesData
    {
        public List<string> moreGamesURLs = new List<string>();
        public FirebaseRemoteConfigMoreGamesData()
        {
        }
    }

	[Serializable]
	public class GameKeyValue
    {
        public string packageName;
        public float coolDownTime;
	}

    [Serializable]
    public class FirebaseRemoteConfigInterstitalCoolDownData
    {
        public List<GameKeyValue> games;
    }
}