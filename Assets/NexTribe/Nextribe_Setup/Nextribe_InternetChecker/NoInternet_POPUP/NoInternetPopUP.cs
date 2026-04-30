using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace NexTribe
{
    public class NoInternetPopUP : MonoBehaviour
	{
		public Button popUpCloseButton;

		public Action turnOffHolder;

		public Button retryButton;

		protected virtual void OnEnable()
		{
			if (popUpCloseButton != null)
			{
				popUpCloseButton.onClick.AddListener(OnCloseButton);
			}

            retryButton.onClick.AddListener(OnRetry);
		}
		protected virtual void OnDisable()
		{
			if (popUpCloseButton != null)
			{
				popUpCloseButton.onClick.RemoveAllListeners();
			}
		}

		public virtual void OnCloseButton()
		{
			//AudioManager.Instance.PlaySound("Button_Click");
			//HapticTouchManager.PlayHaptics(HapticTypes.MediumImpact);
			Hide();
		}
		//---------

		private Action onHiddenCallback;

		public virtual void Show()
		{
			transform.localScale = Vector3.zero;
			gameObject.SetActive(true);
			transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
			{
				PostShowAction();
			});
		}

		public virtual void Hide()
		{
			Time.timeScale = 1f;
			transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
			{
				gameObject.SetActive(false);
				onHiddenCallback?.Invoke();
			});
		}

		protected virtual void PostShowAction() { }

		public virtual void SetOnHiddenCallback(Action callback)
		{
			onHiddenCallback = callback;
		}

        private void OnRetry()
        {
            //AudioManager.Instance.PlaySound("Button_Click");
           // HapticTouchManager.PlayHaptics(HapticTypes.MediumImpact);
            // StartCoroutine(CheckConnection());
        }
    }
}