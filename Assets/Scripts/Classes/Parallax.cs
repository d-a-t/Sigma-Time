using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParallaxBackground {
	public Transform Background;
	public float Effect;
    internal Vector2 Start;
}

public class Parallax : MonoBehaviour {
	public Camera Cam;

	public List<ParallaxBackground> Backgrounds = new List<ParallaxBackground>();

	private Vector2 CamStartPos;
	void Start() {
		Cam = Cam ? Cam : CameraController.Cam.Value;
		CamStartPos = Cam.transform.position.AsVector2();

		foreach (ParallaxBackground v in Backgrounds) {
			v.Start = v.Background.position;
		}
	}

	// Update is called once per frame
	void FixedUpdate() {
		foreach (ParallaxBackground v in Backgrounds) {
			Vector2 pos = (CameraController.Cam.Value.transform.position.AsVector2() - CamStartPos);
			v.Background.position = v.Start.AsVector3() + new Vector3(v.Start.x + pos.x / v.Effect, 0, 0);
		}
	}
}
