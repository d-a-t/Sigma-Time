using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// A class that attaches itself to a empty gameObject. Makes it easy to toggle if it should render.
/// </summary>
public class Part : MonoBehaviour, IDisposable {
	public Maid Maid = new Maid();


	[Header("Positional")]
	/// <summary>
	/// Defines the 2D model which contains the sprites. Useful for a collage of sprites acting as one.
	/// </summary>
	public Transform Model;

	public Collider2D Collider;
	public Rigidbody2D Rigidbody;

	[Header("Rendering")]
	public Variable<int> Layer = new Variable<int>(2);

	private List<Renderer> Renders = new List<Renderer>();
	private List<Part> Children = new List<Part>();

	/// <summary>
	/// Defines if the model should be rendered.
	/// </summary>
	public Variable<bool> Visible = new Variable<bool>(true);

	public virtual void Awake() {
		if (Model == null) {
			Transform child = (new GameObject("Model")).transform;
			child.parent = this.gameObject.transform;
			Model = child;
			Model.localPosition = new Vector3();
		}

		foreach (Part v in Model.GetComponentsInChildren(typeof(Part))) {
			Children.Add(v);
		}
		foreach (Renderer v in Model.GetComponentsInChildren(typeof(Renderer))) {
			if (Children.Count > 0) {
				foreach (Part b in Children) {
					if (!v.gameObject.transform.IsChildOf(b.gameObject.transform)) {
						Renders.Add(v);
					}
				}
			} else {
				Renders.Add(v);
			}
		}
	}

	// Start is called before the first frame update
	public virtual void Start() {
		//Changes if the part should be rendered depending on the Visible value.
		Listener<bool> visibleChange = Visible.Connect((bool val) => {
			foreach (Part v in Children) {
				if (v) {
					v.Visible.Value = val;
				}
			}
			foreach (Renderer v in Renders) {
				if (v) {
					v.enabled = val;
				}
			}
			return true;
		});
		visibleChange.Name = "visibleChange";
		Maid.GiveTask(visibleChange);

		Visible.Call();

		//Changes if the part should be rendered depending on the Visible value.
		Listener<int> layerChange = Layer.Connect((int val) => {
			foreach (Part v in Children) {
				v.Layer.Value = val;
			}
			foreach (Renderer v in Renders) {
				v.sortingOrder = val;
			}
			return true;
		});
		layerChange.Name = "layerChange";
		Maid.GiveTask(layerChange);

		Layer.Call();
	}

	public virtual void Dispose() {
		Maid.Dispose();
		if (this && this.gameObject) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
