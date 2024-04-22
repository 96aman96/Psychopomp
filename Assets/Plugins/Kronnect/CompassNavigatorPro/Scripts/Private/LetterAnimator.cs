using UnityEngine;
using UnityEngine.UI;

namespace CompassNavigatorPro {

    public delegate void OnAnimationEndDelegate(int poolIndex);

    public class LetterAnimator : MonoBehaviour {

        public float startTime, revealDuration, startFadeTime, fadeDuration;
        public Text text, textShadow;
        public int poolIndex;
        public OnAnimationEndDelegate OnAnimationEnds;
        public bool used;

        Vector3 originalScale;

        void Awake() {
            enabled = false;
        }

        void Update() {

            float now = Time.time;
            float elapsed = now - startTime;
            if (elapsed < revealDuration) { // revealing
                float t = Mathf.Clamp01(elapsed / revealDuration);
                UpdateTextScale(t);
                UpdateTextAlpha(t);
            } else if (now < startFadeTime) {
                UpdateTextScale(1.0f);
                UpdateTextAlpha(1.0f);
            } else if (now < startFadeTime + fadeDuration) {
                float t = Mathf.Clamp01(1.0f - (now - startFadeTime) / fadeDuration);
                UpdateTextAlpha(t);
            } else {
                OnAnimationEnds(poolIndex);
                enabled = false;
            }
        }


        public void Play() {
            if (originalScale.z == 0) {
                originalScale = text.transform.localScale;
            }
            enabled = true;
            Update();
        }

        void UpdateTextScale(float t) {
            Vector3 scale = originalScale;
            scale.x *= t;
            scale.y *= t;
            text.transform.localScale = scale;
            textShadow.transform.localScale = scale;
        }

        void UpdateTextAlpha(float t) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, t);
            textShadow.color = new Color(0, 0, 0, t);
        }



    }

}