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
	private bool _FlipX = false;
	private bool _FlipY = false;
	public bool FlipX  {
		get {return _FlipX;}
		set { 
			_FlipX = value; 
			foreach (Part v in Children) {
				if (v) {
					v.FlipX = value;
				}
			}
			foreach (SpriteRenderer v in Renders) {
				if (v) {
					v.flipX = value;
				}
			}
		}
	}
	public bool FlipY  {
		get {return _FlipY;}
		set { 
			_FlipY = value; 
			foreach (Part v in Children) {
				if (v) {
					v.FlipY = value;
				}
			}
			foreach (SpriteRenderer v in Renders) {
				if (v) {
					v.flipY = value;
				}
			}
		}
	}


	private List<SpriteRenderer> Renders = new List<SpriteRenderer>();
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
		foreach (SpriteRenderer v in Model.GetComponentsInChildren(typeof(SpriteRenderer))) {
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
			foreach (SpriteRenderer v in Renders) {
				if (v) {
					v.enabled = val;
				}
			}
			return true;
		});
		visibleChange.Name = "visibleChange";
		Maid.GiveTask(visibleChange);
		Visible.Call();
	}

	public virtual void Dispose() {
		Maid.Dispose();
		if (this && this.gameObject) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
