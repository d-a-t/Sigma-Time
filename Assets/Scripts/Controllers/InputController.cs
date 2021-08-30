using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class KeyBoardDict : Dictionary<KeyCode, Variable<bool>> {
	new public Variable<bool> this[KeyCode key] {
		get {
			if (!base.ContainsKey(key)) {
				base[key] = new Variable<bool>(false);
			}
			return base[key];
		}
	}
}

public class Mouse {
	public sealed class ScrollWheel {
		public Variable<float> Delta = new Variable<float>();
	}

	public ScrollWheel Wheel = new ScrollWheel();
	/// <summary>
	/// Defines the realtime mouse position in World.
	/// </summary>
	public Vector2 Position = new Vector2();
}

/// <summary>
/// A "static" class that allows functions to be binded whenever an input is fired. 
/// This prevents having to check for a input for every frame, as you can just bind a function to this class to fire whenever a specific input is detected. 
/// In short, you don't need to use Update() every time to check if a input is pressed.
/// </summary>
public sealed class InputController : Singleton {
	public KeyCode lastKey;

	//private static Dictionary<string, Variable<bool>> _Keyboard = new Dictionary<string, Variable<bool>>();

	/// <summary>
	/// A Dictionary that contains a table of inputs from which you can bind a function to fire whenever a specific input is detected.
	/// </summary>
	/// <example>
	/// <code>
	/// InputController[KeyCode.R].Connect((bool val) => {
	/// 	yaadadadadda
	/// 	return true;
	/// })
	/// This binds a function to fire whenever the R button is pressed or released. The val defines if it's pressed (true) or released (false).
	/// </code>
	/// </example>
	public static KeyBoardDict Keyboard = new KeyBoardDict();

	public static Mouse Mouse = new Mouse();

	public void Start() {
		Listener<float> updateMouse = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Input, (dt) => {
			Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			Mouse.Position = Camera.main.ScreenToWorldPoint(screenPosition);
			//Mouse.Wheel.Delta.Value = Input.GetAxis("Mouse ScrollWheel");
			return true;
		});
		updateMouse.Name = "updateMouse";
		Maid.GiveTask(updateMouse);

		Listener<float> updateKeyCodes = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Input, (dt) => {
			KeyCode[] copy = new KeyCode[Keyboard.Keys.Count];
			Keyboard.Keys.CopyTo(copy, 0);

			foreach (KeyCode key in copy) {
				if (Input.GetKeyDown(key) && !Keyboard[key].Value) {
					Keyboard[key].Value = true;
					lastKey = key;
				} else if (Input.GetKeyUp(key) && Keyboard[key].Value) {
					Keyboard[key].Value = false;
				}
			}
			return true;
		});
		updateKeyCodes.Name = "updateKeyCodes";
		Maid.GiveTask(updateKeyCodes);
	}

	/// <summary>
	/// Disconnects every binded function and itself. Don't run this unless you're planning to quit the game.
	/// </summary>
	public override void Dispose() {
		Maid.Dispose();
	}
}