using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// A Gun is a tool derived class. It has 3 functions which can be binded to controls: Shooting, Reloading, and Equipping.
/// Whenever it fires, it spawns a Projectile object which shoots towards wherever the mouse is at.
/// Note: There must be a gameObject in the Gun named Firepoint, which defines the muzzle of the gun from which the Projectiles spawn.
/// </summary>
public class Gun : Tool {
	protected bool IsShooting = false;
	protected bool IsReloading = false;
	protected bool IsAiming = false;
	protected bool InBattery = false;
	protected bool CanShoot = true;

	protected Projectile LastProjectile;

	public Vector2 TargetPosition = new Vector2();

	[Header("Stats")]
	/// <summary>
	/// How fast the gun is shooting, as defined in Rounds Per Minutes (RPM)
	/// </summary>
	public int RPM = 900;
	/// <summary>
	/// How much ammo the Gun currently has as it first spawns.
	/// </summary>
	public Variable<int> Ammo = new Variable<int>(30);
	/// <summary>
	/// The maximum capacity of the Gun, reloads to this amount.
	/// </summary>
	public int MaxAmmo = 30;
	/// <summary>
	/// How long it takes to reload the gun in seconds.
	/// </summary>
	public float ReloadLength = 2;
	/// <summary>
	/// How much damage the gun does per shot.
	/// </summary>	
	public int Damage = 30;

	[Header("Sounds")]
	/// <summary>
	/// The AudioSource that plays whenever the Gun shoots. By default, it's a generic Gunshot sound.
	/// </summary>
	public AudioSource Gunshot;
	/// <summary>
	/// The AudioSource that plays whenever the Gun reloads. By default, it's a generic Reload sound.
	/// </summary>
	public AudioSource ReloadEffect;

	private Transform _Firepoint;

	private List<Event> Listeners = new List<Event>();

	public override bool Equip() {
		if (base.Equip()) {
			this.gameObject.transform.parent = null;
			Weld.Active = true;
			return true;
		}
		return false;
	}

	public override bool Unequip() {
		if (base.Unequip()) {
			this.gameObject.transform.parent = this.Holder.Value.InventoryBlock;
			Weld.Active = false;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Reloads the Gun if its Ammo is not at MaxAmmo and not already reloading.
	/// </summary>
	public virtual void Reload() {
		if (!IsReloading && Ammo.Value < MaxAmmo) {
			CanUnequip = false;

			AudioSource temp = ReloadEffect.PlayClipAtPoint(Camera.main.transform.position);
			temp.Play();
			IsReloading = true;

			//Measures the amount of time before reloading gun.
			float reloadCounter = 0;
			Listener<float> reloadGunCounter = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics - 1, (float dt) => {
				if (Equipped.Value) {
					if (IsReloading) {
						if (Ammo.Value < MaxAmmo) {
							//Controller.Value.GUI.Find("Ammo").GetComponent<Text>().text = "...";
							CanShoot = false;
							reloadCounter += dt;
						}
					}
					if (reloadCounter > ReloadLength) {
						reloadCounter = 0;

						IsReloading = false;
						CanShoot = true;

						Ammo.Value = MaxAmmo;
						//Controller.Value.GUI.Find("Ammo").GetComponent<Text>().text = Ammo.ToString();

						CanUnequip = true;
						return false;
					}
				}
				return true;
			});
			reloadGunCounter.Name = "reloadGunCounter";
			Maid.GiveTask(reloadGunCounter);
		}
	}

	protected virtual void Shoot() {
		InBattery = false;
		Ammo.Value -= 1;

		//Controller.Value.GUI.Find("Ammo")?.GetComponent<Text>()?.text = Ammo.ToString();

		AudioSource temp = Gunshot.PlayClipAtPoint(Camera.main.transform.position);
		temp.Play();
		temp.SetScheduledEndTime(AudioSettings.dspTime + (1.2F));

		Projectile bullet = Projectile.Create(Projectile.Types.Fastcast, _Firepoint.position, TargetPosition - _Firepoint.position.AsVector2());
		bullet.Damage = Damage;
		bullet.IgnoreColliders.Add(Controller.Value.Collider);
		LastProjectile = bullet;

		float recoilDtCounter = 0F;
		Listener<float> recoilGun = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics - 1, (float dt) => {
			if (recoilDtCounter < (30F / RPM)) {
				Weld.C1 *= (new CFrame(0, 1F * dt * 10, 0));
				recoilDtCounter += dt;
				return true;
			}
			return false;
		});
		recoilGun.Name = "recoilGun";
	}

	protected override void BindControls() {
		base.BindControls();

		if (Controller.Value == PlayerController.Char) {
			//Binds the Mouse0 to shoot the gun if equipped.
			Listeners.Add((Event)InputController.Keyboard[KeyCode.Mouse0].Connect((bool val) => {
				if (Equipped.Value) {
					if (val) {
						IsShooting = true;
					} else {
						IsShooting = false;
					}
				}
				return true;
			}));

			//Binds the R Key to reload the gun if the gun is equipped.
			Listeners.Add((Event)InputController.Keyboard[KeyCode.R].Connect((bool val) => {
				if (Equipped.Value) {
					if (val) {
						Reload();
					}
				}
				return true;
			}));

			//Updates the position of the gun to always point to wherever the mouse is, and move back if the mouse is too close to the player.
			Listeners.Add((Event)Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics - 1, (float dt) => {
				TargetPosition = InputController.Mouse.Position;
				if (Controller.Value != null) {
					float aimDistance = (Controller.Value.gameObject.transform.position.AsVector2() - TargetPosition).magnitude;

					if (aimDistance < 2) {
						Weld.C0 = (new CFrame(Weld.C0.p)) * CFrame.FromEulerAnglesXYZ(0, 0, Mathf.Atan2(.5F, aimDistance + 2));

						Weld.C0.p.y = Weld.C0.p.y + 10 * dt * (-1 - Weld.C0.p.y);
					} else {
						Weld.C0 = (new CFrame(Weld.C0.p)) * CFrame.FromEulerAnglesXYZ(0, 0, Mathf.Atan2(.5F, aimDistance));

						Weld.C0.p.y = Weld.C0.p.y + 10 * dt * (0 - Weld.C0.p.y);
					}

					if (Weld.C1.p.magnitude > .01) {
						Weld.C1 = Weld.C1.Lerp(new CFrame(), dt * 25);
					} else {
						Weld.C1 = new CFrame();
					}
				}
				return true;
			}));

			//Repeats shooting of the gun as long as binded key to shoot is held, and also makes the recoiling effect of the gun.
			float dtCounter = 0;
			Listeners.Add((Event)Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics - 1, (float dt) => {
				if (InputController.Keyboard[KeyCode.Mouse0].Value && Equipped.Value) {
					if (Ammo.Value == 0) {
						CanShoot = false;
					}
					if (InBattery && CanShoot) {
						if (IsShooting) {
							Shoot();
							if (dtCounter != 0) {
								dtCounter = 0;
							}
						}
					} else {
						dtCounter += dt;
						if (dtCounter >= (60F / RPM)) {
							InBattery = true;
						}
					}
				}
				return true;
			}));
		} else {
			Enemy enemy = (Enemy)Controller.Value;
			//Binds the Mouse0 to shoot the gun if equipped.
			Listeners.Add((Event)enemy.Controls[KeyCode.Mouse0].Connect((bool val) => {
				if (Equipped.Value) {
					if (val) {
						IsShooting = true;
					} else {
						IsShooting = false;
					}
				}
				return true;
			}));

			//Binds the R Key to reload the gun if the gun is equipped.
			Listeners.Add((Event)enemy.Controls[KeyCode.R].Connect((bool val) => {
				if (Equipped.Value) {
					if (val) {
						Reload();
					}
				}
				return true;
			}));

			//Updates the position of the gun to always point to wherever the mouse is, and move back if the mouse is too close to the player.
			Listeners.Add((Event)Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics - 1, (float dt) => {
				TargetPosition = enemy.gameObject.transform.position.AsVector2() - enemy.Mouse;
				if (Controller.Value != null) {
					float aimDistance = (Controller.Value.gameObject.transform.position.AsVector2() - TargetPosition).magnitude;

					if (aimDistance < 2) {
						Weld.C0 = (new CFrame(Weld.C0.p)) * CFrame.FromEulerAnglesXYZ(0, 0, Mathf.Atan2(.5F, aimDistance + 2));

						Weld.C0.p.y = Weld.C0.p.y + 10 * dt * (-1 - Weld.C0.p.y);
					} else {
						Weld.C0 = (new CFrame(Weld.C0.p)) * CFrame.FromEulerAnglesXYZ(0, 0, Mathf.Atan2(.5F, aimDistance));

						Weld.C0.p.y = Weld.C0.p.y + 10 * dt * (0 - Weld.C0.p.y);
					}

					if (Weld.C1.p.magnitude > .01) {
						Weld.C1 = Weld.C1.Lerp(new CFrame(), dt * 25);
					} else {
						Weld.C1 = new CFrame();
					}
				}
				return true;
			}));

			//Repeats shooting of the gun as long as binded key to shoot is held, and also makes the recoiling effect of the gun.
			float dtCounter = 0;
			Listeners.Add((Event)Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics - 1, (float dt) => {
				if (enemy.Controls[KeyCode.Mouse0].Value && Equipped.Value) {
					if (Ammo.Value == 0) {
						CanShoot = false;
					}
					if (InBattery && CanShoot) {
						if (IsShooting) {
							Shoot();
							if (dtCounter != 0) {
								dtCounter = 0;
							}
						}
					} else {
						dtCounter += dt;
						if (dtCounter >= (60F / RPM)) {
							InBattery = true;
						}
					}
				}
				return true;
			}));
		}
	}

	protected override void UnbindControls() {
		base.UnbindControls();

		foreach (Event listener in Listeners) {
			listener.Destroy();
		}
	}

	public override void Awake() {
		base.Awake();

		//Finds the required Firepoint, if not, make one.
		_Firepoint = gameObject.transform.FindDeepChild("Firepoint");
		if (_Firepoint == null) {
			_Firepoint = (new GameObject()).transform;
			_Firepoint.parent = this.gameObject.transform;
		}

		//Defines the Weld Offset for the gun so it can stay on the right side of the character.
		Weld.C0 = new CFrame(.5F, 0, 0);
		Weld.C1 = new CFrame();

		//Reverts the Gunshot AudioSource to the generic Gunshot sound if it's not defined in the Gun.
		if (Gunshot == null) {
			AudioSource temp = this.gameObject.AddComponent<AudioSource>();
			temp.clip = Resources.Load<AudioClip>("Sounds/Gunshot");
			Gunshot = temp;
		}
		Gunshot.volume = SoundController.EffectVolume.Value;

		//Reverts the Reload AudioSource to the generic Reload sound if it's not defined in the Gun.
		if (ReloadEffect == null) {
			AudioSource temp = this.gameObject.AddComponent<AudioSource>();
			temp.clip = Resources.Load<AudioClip>("Sounds/Reload");
			ReloadEffect = temp;
		}
		ReloadEffect.volume = SoundController.EffectVolume.Value;
	}

	// Start is called before the first frame update
	public override void Start() {
		base.Start();

		Maid.GiveTask(new Action(delegate () {
			UnbindControls();
		}));
	}
}
