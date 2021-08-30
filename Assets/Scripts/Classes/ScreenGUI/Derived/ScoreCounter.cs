using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : ScreenGUI {
	// VARIABLES ------------
	public Text Text;

	public void SetScore(int score) {
		Text.text = score.ToString();
	}
}
