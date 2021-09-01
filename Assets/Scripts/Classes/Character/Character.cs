using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// A custom class used to handle characters. Can be used for player characters or NPCs.
/// </summary>
/// <remarks>
/// <para>
/// This class can be easily instantiated and destroyed, using the Maid class. Note that this 
/// </para>
/// </remarks>
public class Character : Part {
	public Animator Animator;

	[Header("Health")]
	public int MaxHealth = 100;
	public Variable<int> Health = new Variable<int>(100);
	public int RegenRate = 5;

	protected HumanDescription _HumDesc;

	[Header("Movement")]

	[Header("Velocity")]
	public float WalkSpeed = Config.Player.Movement.WalkSpeed;
	public float RunSpeed = Config.Player.Movement.RunSpeed;
	public float StopSpeed = 10;

	[Header("Max Velocity")]
	public float MaxWalkSpeed = Config.Player.Movement.WalkSpeed;
	public float MaxRunSpeed = Config.Player.Movement.RunSpeed;

	public Variable<Vector2> DesiredCharDirection = new Variable<Vector2>();

	[Space]
	[SerializeField] private readonly Variable<float> _CurrentSpeed = new Variable<float>(0);
	public float CurrentSpeed {
		get {
			return _CurrentSpeed.Value;
		}
	}

	[SerializeField] public Variable<float> DesiredSpeed = new Variable<float>();

	[Header("Backpack")]
	public List<Tool> Inventory;

	public Transform InventoryBlock;

	[SerializeField] private Tool _CurrentTool;
	public Tool CurrentTool {
		get { return _CurrentTool; }
	}

	public void Move(Vector2 direction) {
		Rigidbody.position += new Vector2(direction.x, direction.y);
	}


	public void SwitchTool(Tool tool) {
		if (tool.Holder.Value = this) {
			CurrentTool?.Unequip();
			_CurrentTool = tool;

			tool.Equip();
		}
	}

	public override void Awake() {
		base.Awake();

		InventoryBlock = (gameObject.transform.transform.Find("Inventory")) ? gameObject.transform.transform.Find("Inventory") : (new GameObject()).transform;
		InventoryBlock.parent = gameObject.transform;

		Model = gameObject.transform.Find("Model");

		//Getting every tool in inventory and setting controller.
		foreach (Transform tool in InventoryBlock) {
			Tool val = tool.gameObject.GetComponent(typeof(Tool)) as Tool;
			val.Holder.Value = this;
			Inventory.Add(val);
		}
	}


	public override void Start() {
		base.Start();

		Maid.GiveTask(new Action(delegate () {
			foreach (Tool tool in Inventory) {
				tool.Dispose();
			}
		}));

		Listener<int> onHealthChange = Health.Connect((int val) => {
			if (val < 0) {
				this.Dispose();
				return false;
			}
			return true;
		});

		//This binded functions smoothly updates Character position depending on CharDirection.
		Listener<float> moveUpdate = Runservice.BindToFixedUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
			Vector2 DesiredVelocity = DesiredCharDirection.Value.normalized * DesiredSpeed.Value;
			//Vector2 newVel = Rigidbody.velocity.AsVector2();
			/*
			if (DesiredVelocity.magnitude == 0 && newVel.magnitude < (Config.Player.Movement.AccelSpeed * dt * 1.1)) {
				newVel = new Vector2();
			} else {
				if (Mathf.Abs(newVel.x - DesiredVelocity.x) > (Config.Player.Movement.AccelSpeed * dt)) {
					if (newVel.x < DesiredVelocity.x) {
						newVel.x = Mathf.Clamp(newVel.x + (Config.Player.Movement.AccelSpeed * dt), -RunSpeed, RunSpeed);
					} else if (newVel.x > DesiredVelocity.x) {
						newVel.x = Mathf.Clamp(newVel.x - (Config.Player.Movement.AccelSpeed * dt), -RunSpeed, RunSpeed);
					}
				}
				if (Mathf.Abs(newVel.y - DesiredVelocity.y) > (Config.Player.Movement.AccelSpeed * dt)) {
					if (newVel.y < DesiredVelocity.y) {
						newVel.y = Mathf.Clamp(newVel.y + (Config.Player.Movement.AccelSpeed * dt), -RunSpeed, RunSpeed);
					} else if (newVel.y > DesiredVelocity.y) {
						newVel.y = Mathf.Clamp(newVel.y - (Config.Player.Movement.AccelSpeed * dt), -RunSpeed, RunSpeed);
					}
				}
			}
			_CurrentSpeed.Value = newVel.magnitude;
			*/

			if (DesiredCharDirection.Value.x < 0) {
				FlipX = true;
			} else if (DesiredCharDirection.Value.x > 0) {
				FlipX = false;
			}

			if (DesiredCharDirection.Value.magnitude == 0) {
				Rigidbody.AddForce((new Vector2(Rigidbody.velocity.x, 0) * -StopSpeed) * dt);
			} else if (Rigidbody.velocity.magnitude < MaxWalkSpeed) {
				Rigidbody.AddForce(((DesiredCharDirection.Value.normalized * RunSpeed) - Rigidbody.velocity) * dt);
			}


			_CurrentSpeed.Value = Rigidbody.velocity.magnitude;
			Animator.SetFloat("Velocity", _CurrentSpeed.Value);
			return true;
		});
		moveUpdate.Name = "moveUpdate";
		Maid.GiveTask(moveUpdate);
	}
}
