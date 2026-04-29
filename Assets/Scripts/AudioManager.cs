using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	#region Singleton

	private static AudioManager _instance;

	public static AudioManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<AudioManager>();
			}
			return _instance;
		}
	}

    #endregion

	[SerializeField] AudioSource BgSource;
	[SerializeField] AudioClip menuBGM;
	[SerializeField] AudioClip gamePlayBGM;

	[SerializeField] AudioClip magnetSFX;
	[SerializeField] AudioClip undoSFX;
	[SerializeField] AudioClip shuffleSFX;
	[SerializeField] AudioClip freezeSFX;
	[SerializeField] AudioClip energySFX;
	[SerializeField] AudioClip addLifeSFX;
	[SerializeField] AudioClip clickSFX;
	[SerializeField] AudioClip coinCollectionSFX;
	[SerializeField] AudioClip coinPurchaseSFX;
	[SerializeField] AudioClip constructionSFX;
	[SerializeField] AudioClip cutSceneSFX;
	[SerializeField] AudioClip failSFX;
	[SerializeField] AudioClip winSFX;
	[SerializeField] AudioClip starSFX;
	[SerializeField] AudioClip wardrobSFX;
	[SerializeField] AudioClip textScribSFX;
	[SerializeField] AudioClip matchSFX;
	[SerializeField] AudioClip itemSFX;

	[SerializeField] float musicVol = 1;
	[SerializeField] float effectVol = 1;

	private void Awake()
    {
        _instance = this;
    }

	public void PlayBackGroundMusic(int _index)
	{
		if (!Loader.Instance.IsMusic)
			return;
		BgSource.Stop();
		//BgSource.clip = _bgm;
		if (_index == 1)
		{
			BgSource.clip = gamePlayBGM;
			BgSource.volume = 0.3f;
		}
		else
		{
			BgSource.clip = menuBGM;
			BgSource.volume = musicVol;
		}
		BgSource.loop = true;
		//BgSource.Play(2000);
		BgSource.PlayDelayed(2);
	}

	public void StopBGM()
    {
		BgSource.volume = 0;
		BgSource.Stop();
	}

	public void PlayStoyEffect(AudioSource _textSource)
    {
		_textSource.Stop();
		_textSource.clip = textScribSFX;
		_textSource.volume = effectVol;

		_textSource.loop = true;
		_textSource.Play();
	}

	public void StopStoryEffect(AudioSource _textSource)
    {
		_textSource.Stop();
	}

	public void PlaySoundEffects(AudioClip _effect)
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(_effect, effectVol);
	}

	public void PlayMagnetEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(magnetSFX, effectVol);
	}

	public void PlayUndoEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(undoSFX, effectVol);
	}
	public void PlayShuffleEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(shuffleSFX, effectVol);
	}
	public void PlayFreezeEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(freezeSFX, effectVol);
	}
	public void PlayEnergyEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(energySFX, effectVol);
	}
	public void PlayFailEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(failSFX, effectVol);
	}
	public void PlayWinEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(winSFX, effectVol);
	}
	public void PlayClickEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(clickSFX, effectVol);
	}
	public void PlayConstructionEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(constructionSFX, effectVol);
	}
	public void PlayCoinCollectionEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(coinCollectionSFX, effectVol);
	}

	public void PlayCoinPurchaseEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(coinPurchaseSFX, effectVol);
	}
	public void PlayCutSceneEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(cutSceneSFX, effectVol);
	}
	public void PlayStarEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(starSFX, effectVol);
	}
	public void PlayWardrobeEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(wardrobSFX, effectVol);
	}
	public void PlayMatchEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(matchSFX, effectVol);
	}
	public void PlayItemEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(itemSFX, effectVol);
	}
	public void PlayTextEffects()
	{
		if (Loader.Instance.IsSound)
			AudioSource3D.PlayClip2D(textScribSFX, effectVol);
	}

}
