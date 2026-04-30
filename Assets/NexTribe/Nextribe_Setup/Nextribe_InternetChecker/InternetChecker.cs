using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace NexTribe
{
    public class InternetChecker : Singleton<InternetChecker>
    {
        private NoInternetPopUP popupInstance;

		public NoInternetPopUP noInternetPopupPrefab;
        public Transform noInternetPopupHolder;
		[Header("Enable Checker")]
        [SerializeField] private bool needInternetChecker = true;

        [Header("Settings")]
        [SerializeField] private float checkInterval = 3f;
        [SerializeField] private float requestTimeout = 3f;

        // Lightweight endpoint better than loading google page
        [SerializeField] private string testUrl = "https://clients3.google.com/generate_204";

        public bool IsConnected { get; private set; } = true;

        private bool popupShown = false;
        private Coroutine checkerRoutine;

        private void Start()
        {
            if (!needInternetChecker)
            {
                Destroy(gameObject);
                return;
            }

            checkerRoutine = StartCoroutine(CheckInternetRoutine());
        }

        private IEnumerator CheckInternetRoutine()
        {
            while (true)
            {
                yield return CheckInternet();

                HandleConnectionState();

                yield return new WaitForSecondsRealtime(checkInterval);
            }
        }

        private IEnumerator CheckInternet()
        {
            // Fast first-level check
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                IsConnected = false;
                yield break;
            }

            using (UnityWebRequest request = UnityWebRequest.Get(testUrl))
            {
                request.timeout = Mathf.RoundToInt(requestTimeout);
                yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                IsConnected = request.result == UnityWebRequest.Result.Success;
#else
                IsConnected = !request.isNetworkError && !request.isHttpError;
#endif
            }
        }

        private void ShowNoInternetPopup()
        {
            noInternetPopupHolder.gameObject.SetActive(true);
            popupInstance = Instantiate(noInternetPopupPrefab, noInternetPopupHolder);
            RectTransform rt = popupInstance.GetComponent<RectTransform>();
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            popupInstance.Show();
		}

        private void HandleConnectionState()
        {
            if (!IsConnected)
            {
                if (!popupShown)
                {
                    popupShown = true;
                    Time.timeScale = 0f; // Pause game logic

                    ShowNoInternetPopup();

					Debug.Log("No Internet - Popup Shown");
                }
            }
            else
            {
                if (popupShown)
                {
                    popupShown = false;

                    Time.timeScale = 1f;

                    if (popupInstance != null)
                    {
						popupInstance.OnCloseButton();
                        noInternetPopupHolder.gameObject.SetActive(false);
                    }

                    Debug.Log("Internet Restored - Popup Closed");
                }
            }
        }

        public void CheckForInternetPopUp()
        {
            if (!IsConnected)
            {
                HandleConnectionState();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause && checkerRoutine == null && needInternetChecker)
            {
                checkerRoutine = StartCoroutine(CheckInternetRoutine());
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}