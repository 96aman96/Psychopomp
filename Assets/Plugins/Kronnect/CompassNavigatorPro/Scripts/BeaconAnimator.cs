using UnityEngine;
using System.Collections;

namespace CompassNavigatorPro {
    public class BeaconAnimator : MonoBehaviour {

        public float intensity = 5f;
        public float duration;
        public Color tintColor;

        float startingTime;
        Material mat;
        Color fullyTransparentColor, originalColor;

        static class ShaderParams {
            public static int EmissionMap = Shader.PropertyToID("_EmissionMap");
            public static int EmissionColor = Shader.PropertyToID("_EmissionColor");
        }

        // Use this for initialization
        void Awake() {
            mat = GetComponent<Renderer>().material;
            fullyTransparentColor = new Color(0, 0, 0, 0);
            duration = 1f;
        }

        void Start() {
            startingTime = Time.time;
            originalColor = mat.color * tintColor * intensity;
            mat.SetColor(ShaderParams.EmissionColor, tintColor);
            UpdateColor();
        }

        void OnDisable() {
            DestroyBeacon();
        }

        // Update is called once per frame
        void Update() {
            float now = Time.time;
            mat.mainTextureOffset = new Vector2(now * -0.25f, now * -0.25f);
            mat.SetTextureOffset(ShaderParams.EmissionMap, new Vector2(now * -0.15f, now * -0.2f));
            UpdateColor();
        }

        void UpdateColor() {
            float elapsed = duration <= 0 ? 1f : Mathf.Clamp01((Time.time - startingTime) / duration);
            if (elapsed >= 1f) {
                DestroyBeacon();
                return;
            }
            float t = Ease(elapsed);
            mat.color = Color.Lerp(fullyTransparentColor, originalColor, t);
        }

        float Ease(float t) {
            return Mathf.Sin(t * Mathf.PI);
        }


        void DestroyBeacon() {
            if (mat != null) {
                DestroyImmediate(mat);
                mat = null;
            }
            if (Application.isPlaying) {
                Destroy(gameObject);
            }
        }
    }

}