using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : ScreenGUI {
	[SerializeField] private Slider MasterVolumeSlider;
	[SerializeField] private Slider MusicVolumeSlider;
	[SerializeField] private Slider EffectVolumeSlider;

	// Start is called before the first frame update
	public override void Start() {
		base.Start();

		MasterVolumeSlider.value = SoundController.MasterVolume.Value;
	}

	public void ChangeMasterVolume() {
		SoundController.MasterVolume.Value = MasterVolumeSlider.value;
	}

	public void ChangeMusicVolume() {
		SoundController.MusicVolume.Value = MusicVolumeSlider.value;
	}

	public void ChangeEffectVolume() {
		SoundController.EffectVolume.Value = EffectVolumeSlider.value;
	}
}
