using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A class that defines an item that which the player can equip and use with unique functions, as compared to a regular item. Ex: Minecraft pickaxe vs apple.
/// </summary>
public class Tool : Item {
	[Header("Character Info")]
	/// <summary>
	/// Defines who's controlling the tool.
	/// </summary>
	public Variable<Character> Controller = new Variable<Character>();

	protected Animator Animator;

	/// <summary>
	/// Defines if the tool is equipped.
	/// </summary>
	public Variable<bool> Equipped = new Variable<bool>(false);

	/// <summary>
	/// Defines if the tool can be equipped.
	/// </summary>
	public bool CanEquip = true;

	/// <summary>
	/// Defines if the tool can be unequipped.
	/// </summary>
	public bool CanUnequip = true;

	[Header("Weld Info")]
	/// <summary>
	/// This Weld updates to attach itself to the Controller whenever Controller changes.
	/// </summary>
	public Weld Weld;

	//Yea, you could just directly edit the Equipped parameter.
	/// <summary>
	/// Equips the tool and makes it visible.
	/// </summary>
	public virtual bool Equip() {
		if (!CanEquip) {
			return false;
		}
		Controller.Value = Holder.Value;
		Equipped.Value = true;
		this.Visible.Value = true;

		BindControls();

		return true;
	}

	/// <summary>
	/// Unequips the tool and makes it invisible.
	/// </summary>
	public virtual bool Unequip() {
		if (!CanUnequip) {
			return false;
		}
		Controller.Value = null;
		Equipped.Value = false;
		this.Visible.Value = false;

		UnbindControls();

		return true;
	}

	public Dictionary<String, Delegate> EnemyControls = new Dictionary<String, Delegate>();

	protected virtual void BindControls() {}

	protected virtual void UnbindControls() {}

	public override void Awake() {
		base.Awake();

		//Toggles if the tool is visible depending on if it's equipped or not.
		Listener<bool> setTransparency = this.Equipped.Connect((bool val) => {
			this.Visible.Value = val;
			return true;
		});
		setTransparency.Name = "setTransparency";
		Maid.GiveTask(setTransparency);

		//Initializes weld and connects the tool to its controller.
		this.Weld = new Weld();
		this.Weld.Part1 = this.gameObject.transform;

		//Changes which Character this tool welds to if its Controller changes.
		Listener<Character> controllerChange = Controller.Connect((Character val) => {
			if (val) {
				this.Weld.Part0 = val.gameObject.transform;
			} else {
				this.Weld.Part0 = null;
			}
			return true;
		});
		controllerChange.Name = "controllerChange";
		Maid.GiveTask(controllerChange);
	}

	public override void Start() {
		base.Start();

		this.Visible.Value = this.Equipped.Value;
	}
}
