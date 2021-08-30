using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A "static" class that loads <c>Singletons</c> into the game. Don't mess with this since this handles integration.
/// </summary>
public sealed class LoaderService : Singleton {
	//Starting singletons.
	public void Start() {
		/// <summary>
		/// Destroys and disconnects every single Maid object, and quits application when the user presses escape.
		/// </summary>

	}

	//Disposing every singleton.
	public void OnApplicationQuit() {
		Maid.KillAll();
	}
}