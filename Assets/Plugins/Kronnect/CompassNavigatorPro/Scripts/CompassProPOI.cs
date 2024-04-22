using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CompassNavigatorPro {

    public enum POIVisibility {
        WhenInRange = 0,
        AlwaysVisible = 1,
        AlwaysHidden = 2
    }

    public enum TitleVisibility {
        OnlyWhenVisited = 0,
        Always = 1
    }

    public delegate void OnHeartbeatEvent();

    [HelpURL("https://kronnect.com/guides-category/compass-navigator-pro-2/")]
    [AddComponentMenu("Compass Navigator Pro/Compass POI")]
    [ExecuteInEditMode]
    [DefaultExecutionOrder(101)]
    public partial class CompassProPOI : MonoBehaviour {

        [Tooltip("Unique ID to be used when DontDestroyOnLoad option is set to true.")]
        public int id;

        [Tooltip("Higher priority icons are rendered on top of others. This option only has effect for POIs loaded at the start. POIs loaded in runtime are added to the hierarchy and will render on top of the previous ones.")]
        public int priority;

        [Tooltip("POI visibility in compass bar.")]
        public POIVisibility visibility = POIVisibility.WhenInRange;

        [Tooltip("If enabled, the icon will stop at the edges of the bar even if it's behind the player.")]
        public bool clampPosition;

        [Tooltip("A value of 0 uses the global visible distance property from the Compass Bar settings. A value greater than 0 will override the global value for this POI. Useful when you need this POI to use a different visible distance.")]
        public float visibleDistanceOverride;

        [Tooltip("A value of 0 uses the global visible min distance property from the Compass Bar settings. A value greater than 0 will override the global value for this POI.")]
        public float visibleMinDistanceOverride;

        [Tooltip("A value of 0 uses the global min distance text from the Compass Bar settings. Distance text won't be shown for objects within this distance.")]
        public float titleMinPOIDistanceOverride;

        [Tooltip("Title to be shown when this POI is in the center of the compass bar and it's a known location (isVisited = true)")]
        public string title;

        [Tooltip("Rule for title visibility.")]
        public TitleVisibility titleVisibility = TitleVisibility.OnlyWhenVisited;

        [Tooltip("Specifies if this POI can be marked as visited when reached.")]
        public bool canBeVisited = true;

        [Tooltip("A value of 0 uses the global visited distance property from the Compass Bar settings. A value greater than 0 will override the global value for this POI. Useful when you need this POI to use a different visited distance.")]
        public float visitedDistanceOverride;

        [Tooltip("Specifies if POI must be removed from the UI when visited.")]
        public bool hideWhenVisited;

        [Tooltip("Specifies if this POI has been already visited.")]
        public bool isVisited;

        [Tooltip("Text to show when discovered. Leave this to null if you don't want to show any text.")]
        public string visitedText;

        public bool playAudioClipWhenVisited = true;

        [Tooltip("Sound to play when POI is visited the first time. If nothing set, the default audio-clip assigned in Compass Navigator Pro inspector will be used.")]
        public AudioClip visitedAudioClipOverride;

        [Tooltip("Radius of interest of this POI. Useful for area POIs, like rooms, cities or generic areas of search. The radius is used by the circle feature in the mini-map or to determine if player reaches an area.")]
        public float radius;

        [Tooltip("User defined icon scale multiplier.")]
        public float iconScale = 1f;

        [Tooltip("Show distance to the POI in the compass bar under the icon")]
        public bool iconShowDistance = true;

        [Tooltip("The icon for the POI if has not been discovered/visited.")]
        public Sprite iconNonVisited;

        [Tooltip("The icon for the POI if has been visited.")]
        public Sprite iconVisited;

        [Tooltip("Tinting color")]
        public Color tintColor = Color.white;

        [Tooltip("If the icon will be shown in the scene during playmode. If enabled, the indicator will fade in smoothly as the player approaches it.")]
        public bool showOnScreenIndicator = true;

        [Tooltip("Scale for this POI indicator")]
        public float onScreenIndicatorScale = 1f;

        [Tooltip("Show distance to camera for this POI indicator")]
        public bool onScreenIndicatorShowDistance = true;

        [Tooltip("Show title for this POI indicator")]
        public bool onScreenIndicatorShowTitle = true;

        [Tooltip("Optional offset added to the POI position to compute the distance or the screen coordinate of the visual indicators.")]
        public Vector3 positionOffset;

        [Tooltip("If the icon will be shown around the edges of screen in the scene during playmode when it's not visible in the screen.")]
        public bool showOffScreenIndicator = true;

        [Tooltip("Distance at which the on-screen indicator will start to fade when it approaches camera")]
        public float onScreenIndicatorNearFadeDistance;

        [Tooltip("Minimum distance at which the on-screen indicator disappear")]
        public float onScreenIndicatorNearFadeMin;

        [Tooltip("Sound to play when beacon is shown.")]
        public AudioClip beaconAudioClip;

        [Tooltip("Preserves the state of this POI between scene changes. Note that this POI only will be visible in the scene where it was first created.")]
        public bool dontDestroyOnLoad;

        [Tooltip("Enables heartbeat effect. Plays a sound with variable speed when approaching this POI.")]
        public bool heartbeatEnabled;

        [Tooltip("Sound to play when heartbeat effect is enabled.")]
        public AudioClip heartbeatAudioClip;

        [Tooltip("Distance to start playing heartbeat effect is enabled.")]
        public float heartbeatDistance = 20f;

        [Tooltip("Interval of heartbeat rate based on distance.")]
        public AnimationCurve heartbeatInterval = AnimationCurve.Linear(0, 0.25f, 1f, 3f);

        [Tooltip("POI visibility on the mini-map.")]
        public POIVisibility miniMapVisibility = POIVisibility.WhenInRange;

        [Tooltip("Optionally assign a custom prefab for this POI icon on the mini-map. If null, the system will use the icon prefab set in the Compass Navigator Pro component.")]
        public GameObject miniMapIconPrefabOverride;

        [Tooltip("A value of 0 uses the global visible distance property from the mini-map settings. A value greater than 0 will override the global value for this POI. Useful when you need this POI to use a different visible distance.")]
        public float miniMapVisibleDistanceOverride;

        [Tooltip("If enabled, the icon will stop at the edges of the mini-map even if it's behind the player.")]
        public bool miniMapClampPosition;

        [Tooltip("If enabled, the minimap icon will be rotated according to the POI rotation.")]
        public bool miniMapShowRotation;

        [Tooltip("Custom angle rotation adjustment.")]
        public float miniMapRotationAngleOffset;

        [Tooltip("Icon scale on the mini-map.")]
        public float miniMapIconScale = 1f;

        [Tooltip("Add a circle around the POI in the mini-map illustrating the POI radius")]
        public bool miniMapShowCircle;

        public Color miniMapCircleColor = new Color(0, 1, 0, 0.5f);

        public Color miniMapCircleInnerColor = new Color(0, 0, 1, 0);

        [Range(0, 1)]
        public float miniMapCircleStartRadius = 0.25f;

        [Tooltip("Add a circle animation when the icon appears in the mini-map")]
        public bool miniMapCircleAnimationWhenAppears = true;

        [Tooltip("Number of repetitions for the circle animation")]
        public int miniMapCircleAnimationRepetitions = 5;

        public OnHeartbeatEvent OnHeartbeat;




        #region State variables

        public Scene scene;

        [NonSerialized]
        public float distanceToFollow, prevDistanceToFollow;

        [NonSerialized]
        public Vector3 viewportPos, lastIndicatorViewportPos;

        [NonSerialized]
        public int viewportPosFrameCount;

        [NonSerialized]
        public float visitedTime;

        // Compass bar related
        [NonSerialized] public bool isVisible;
        public RectTransform compassIconRT; // root
        public Image compassIconImage;
        [NonSerialized] public bool curvedMaterialSet; // used for optimization: image.material is very slow
        [NonSerialized]public float compassCurrentIconScale;

        // Mini-map related
        [NonSerialized] public bool miniMapIsVisible;
        public RectTransform miniMapIconRT; // root
        public RectTransform miniMapIconImageRT;
        public Image miniMapIconImage, miniMapCircleImage;
        public Material miniMapCircleMaterial;
        public RectTransform miniMapCircleRT;

        [NonSerialized] public float circleScale = 2f;
        [NonSerialized] public float lastCircleRadius, lastCircleHeight, lastCircleZoomLevel;
        [NonSerialized] public float circleVisibleTime;
        [NonSerialized] public int insideCircle;

        // Indicator related
        [NonSerialized] public int isOnScreen;
        public RectTransform indicatorRT; // root
        public Image indicatorImage;
        public CanvasGroup indicatorCanvasGroup;
        public RectTransform indicatorArrowRT;
        public TextMeshProUGUI indicatorDistanceText, indicatorTitleText;

        [NonSerialized]
        public float prevIndicatorDistance, lastCompassIconDistance;
        [NonSerialized]
        public string lastIndicatorDistanceText, lastCompassIconDistanceText;


        /// <summary>
        /// Reference to the icon gameobject on the compass bar when it's created
        /// </summary>
        public GameObject compassIconGameObject {
            get {
                if (compassIconRT != null) {
                    return compassIconRT.gameObject;
                }
                return null;
            }
        }

        /// <summary>
        /// Reference to the icon gameobject on the minimap when it's created
        /// </summary>
        public GameObject miniMapIconGameObject {
            get {
                if (miniMapIconRT != null) {
                    return miniMapIconRT.gameObject;
                }
                return null;
            }
        }

        /// <summary>
        /// Reference to the indicator gameobject when it's created
        /// </summary>
        public GameObject indicatorGameObject {
            get {
                if (indicatorRT != null) {
                    return indicatorRT.gameObject;
                }
                return null;
            }
        }

        public TextMeshProUGUI compassIconDistanceText;

        [NonReorderable]
        public RectTransform compassIconDistanceTextRT;

        /// <summary>
        /// Reference to the compass script
        /// </summary>
        [NonSerialized]
        public CompassPro compass;

        /// <summary>
        /// Time when the poi appeared on the compass bar
        /// </summary>
        [NonSerialized]
        public float visibleTime;

        [NonSerialized]
        public bool heartbeatIsActive;

        Coroutine heartbeatPlayer;

        [HideInInspector]
        public float miniMapCurrentIconScale;

        #endregion

        private void OnValidate() {
            radius = MathF.Max(0, radius);
            iconScale = Mathf.Max(0, iconScale);
            miniMapCircleAnimationRepetitions = Mathf.Max(1, miniMapCircleAnimationRepetitions);
            onScreenIndicatorNearFadeMin = Mathf.Max(0, onScreenIndicatorNearFadeMin);
            onScreenIndicatorNearFadeDistance = Mathf.Max(0, onScreenIndicatorNearFadeDistance);
        }

        void OnEnable() {
            scene = SceneManager.GetActiveScene();
            if (id == 0) {
                id = Guid.NewGuid().GetHashCode();
                if (iconNonVisited == null) {
                    iconNonVisited = Resources.Load<Sprite>("CNPro/Sprites/compassIcon");
                }
            }
            RegisterPOI();
        }

        void OnDestroy() {
            Release();
            CompassPro compass = CompassPro.instance;
            if (compass != null) {
                compass.POIUnregister(this);
            }
        }

        public void Release() {
            if (compassIconRT != null) {
                DestroyImmediate(compassIconRT.gameObject);
            }
            if (miniMapIconRT != null) {
                DestroyImmediate(miniMapIconRT.gameObject);
            }
            if (indicatorRT != null) {
                DestroyImmediate(indicatorRT.gameObject);
            }
            if (miniMapCircleMaterial != null) {
                DestroyImmediate(miniMapCircleMaterial);
            }
        }

        public void RegisterPOI() {
            CompassPro compass = CompassPro.instance;
            if (compass == null)
                return;

            if (dontDestroyOnLoad && Application.isPlaying) {
                DontDestroyOnLoad(gameObject);
            }

            compass.POIRegister(this);
        }


        public void StartHeartbeat() {
            if (isVisited)
                return;
            heartbeatPlayer = StartCoroutine(HeartBeatPlayer());
            heartbeatIsActive = true;
        }


        public void StopHeartbeat() {
            if (heartbeatPlayer != null) {
                StopCoroutine(heartbeatPlayer);
            }
            heartbeatIsActive = false;
        }


        public void StartCircleAnimation() {
            if (!miniMapCircleAnimationWhenAppears) miniMapCircleAnimationWhenAppears = true;
            circleVisibleTime = 0;
        }

        IEnumerator HeartBeatPlayer() {
            AudioClip heartbeatSound = heartbeatAudioClip != null ? heartbeatAudioClip : CompassPro.instance.heartbeatDefaultAudioClip;
            if (heartbeatSound == null) {
                Debug.LogWarning("Compass POI: heartbeat sound not set.");
                yield break;
            }
            heartbeatDistance = Mathf.Max(1f, heartbeatDistance);
            float minDistance = CompassPro.instance.visitedDistance;
            while (true) {
                float distance = distanceToFollow;
                if (distanceToFollow > heartbeatDistance || isVisited) {
                    heartbeatIsActive = false;
                    yield break;
                }
                if (distance < minDistance) {
                    distance = minDistance;
                }
                Vector3 camPos = CompassPro.instance.cameraMain.transform.position;
                AudioSource.PlayClipAtPoint(heartbeatSound, camPos);
                if (OnHeartbeat != null) {
                    OnHeartbeat();
                }
                float curvePos = (distance - minDistance) / heartbeatDistance;
                float delay = heartbeatInterval.Evaluate(curvePos);
                yield return new WaitForSeconds(delay);
            }
        }

        /// <summary>
        /// Gets the screen rectangle of the icon in the compass bar
        /// </summary>
        /// <returns>The compass bar icon screen rect.</returns>
        public Rect GetCompassIconScreenRect() {
            if (isVisible && compassIconRT != null && compass != null) {
                Vector3 pos = compassIconRT.transform.position;
                Vector3 size = compassIconRT.sizeDelta;
                return new Rect(pos.x - size.x * 0.5f, Screen.height - pos.y - size.y * 0.5f, size.x, size.y);
            }
            return new Rect(0, 0, 0, 0);
        }

        /// <summary>
        /// Gets the screen rectangle of the mini-map icon
        /// </summary>
        /// <returns>The mini map icon screen rect.</returns>
        public Rect GetMiniMapIconScreenRect() {
            if (miniMapIsVisible && miniMapIconRT != null && compass != null) {
                return miniMapIconRT.GetScreenRect();
            }
            return new Rect(0, 0, 0, 0);
        }

        public bool ToggleCompassBarIconVisibility(bool visible) {
            this.isVisible = visible;
            if (compassIconRT == null) return false;
            GameObject compassIconGO = compassIconRT.gameObject;
            bool imageIsVisible = compassIconGO.activeSelf;
            if (imageIsVisible != visible) {
                compassIconGO.SetActive(visible);
                return true;
            }
            return false;
        }

        public bool ToggleMiniMapIconVisibility(bool visible) {
            this.miniMapIsVisible = visible;
            if (miniMapIconImage != null && miniMapIconImage.enabled != visible) {
                miniMapIconImage.enabled = visible;
                return true;
            }
            return false;
        }

        public bool ToggleIndicatorVisibility(bool visible) {
            if (indicatorImage != null && indicatorImage.isActiveAndEnabled != visible) {
                indicatorImage.gameObject.SetActive(visible);
                return true;
            }
            return false;
        }

        public bool ToggleMiniMapCircleVisibility(bool visible) {
            if (miniMapCircleImage != null && miniMapCircleImage.isActiveAndEnabled != visible) {
                miniMapCircleRT.gameObject.SetActive(visible);
                return true;
            }
            return false;
        }

    }

}