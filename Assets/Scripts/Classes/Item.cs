using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// A class that defines objects which the character can store, but not necessarily use unlike tools.
/// Note: Most functions regarding items are not yet implemented, such as inventory management.
/// </summary>
public class Item : Part {

	private Variable<bool> Enabled = new Variable<bool>(false);

	[Header("Character Info")]
	/// <summary>
	/// Defines who has the item stored in their inventory.
	/// </summary>
	public Variable<Character> Holder = new Variable<Character>();

	[Header("Inventory Attributes")]
	/// <summary>
	/// Defines the size of the item in inventory.
	/// </summary>
	public Vector2 Size = new Vector2(1, 1);
	[SerializeField] protected bool CanBeDropped = true;
}
