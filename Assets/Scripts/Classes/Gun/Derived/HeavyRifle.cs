using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyRifle : Gun {
	[Header("3D Objects")]
	public Part Magazine;
	public Part Casing;

	protected override void Shoot() {
		base.Shoot();

		//Knockback on gun hit on enemy.
		LastProjectile.Hit.Connect((Transform hit) => {
			if (hit.gameObject.GetComponent<Character>()) {
				float counter = .05F;
				Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
					if (counter > 0 && hit) {
						hit.position += LastProjectile.Direction.AsVector3() * 10 * dt;
						counter -= dt;
					} else {
						return false;
					}
					return true;
				});
			}
			return false;
		});

		Transform magWell = this.gameObject.transform.FindDeepChild("Magwell").transform;

		Part copy = Object.Instantiate(Casing, magWell.position, magWell.rotation);
		copy.Visible.Value = true;
		copy.Layer.Value = 1;

		CFrame copyCFrame = copy.transform.GetCFrame();
		Vector3 copyNewPos = (copy.transform.position + (gameObject.transform.GetCFrame().rightVector * 2) - new Vector3(-.5F, 0, 5));

		//Measures the amount of time before reloading gun.
		float counter = 0;
		Listener<float> casingEffect = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics - 1, (float dt) => {
			counter += dt;

			copyCFrame = copy.transform.GetCFrame();
			copy.transform.UpdateFromCFrame(copyCFrame.Lerp(new CFrame(copyNewPos) * (copyCFrame - copyCFrame.p), dt * 10));

			if (counter < .4) {
				copyCFrame = copy.transform.GetCFrame();
				copy.transform.UpdateFromCFrame(copyCFrame * CFrame.FromEulerAnglesXYZ(0, 0, Random.Range(-420 * dt, -240 * dt)));
			}

			if (counter > 2) {
				copy.Dispose();
				return false;
			}
			return true;
		});
		casingEffect.Name = "casingEffect";
		copy.Maid.GiveTask(casingEffect);
	}

	// Start is called before the first frame update
	public override void Reload() {
		if (!IsReloading && Ammo.Value < MaxAmmo) {
			base.Reload();

			Part copy = Object.Instantiate(Magazine, Magazine.transform.position, Magazine.transform.rotation);
			copy.gameObject.transform.localScale = new Vector3(.15F, 1, 1);
			copy.Visible.Value = true;
			copy.Layer.Value = 1;

			CFrame copyCFrame = copy.transform.GetCFrame();
			CFrame copyNewCFrame = new CFrame();

			Magazine.Visible.Value = false;

			//Measures the amount of time before reloading gun.
			float counter = 0;
			Listener<float> magazineEffect = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics - 1, (float dt) => {
				counter += dt;

				if (copy && Magazine) {
					if (counter > .5) {
						copy.transform.UpdateFromCFrame((copy.transform.GetCFrame()).Lerp(copyNewCFrame, dt * 10));
					} else {
						copy?.transform.UpdateFromCFrame(Magazine.transform.GetCFrame());

						copyCFrame = copy.transform.GetCFrame();
						copyNewCFrame = new CFrame(copy.transform.position + copyCFrame.rightVector - new Vector3(0, 0, 5)) * (copyCFrame - copyCFrame.p) * CFrame.FromEulerAnglesXYZ(0, 0, Random.Range(-45, 45) * Mathf.Deg2Rad);
					}

					if (counter > ReloadLength) {
						copy.Dispose();
						Magazine.Visible.Value = true;
						return false;
					}
					return true;
				} else {
					return false;
				}
			});
			magazineEffect.Name = "magazineEffect";
			copy.Maid.GiveTask(magazineEffect);
		}
	}
}
