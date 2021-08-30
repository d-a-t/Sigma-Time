using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : ScreenGUI {
	// if play button hit, play the game scene
	public void StartGame() {
		// load the next scene into scene manager
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	// if quit button clicked, quit game
	public override void Dispose() {
		base.Dispose();
		Maid.KillAll();
		Application.Quit();
	}
}
