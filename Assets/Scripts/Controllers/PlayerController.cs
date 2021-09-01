using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// This takes a Character and binds controlls onto them.
/// </summary>
public sealed class PlayerController : Singleton {
	public static Variable<int> Score = new Variable<int>();
	public static Character Char;

	[SerializeField] private Character _Char;

	public BulletBar BulletBar;
	public HealthBar HealthBar;
	public ScoreCounter ScoreCounter;
	public EndScreen EndScreen;

	private Listener<int> BindAmmoToGui;

	public void Awake() {
		Char = _Char? _Char : Char;
	}

	public void Start() {
		EndScreen?.gameObject.SetActive(false);

		Maid.GiveTask(Score.Connect((int val) => {
			ScoreCounter.SetScore(val);
			return true;
		}));

		if (Char) {
			BindChar(Char);

			if (EndScreen) {
				Char.Maid.GiveTask(new Action(delegate {
					EndScreen.gameObject.SetActive(true);
					EndScreen.SetScore(Score.Value);
				}));
			}
		}
		/*
				ReloadGun = InputController.Keyboard[KeyCode.R].Connect((bool val) => {
					if (val) {
						Char.CurrentWeapon.Reload();
					}
					return true;
				});
				ReloadGun.Name = "ReloadGun";
				Maid.GiveTask(ReloadGun);

				*/
	}

	public override void Dispose() {
		Maid.Dispose();
	}

	public void BindChar(Character newChar) {
		Char = newChar;

		if (HealthBar) {
			HealthBar.SetMaxHealth(Char.MaxHealth);
			Char.Maid.GiveTask(Char.Health.Connect((int val) => {
				if (val > 0) {
					HealthBar.setHealth(val);
					return true;
				}
				HealthBar.setHealth(0);
				return false;
			}));
		}
		//Note, every binded function is given its own name and also binded to Maid. This allows for easy debugging and automatic memory management.

		Listener<bool> slowDown = InputController.Keyboard[KeyCode.Space].Connect((bool val) => {
			if (val) {
				Global.Environment.TimeScale = .5F;
			} else {
				Global.Environment.TimeScale = 1F;
			}
			return true;
		});
		slowDown.Name = "slowDown";
		Char.Maid.GiveTask(slowDown);

		//Binding the D key to move the character right.
		Listener<bool> moveRight = InputController.Keyboard[KeyCode.D].Connect((bool val) => {
			if (val) {
				Char.DesiredCharDirection.Value = new Vector2(1 , Char.DesiredCharDirection.Value.y);;
			} else if (InputController.Keyboard[KeyCode.A].Value) {
				Char.DesiredCharDirection.Value = new Vector2(-1 , Char.DesiredCharDirection.Value.y);
			} else {
				Char.DesiredCharDirection.Value = new Vector2(0 , Char.DesiredCharDirection.Value.y);;
			}
			return true;
		});
		moveRight.Name = "moveRight";
		Char.Maid.GiveTask(moveRight);

		//Binding the A key to move the character left.
		Listener<bool> moveLeft = InputController.Keyboard[KeyCode.A].Connect((bool val) => {
			if (val) {
				Char.DesiredCharDirection.Value = new Vector2(-1 , Char.DesiredCharDirection.Value.y);
			} else if (InputController.Keyboard[KeyCode.D].Value) {
				Char.DesiredCharDirection.Value = new Vector2(1 , Char.DesiredCharDirection.Value.y);
			} else {
				Char.DesiredCharDirection.Value = new Vector2(0 , Char.DesiredCharDirection.Value.y);
			}
			return true;
		});
		moveLeft.Name = "moveLeft";
		Char.Maid.GiveTask(moveLeft);

		//Binding the left shift key to toggle running.
		Listener<bool> toggleRun = InputController.Keyboard[KeyCode.LeftShift].Connect((bool val) => {
			//ternary operator yo
			Char.DesiredSpeed.Value = (val) ? Config.Player.Movement.RunSpeed : Config.Player.Movement.WalkSpeed;
			return true;
		});
		toggleRun.Name = "toggleRun";
		Char.Maid.GiveTask(toggleRun);

		//Dash keystroke
		Listener<bool> dashForward = InputController.KeyStroke[Config.Player.Controls.Movement.DashForward].Connect((bool val) => {
			if (val) {
				Char.DesiredCharDirection.Value = new Vector2(1, 0);
				Char.DesiredCharDirection.Locked = true;
			} else {
				Char.DesiredCharDirection.Locked = false;
				InputController.Keyboard[KeyCode.A].Call();
				InputController.Keyboard[KeyCode.D].Call();
			}
			return true;
		});
		toggleRun.Name = "toggleRun";
		Char.Maid.GiveTask(toggleRun);

		//Updating Camera
		Listener<float> updateCam = Runservice.BindToUpdate(Global.RunservicePriority.RenderStep.First, (float dt) => {
			if (Char) {
				CameraController.Cam.Value.transform.position = new Vector3(Char.transform.position.x, Char.transform.position.y, CameraController.Cam.Value.transform.position.z);
				return true;
			}
			return false;
		});
		updateCam.Name = "updateCam";
		Char.Maid.GiveTask(updateCam);

		//Binding Attack
		Listener<bool> slashSword = InputController.Keyboard[KeyCode.Mouse0].Connect((bool val) => {
			if (val) {
				Char.Animator.SetTrigger("Attack");
			}
			return true;
		});
		slashSword.Name = "slashSword";
		Char.Maid.GiveTask(slashSword);


		Maid.GiveTask(Char);
	}

}
