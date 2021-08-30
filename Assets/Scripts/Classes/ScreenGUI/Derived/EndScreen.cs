using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EndScreen : ScreenGUI {
	// VARIABLES ------------
	public Text Text;
	public Button Button;

	public void SetScore(int score) {
		Text.text = score.ToString();
	}

    public void ReturnToMenu() {
        Maid.KillAll();
		SceneManager.LoadScene("MenuUpdated");
	}
}
