using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// A guideline
/// </summary>
public class EXAMPLE {
	Maid Maid = new Maid(); //Initializes and constructs a Maid object, which is a "cleanup" class, which destroys connected objects.

	public void Start() {
		//The Listener object, example, is a function that binds to Runservice to fire after Camera is rendered (Heartbeat).
			//Note that lambda functions are anonymous functions that can be assigned as a variable. They don't need a name unlike regular functions. Useful for being passed as a parameter such in this case.
		//Listener<float> => The float defines what type will be pased into the function.
		//Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {})
			//BindToUpdate() => A function that binds another function to run after Camera renders. 
			//Global.RunservicePriority.Heartbeat.Physics => The defined int priority. The lower the number, the earlier it is run compared to other binded functions. Check up on Global.cs for the list of priorities.
			//(float dt) => {} => (float dt) are the parameters - analagous to function parameters. The "=>" point to the lambda body. The brackets are the lambda body.
				//In this case, dt stands for the time between each frame.

		Listener<float> example = Runservice.BindToUpdate(Global.RunservicePriority.Heartbeat.Physics, (float dt) => {
			//yadadadada
			return true; //Every Listener object's function must return a boolean. If it returns true, that means that the binded function can run again. If it returns false, that means the function is discarded and not able to run again.
		});
		example.Name = "example"; //Every Listener object can be assigned a name. This makes it useful for debugging, but is not necessary. It's just good practice.
		Maid.GiveTask(example); //The Maid is given this Listener<float> example object, so whenever Maid is destroyed, so too is Listener<float> example.

		//Listener<bool> => The bool defines what type will be pased into the function.
		//InputController.Keyboard[KeyCode.U].Connect((bool val) => {})
			//InputController.Keyboard => Returns a dictionary of Keycodes from which functions can bind to. 
			//[KeyCode.U] => Returns specifically the KeyCode.U key.
			//[KeyCode.U].Connect(bool val) => Connects the function to fire whenever KeyCode.U is pressed or released.
				//bool val => In this case, defines if the key is pressed or released. true = pressed, false = released.
		Listener<bool> foo = InputController.Keyboard[KeyCode.U].Connect((bool val) => {
			//yadadadadad
			return false; //Since this binded function will return false, it runs once then automatically discarded.
		});
		foo.Name = "foo";
		Maid.GiveTask(foo); 
	}
}
