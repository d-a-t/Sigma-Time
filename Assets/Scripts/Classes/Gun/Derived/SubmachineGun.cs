using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmachineGun : Gun {
	[Header("3D Objects")]
	public Part Magazine;
	public Part Casing;

	public override void Awake() {
        base.Awake();

		Magazine.Visible.Locked = true;
	}

	public override void Start() {
		base.Start();
		Weld.C0 = new CFrame(new Vector3(.5F, 0, 0));
	}

	protected override void Shoot() {
		base.Shoot();

		Transform reciever = this.gameObject.transform.FindDeepChild("Reciever").transform;

		Part copy = Object.Instantiate(Casing, reciever.position, reciever.rotation);
		copy.Visible.Value = true;
		copy.Layer.Value = 1;

		CFrame copyCFrame = copy.transform.GetCFrame();
		Vector3 copyNewPos = (copy.transform.position + (gameObject.transform.GetCFrame().rightVector * 2) - new Vector3(0, 0, 5));

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
			CFrame copyCFrame = copy.transform.GetCFrame();
			CFrame copyNewCFrame = new CFrame();

			//Measures the amount of time before reloading gun.
			float counter = 0;
			Listener<float> magazineEffect = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics - 1, (float dt) => {
				counter += dt;

				if (counter > .5) {
					copy.transform.UpdateFromCFrame((copy.transform.GetCFrame()).Lerp(copyNewCFrame, dt * 10));
                    copy.Visible.Locked = false;
					copy.Visible.Value = true;
					copy.Layer.Value = 1;
				} else {
					copy.transform.UpdateFromCFrame(Magazine.transform.GetCFrame());

					copyCFrame = copy.transform.GetCFrame();
					copyNewCFrame = new CFrame(copy.transform.position + copyCFrame.rightVector - new Vector3(0, 0, 5)) * (copyCFrame - copyCFrame.p) * CFrame.FromEulerAnglesXYZ(0, 0, Random.Range(-45, 45) * Mathf.Deg2Rad);
				}

				if (counter > ReloadLength) {
					copy.Dispose();
					return false;
				}
				return true;
			});
			magazineEffect.Name = "magazineEffect";
			copy.Maid.GiveTask(magazineEffect);
		}
	}
}
