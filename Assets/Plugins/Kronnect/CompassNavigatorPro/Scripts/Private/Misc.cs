using UnityEngine;
using System.Collections;
using System;

namespace CompassNavigatorPro {
	public static class Misc {
		public static Vector4 Vector4back = new Vector4 (0, 0, -1, 0);

		public static Vector3 Vector3one = Vector3.one;
		public static Vector3 Vector3zero = Vector3.zero;
		public static Vector3 Vector3back = Vector3.back;
		public static Vector3 Vector3left = Vector3.left;
		public static Vector3 Vector3right = Vector3.right;
		public static Vector3 Vector3up = Vector3.up;
		public static Vector3 Vector3down = Vector3.down;
		public static Vector3 Vector3half = new Vector3(0.5f, 0.5f, 0.5f);

		public static Vector2 Vector2left = Vector2.left;
		public static Vector2 Vector2right = Vector2.right;
		public static Vector2 Vector2one = Vector2.one;
		public static Vector2 Vector2zero = Vector2.zero;
		public static Vector2 Vector2down = Vector2.down;
		public static Vector2 Vector2up = Vector2.up;
		public static Vector2 Vector2max = new Vector2 (100000f, 100000f);
		public static Vector2 Vector2half = new Vector2(0.5f, 0.5f);

		public static Vector3 ViewportCenter = new Vector3 (0.5f, 0.5f, 0.0f);

        public static Color ColorTransparent = new Color (0, 0, 0, 0);
		public static Color ColorWhite = Color.white;

		public static Quaternion QuaternionZero = Quaternion.Euler (0f, 0f, 0f);

		public static Rect GetScreenRect (this RectTransform o) {
			Vector2 size = Vector2.Scale (o.rect.size, o.lossyScale);
            Rect rect = new Rect(o.position.x, o.position.y, size.x, size.y);
			rect.x -= (o.pivot.x * size.x);
			rect.y -= (o.pivot.y * size.y);
			return rect;
		}

        static readonly Vector3[] wc = new Vector3[4];

		public static Rect GetScreenRect (this RectTransform o, Camera camera) {
			o.GetWorldCorners (wc);
			return new Rect (wc [0].x, wc [0].y, wc [2].x - wc [0].x, wc [2].y - wc [0].y);
		}


		public static Rect GetViewportRect (this RectTransform o, Camera camera) {
			Rect rect = o.GetScreenRect (camera);
			rect.x /= camera.pixelWidth;
			rect.y /= camera.pixelHeight;
			rect.width /= camera.pixelWidth;
			rect.height /= camera.pixelHeight;
			return rect;
		}

		public static WaitForSeconds WaitForOneSecond = new WaitForSeconds(1f);

        public static T FindObjectOfType<T>(bool includeInactive = false) where T: UnityEngine.Object {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindAnyObjectByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
#else
            return UnityEngine.Object.FindObjectOfType<T>(includeInactive);
#endif
        }

        public static UnityEngine.Object[] FindObjectsOfType(Type type, bool includeInactive = false) {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindObjectsByType(type, includeInactive ? FindObjectsInactive.Include: FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            return UnityEngine.Object.FindObjectsOfType(type, includeInactive);
#endif
        }


        public static T[] FindObjectsOfType<T>(bool includeInactive = false) where T: UnityEngine.Object {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindObjectsByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            return UnityEngine.Object.FindObjectsOfType<T>(includeInactive);
#endif
        }

    }
}