using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// honestly this class may not be used. Originally it's used to switch between Cameras on the fly, but we don't really have that.
/// </summary>
public sealed class CameraController : Singleton {
	public static Variable<Camera> Cam = new Variable<Camera>();

	public static Canvas GUI;
	private Canvas _GUI;

	public Variable<GameObject> Target = new Variable<GameObject>();

	private Listener<float> CamUpdate;

	public void Awake() {
		GUI = _GUI? _GUI : GUI;
		
		Cam.Value = Camera.main;

		Cam.Value.nearClipPlane = Config.Player.Camera.NearClipPlane;
		Cam.Value.fieldOfView = Config.Player.Camera.FOV;

		Target.Value = GameObject.Find("Sprite");
	}

}
