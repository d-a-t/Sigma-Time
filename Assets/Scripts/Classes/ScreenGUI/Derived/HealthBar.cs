using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : ScreenGUI {
	// VARIABLES ------------
	public Slider slider;
	public Gradient gradient;
	public Image fill;

	// sets max vaalue for the slider and makes sure health starts at full
	public void SetMaxHealth(int health) {
		slider.maxValue = health;
		slider.value = health;

		// make health bar green
		fill.color = gradient.Evaluate(1f);
	}

	public void setHealth(int health) {
		slider.value = health;

		// change health bar accodrding to health
		fill.color = gradient.Evaluate(slider.normalizedValue);
	}
}
