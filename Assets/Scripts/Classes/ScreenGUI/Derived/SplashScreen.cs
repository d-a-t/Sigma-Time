using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : ScreenGUI {
	public Image Publisher;
	public Image Studios;


	public override void Start() {
		base.Start();

		float counter = 0;
		int check = 0;
		Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
			if (dt < .05F) {
				switch (check) {
					case 0:
						if (counter < 3F) {
							Publisher.color = new Color(Publisher.color.r, Publisher.color.g, Publisher.color.b, (counter / 3));
						} else {
							Publisher.color = new Color(Publisher.color.r, Publisher.color.g, Publisher.color.b, 1);

							counter = 0;
							check++;
						}
						break;
					case 1:
						if (counter > 1F) {
							counter = 0;
							check++;
						}
						break;
					case 2:
						if (counter < 2F) {
							Publisher.color = new Color(Publisher.color.r, Publisher.color.g, Publisher.color.b, 1 - (counter / 2));
						} else {
							Publisher.color = new Color(Publisher.color.r, Publisher.color.g, Publisher.color.b, 0);

							counter = 0;
							check++;
						}
						break;
					case 3:
						if (counter < 3F) {
							Studios.color = new Color(Studios.color.r, Studios.color.g, Studios.color.b, (counter / 3));
						} else {
							Studios.color = new Color(Studios.color.r, Studios.color.g, Studios.color.b, 1);

							counter = 0;
							check++;
						}
						break;
					case 4:
						if (counter > 1F) {
							counter = 0;
							check++;
						}
						break;
					case 5:
						if (counter < 2F) {
							Studios.color = new Color(Studios.color.r, Studios.color.g, Studios.color.b, 1 - (counter / 2));
						} else {
							Studios.color = new Color(Studios.color.r, Studios.color.g, Studios.color.b, 0);

							counter = 0;
							check++;
						}
						break;
					case 6:
						SceneManager.LoadScene("MenuUpdated");
						return false;
				}

				counter += dt;
			}
			return true;
		});
	}
}
