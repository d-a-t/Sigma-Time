using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config {
	namespace Player {
		public static class Camera {
			public static Vector2 PivotPoint = new Vector3(0,0);
			public static int FOV = 75;
			public static float NearClipPlane = .01F;
		}
		public static class Movement {
			public static float WalkSpeed = 10;
			public static float RunSpeed = 20;

			public static float AccelSpeed = 100;
		}
	}
}
