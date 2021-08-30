using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls sound settings.
/// </summary>
public sealed class SoundController : Singleton {
	public AudioSource BackgroundMusic;

	public static Variable<float> MasterVolume = new Variable<float>(1);
	public static Variable<float> MusicVolume = new Variable<float>(1);
	public static Variable<float> EffectVolume = new Variable<float>(1);

	public void Awake() {
		// if audio settings haven't been changed
		if (!PlayerPrefs.HasKey("MasterVolume")) {
			// set audio to 100%
			PlayerPrefs.SetFloat("MasterVolume", 1);
		}
		if (!PlayerPrefs.HasKey("MusicVolume")) {
			// set audio to 100%
			PlayerPrefs.SetFloat("MusicVolume", 1);
		}
		if (!PlayerPrefs.HasKey("EffectVolume")) {
			// set audio to 100%
			PlayerPrefs.SetFloat("EffectVolume", 1);
		}
	}

	public void Start() {
		Listener<float> masterVolumeUpdate = MasterVolume.Connect((float val) => {
			AudioListener.volume = val;
			PlayerPrefs.SetFloat("MasterVolume", val);
			return true;
		});
		masterVolumeUpdate.Name = "masterVolumeUpdate";
		Maid.GiveTask(masterVolumeUpdate);

		MasterVolume.Value = PlayerPrefs.GetFloat("MasterVolume");

		Listener<float> musicVolumeUpdate = MusicVolume.Connect((float val) => {
			BackgroundMusic.volume = val;
			PlayerPrefs.SetFloat("MusicVolume", val);
			return true;
		});
		musicVolumeUpdate.Name = "musicVolumeUpdate";
		Maid.GiveTask(musicVolumeUpdate);

		MusicVolume.Value = PlayerPrefs.GetFloat("MusicVolume");

		Listener<float> effectVolumeUpdate = EffectVolume.Connect((float val) => {
			PlayerPrefs.SetFloat("EffectVolume", val);
			return true;
		});
		effectVolumeUpdate.Name = "effectVolumeUpdate";
		Maid.GiveTask(effectVolumeUpdate);

		EffectVolume.Value = PlayerPrefs.GetFloat("EffectVolume");
	}

	public override void Dispose() {
		Maid.Dispose();
	}
}
