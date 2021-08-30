using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Hivemind controller for enemies
/// </summary>
public sealed class EnemyController : Singleton {
	[SerializeField] private Enemy original;

	private static List<Enemy> EnemyList = new List<Enemy>();

	public List<Transform> Spawners = new List<Transform>();

	private Listener<float> SpawnEnemyInterval;

	public void SpawnEnemy(Vector3 pos) {
		Enemy copy = GameObject.Instantiate(original);
		copy.Visible.Value = true;
		copy.transform.position = pos;

		BindEnemy(copy);

		EnemyList.Add(copy);
	}

	public static void KillAll() {
		foreach (Enemy enemy in EnemyList) {
			enemy.Dispose();
		}
	}

	public void Start() {
		foreach (Enemy enemy in EnemyList) {
			BindEnemy(enemy);
		}

		float counter = 0;
		SpawnEnemyInterval = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
			if (PlayerController.Char) {
				if (counter > 5) {
					counter = 0;

					Vector3 farthest = new Vector3();
					if (EnemyList.Count < 4) {
						foreach (Transform spawner in Spawners) {
							if ((spawner.position - PlayerController.Char.transform.position).magnitude > (farthest - PlayerController.Char.transform.position).magnitude) {
								farthest = spawner.position;
							}
						}
					}
					SpawnEnemy(farthest);
				}
				counter += dt;
				return true;
			}
			return false;
		});
		Maid.GiveTask(SpawnEnemyInterval);
	}

	public override void Dispose() {
		Maid.Dispose();
	}

	public void ChangeGlobalTarget(Character target) {
		foreach (Enemy enemy in EnemyList) {
			enemy.Target = target.transform;
		}
	}

	public void BindEnemy(Enemy enemy) {
		enemy.Target = PlayerController.Char.transform;

		enemy.Maid.GiveTask(new Action(delegate {
			PlayerController.Score.Value += enemy.score;
			EnemyList.Remove(enemy);
		}));

		enemy.SwitchTool(enemy.Inventory[0]);

		//Binding a function to update the Character to face wherever the mouse is at BEFORE camera renders.
		Listener<float> updateEnemyRotation = Runservice.BindToLateUpdate(Global.RunservicePriority.RenderStep.Character, (float dt) => {
			float angles = (float)Math.Atan2(enemy.Mouse.y,enemy.Mouse.x) * Mathf.Rad2Deg;

			enemy.transform.eulerAngles = new Vector3(0, 0, angles + 90);

			enemy.PointTowardsTarget();
			return true;
		});
		updateEnemyRotation.Name = "updateEnemyRotation";
		enemy.Maid.GiveTask(updateEnemyRotation);

		//Binding a function to update the Character to face wherever the mouse is at BEFORE camera renders.
		Listener<float> updateEnemyPosition = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
			if (enemy.Target) {
				if ((enemy.Target.position - enemy.transform.position).magnitude > enemy.OptimalRange.Item1) {
					enemy.WalkTowardsTarget();
				} else {
					enemy.Stop();
				}
			} else {
				enemy.Stop();
			}
			return true;
		});
		updateEnemyPosition.Name = "updateEnemyPosition";
		enemy.Maid.GiveTask(updateEnemyPosition);

		//Binding a function to update the Character to face wherever the mouse is at BEFORE camera renders.
		Listener<float> opportunityShoot = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
			if (enemy.Target) {
				if (enemy.CurrentTool is Gun) {
					if (((Gun)enemy.CurrentTool).Ammo.Value <= 0) {
						enemy.Controls[KeyCode.R].Value = true;
						enemy.Controls[KeyCode.R].Value = false;
					}

					if (enemy ? ((enemy.transform.position - PlayerController.Char.transform.position).magnitude < enemy.OptimalRange.Item2) : false) {
						enemy.BurstShoot();
						return true;
					}
				}
			}
			return true;
		});
		opportunityShoot.Name = "opportunityShoot";
		enemy.Maid.GiveTask(opportunityShoot);

	}
}
