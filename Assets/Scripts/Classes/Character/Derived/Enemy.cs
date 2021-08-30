using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Used for enemies.
/// </summary>
public class Enemy : Character {
	//Defines the pseudoMouse of Enemy
	public Vector2 Mouse = new Vector2();
	public Transform Target;

	public KeyBoardDict Controls = new KeyBoardDict();

	[Header("Stats")]
	public float AimingSpeed = 5F;
	public float ActionCoolDown = 2F;
	public int score = 100; //points to add to score after being killed

	[Header("Logic")]
	public bool PointingToTarget = false;
	public bool LockToTarget = false;
	public Variable<bool> AwaitingCoolDown = new Variable<bool>();
	public (float, float) OptimalRange = (7, 20);

	public void PointTowardsTarget() {
		PointingToTarget = true;
	}

	public void WalkTowardsTarget() {
		if (Target) {
			DesiredCharDirection = (Target.position - transform.position).AsVector2();
		}
	}

	public void Stop() {
		DesiredCharDirection = new Vector2();
	}

	public void BurstShoot() {
		if (!AwaitingCoolDown.Value) {
			AwaitingCoolDown.Value = true;

			float counter = 0;
			Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
				if (this) {
					counter += dt;
					if (counter < 1F) {
						Controls[KeyCode.Mouse0].Value = true;
					} else {
						Controls[KeyCode.Mouse0].Value = false;
						return false;
					}
					return true;
				}
				return false;
			});
		}
	}

	public override void Start() {
		base.Start();
		ActionCoolDown = UnityEngine.Random.Range(2F, 3.5F);

		Listener<float> pointingTowardsTarget = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
			if ((PointingToTarget || LockToTarget) && Target) {
				if (((Mouse + gameObject.transform.position.AsVector2()) - Target.position.AsVector2()).SqrMagnitude() < 5) {
					PointingToTarget = false;
				}
				Mouse = Vector2.Lerp(Mouse, gameObject.transform.position.AsVector2() - Target.position.AsVector2(), AimingSpeed * dt * Mathf.Clamp(((Mouse + gameObject.transform.position.AsVector2()) - Target.position.AsVector2()).SqrMagnitude(), 1, 50));
			}
			return true;
		});
		Maid.GiveTask(pointingTowardsTarget);

		Listener<bool> refreshingCoolDown = AwaitingCoolDown.Connect((bool val) => {
			if (val) {
				AwaitingCoolDown.Locked = true;

				float counter = 0;
				Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
					if (this) {
						counter += dt;
						if (counter > ActionCoolDown) {
							AwaitingCoolDown.Locked = false;
							AwaitingCoolDown.Value = false;

							return false;
						}
						return true;
					}
					return false;
				});
			}
			return true;
		});
		Maid.GiveTask(refreshingCoolDown);
	}
}
