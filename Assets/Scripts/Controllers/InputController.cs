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

public class KeyStroke {
	public List<(KeyCode, float)> Inputs = new List<(KeyCode, float)>();

	internal int index = 0;
	internal float timeLast = Time.time;
	internal bool canProceed = true;

	public KeyStroke(List<(KeyCode, float)> entry) {
		this.Inputs = entry;
	}
}

public class KeyStrokeDict : Dictionary<KeyStroke, Variable<bool>> {
	new public Variable<bool> this[KeyStroke key] {
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
	/// InputController.Keyboard[KeyCode.R].Connect((bool val) => {
	/// 	yaadadadadda
	/// 	return true;
	/// })
	/// This binds a function to fire whenever the R button is pressed or released. The val defines if it's pressed (true) or released (false).
	/// </code>
	/// </example>
	public static KeyBoardDict Keyboard = new KeyBoardDict();

	/// <summary>
	/// A Dictionary that contains a table of inputs from which you can bind a function to fire whenever a specific input is detected.
	/// </summary>
	/// <example>
	/// <code>
	/// InputController.Keyboard[KeyCode.R].Connect((bool val) => {
	/// 	yaadadadadda
	/// 	return true;
	/// })
	/// This binds a function to fire whenever the R button is pressed or released. The val defines if it's pressed (true) or released (false).
	/// </code>
	/// </example>
	public static KeyStrokeDict KeyStroke = new KeyStrokeDict();
	
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

		Listener<float> updateKeyStroke = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Input, (dt) => {
			KeyStroke[] copy = new KeyStroke[KeyStroke.Keys.Count];
			KeyStroke.Keys.CopyTo(copy, 0);

			foreach (KeyStroke key in copy) {
				if (key.index == 0) {
					key.timeLast = Time.time;
				}
				if (key.index < key.Inputs.Count) {
					(KeyCode, float) current = key.Inputs[key.index];

					if (key.canProceed) {
						if (InputController.Keyboard[current.Item1].Value) {
							if (Time.time - key.timeLast < current.Item2) {
								if  (key.index < key.Inputs.Count-1) {
									if (key.Inputs[key.index].Item1 == key.Inputs[key.index+1].Item1) {
										key.canProceed = false;
										InputController.Keyboard[key.Inputs[key.index].Item1].Connect((bool val)=> {
											if (!val) {
												key.canProceed = true;
												return false;
											}
											return true;
										});
									}
								}
								key.index++;
							} else {
								key.timeLast = Time.time;
								key.index = 0;
							}
						}
					}
				} else {
					key.index = 0;
					KeyStroke[key].Value = true;

					//Getting last key in keystroke to connect a function that activates when last key pressed up to say let go of keystroke.
					Keyboard[key.Inputs[key.Inputs.Count-1].Item1].Connect((bool val) => {
						if (!val) {
							KeyStroke[key].Value = false;
							return false;
						}
						return true;
					});
				}
			}
			return true;
		});
		updateKeyStroke.Name = "updateKeyStroke";
		Maid.GiveTask(updateKeyStroke);
	}

	/// <summary>
	/// Disconnects every binded function and itself. Don't run this unless you're planning to quit the game.
	/// </summary>
	public override void Dispose() {
		Maid.Dispose();
	}
}