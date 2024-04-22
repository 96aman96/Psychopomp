using System;
using UnityEngine;

namespace CompassNavigatorPro {

    public enum MiniMapStyle {
        TornPaper = 0,
        SolidBox = 1,
        SolidCircle = 2,
        Fantasy1 = 10,
        Fantasy2 = 11,
        Fantasy3 = 12,
        Fantasy4 = 13,
        Fantasy5 = 14,
        Fantasy6 = 15,
        SciFi1 = 20,
        SciFi2 = 21,
        SciFi3 = 22,
        Custom = 100,
        None = 200
    }

    public enum MiniMapContents {
        TopDownWorldView = 0,
        WorldMappedTexture = 1,
        UITexture = 2,
        Radar = 3
    }

    public enum MiniMapOrientation {
        Camera,
        Follow
    }

    public enum MiniMapResolution {
        [InspectorName("64")] _64 = 64,
        [InspectorName("128")] _128 = 128,
        [InspectorName("256")] _256 = 256,
        [InspectorName("512")] _512 = 512,
        [InspectorName("1024")] _1024 = 1024,
        [InspectorName("2048")] _2048 = 2048,
        [InspectorName("4096")] _4096 = 4096,
        [InspectorName("8192")] _8192 = 8192
    }

    public enum MiniMapPositionAndScaleMode {
        ControlledByCompassNavigatorPro,
        UserDefined
    }

    public enum MiniMapPosition {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum MiniMapCameraMode {
        Perspective = 0,
        Orthographic = 1
    }

    public enum MiniMapCameraSnapshotFrequency {
        Continuous = 0,
        TimeInterval = 1,
        DistanceTravelled = 2
    }

    public enum MiniMapViewConeFovSource {
        FromCamera,
        UserDefined
    }

    public enum MiniMapPulsePreset {
        Default,
        LongSweep,
        Scanning,
        Custom
    }

    public enum MiniMapRadarInfoType {
        Nothing,
        RingIntervalDistance,
        RadarRange
    }

    public partial class CompassPro : MonoBehaviour {

        #region Public MiniMap properties

        [Tooltip("Shows the minimap")]
        [SerializeField]
        bool _showMiniMap;

        /// <summary>
        /// Show/Hide minimap 
        /// </summary>
        public bool showMiniMap {
            get { return _showMiniMap; }
            set {
                if (value != _showMiniMap) {
                    miniMapFullScreenState = false;
                    _showMiniMap = value;
                    SetupMiniMap();
                    UpdateCompassBarAlpha();
                }
            }
        }


        [SerializeField]
        MiniMapPositionAndScaleMode _miniMapPositionAndSize = MiniMapPositionAndScaleMode.ControlledByCompassNavigatorPro;

        /// <summary>
        /// Minimap position and size
        /// </summary>
        public MiniMapPositionAndScaleMode miniMapPositionAndSize {
            get { return _miniMapPositionAndSize; }
            set {
                if (value != _miniMapPositionAndSize) {
                    _miniMapPositionAndSize = value;
                    SetupMiniMap();
                }
            }
        }

        [SerializeField]
        MiniMapPosition _miniMapLocation = MiniMapPosition.BottomRight;

        /// <summary>
        /// Minimap screen location
        /// </summary>
        public MiniMapPosition miniMapLocation {
            get { return _miniMapLocation; }
            set {
                if (value != _miniMapLocation) {
                    _miniMapLocation = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("POIs beyond visible distance (meters) will not be shown in the compass bar")]
        [SerializeField]
        float _miniMapVisibleMaxDistance = 10000f;

        /// <summary>
        /// Gets or sets the maximum distance to a POI so it's visible in the mini-map.
        /// </summary>
        public float miniMapVisibleMaxDistance {
            get { return _miniMapVisibleMaxDistance; }
            set {
                if (value != _miniMapVisibleMaxDistance) {
                    _miniMapVisibleMaxDistance = value;
                }
            }
        }

        [SerializeField]
        Vector2 _miniMapScreenPositionOffset = new Vector2(-5, 5);

        /// <summary>
        /// Minimap screen location offset
        /// </summary>
        public Vector2 miniMapLocationOffset {
            get { return _miniMapScreenPositionOffset; }
            set {
                if (value != _miniMapScreenPositionOffset) {
                    _miniMapScreenPositionOffset = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("Keeps the mini-map oriented to North")]
        [SerializeField]
        bool _miniMapKeepStraight;

        /// <summary>
        /// Keep the mini-map oriented to North
        /// </summary>
        public bool miniMapKeepStraight {
            get { return _miniMapKeepStraight; }
            set {
                if (value != _miniMapKeepStraight) {
                    _miniMapKeepStraight = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("Orientation of the mini-map")]
        [SerializeField]
        MiniMapOrientation _miniMapOrientation;

        /// <summary>
        /// Orientation of the mini-map
        /// </summary>
        public MiniMapOrientation miniMapOrientation {
            get { return _miniMapOrientation; }
            set {
                if (value != _miniMapOrientation) {
                    _miniMapOrientation = value;
                    UpdateMiniMapContents();
                }
            }
        }

        [Tooltip("Rotation of the mini-map camera")]
        [SerializeField]
        [Range(0, 90)]
        float _miniMapCameraTilt;

        /// <summary>
        /// Mini-map camera rotation. By default it looks straight down.
        /// </summary>
        public float miniMapCameraTilt {
            get { return _miniMapCameraTilt; }
            set {
                if (value != _miniMapCameraTilt) {
                    _miniMapCameraTilt = value;
                    UpdateMiniMapContents();
                }
            }
        }

        [Tooltip("Screen size of mini-map in % of screen height")]
        [SerializeField]
        float _miniMapSize = 0.35f;

        /// <summary>
        /// The screen size of the mini-map
        /// </summary>
        public float miniMapSize {
            get { return _miniMapSize; }
            set {
                if (value != _miniMapSize) {
                    _miniMapSize = Mathf.Max(value, 0.001f);
                    SetupMiniMap();
                }
            }
        }

        Vector3 _miniMapFollowOffset; // not serialized

        /// <summary>
        /// Offset added to the follow position
        /// </summary>
        public Vector3 miniMapFollowOffset {
            get { return _miniMapFollowOffset; }
            set { if (_miniMapFollowOffset != value) {
                    _miniMapFollowOffset = value;
                    ClampDragOffset();
                    needUpdateMiniMapIcons = true;
                }
            }
        }


        [Tooltip("Mask for the border of the mini map")]
        [SerializeField]
        Sprite _miniMapMaskSprite;

        /// <summary>
        /// The sprite for the mini-map mask
        /// </summary>
        public Sprite miniMapMaskSprite {
            get { return _miniMapMaskSprite; }
            set {
                if (value != _miniMapMaskSprite) {
                    _miniMapMaskSprite = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Texture for the border of the mini map")]
        [SerializeField]
        Texture2D _miniMapBorderTexture;

        /// <summary>
        /// Show/Hide minimap 
        /// </summary>
        public Texture2D miniMapBorderTexture {
            get { return _miniMapBorderTexture; }
            set {
                if (value != _miniMapBorderTexture) {
                    _miniMapBorderTexture = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Mini-map style")]
        [SerializeField]
        MiniMapStyle _miniMapStyle = MiniMapStyle.TornPaper;

        /// <summary>
        /// Style of mini-map
        /// </summary>
        public MiniMapStyle miniMapStyle {
            get { return _miniMapStyle; }
            set {
                if (value != _miniMapStyle) {
                    _miniMapStyle = value;
                    SetupMiniMap();
                }
            }
        }


        [SerializeField]
        Color _miniMapBackgroundColor = Color.black;

        /// <summary>
        /// Background color for the mini-map when there's nothing rendered
        /// </summary>
        public Color miniMapBackgroundColor {
            get { return _miniMapBackgroundColor; }
            set {
                if (value != _miniMapBackgroundColor) {
                    _miniMapBackgroundColor = value;
                    SetupMiniMap();
                }
            }
        }


        [SerializeField]
        bool _miniMapBackgroundOpaque;

        /// <summary>
        /// Enable to force opaque background for mini-map
        /// </summary>
        public bool miniMapBackgroundOpaque {
            get { return _miniMapBackgroundOpaque; }
            set {
                if (value != _miniMapBackgroundOpaque) {
                    _miniMapBackgroundOpaque = value;
                    UpdateMiniMap();
                }
            }
        }



        [Tooltip("What to show when minimap is non full screen mode")]
        [SerializeField]
        MiniMapContents _miniMapContents = MiniMapContents.TopDownWorldView;

        /// <summary>
        /// Contents for the mini-map
        /// </summary>
        public MiniMapContents miniMapContents {
            get { return _miniMapContents; }
            set {
                if (value != _miniMapContents) {
                    _miniMapContents = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("The texture to be used as background for the minimap in non full screen mode")]
        [SerializeField]
        Texture _miniMapContentsTexture;

        /// <summary>
        /// Texture for the mini-map
        /// </summary>
        public Texture miniMapContentsTexture {
            get { return _miniMapContentsTexture; }
            set {
                if (value != _miniMapContentsTexture) {
                    _miniMapContentsTexture = value;
                    UpdateMiniMap();
                }
            }
        }


        [Tooltip("Allows rotation of the UI texture")]
        [SerializeField]
        bool _miniMapContentsTextureAllowRotation;

        /// <summary>
        /// Keep the mini-map oriented to North
        /// </summary>
        public bool miniMapContentsTextureAllowRotation {
            get { return _miniMapContentsTextureAllowRotation; }
            set {
                if (value != _miniMapContentsTextureAllowRotation) {
                    _miniMapContentsTextureAllowRotation = value;
                    UpdateMiniMap();
                }
            }
        }


        [Tooltip("Mask for the border of the mini map in full screen mode")]
        [SerializeField]
        Sprite _miniMapMaskSpriteFullScreenMode;

        /// <summary>
        /// The sprite for the mini-map mask in full-screen mode
        /// </summary>
        public Sprite miniMapMaskSpriteFullScreenMode {
            get { return _miniMapMaskSpriteFullScreenMode; }
            set {
                if (value != _miniMapMaskSpriteFullScreenMode) {
                    _miniMapMaskSpriteFullScreenMode = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("Texture for the border of the mini map in full screen mode")]
        [SerializeField]
        Texture2D _miniMapBorderTextureFullScreenMode;

        /// <summary>
        /// Optional mini-map border texture when in full-screen mode
        /// </summary>
        public Texture2D miniMapBorderTextureFullScreenMode {
            get { return _miniMapBorderTextureFullScreenMode; }
            set {
                if (value != _miniMapBorderTextureFullScreenMode) {
                    _miniMapBorderTextureFullScreenMode = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Mini-map style in full screen mode")]
        [SerializeField]
        MiniMapStyle _miniMapFullScreenStyle = MiniMapStyle.SolidBox;

        /// <summary>
        /// Style for the mini-map when in full-screen mode
        /// </summary>
        public MiniMapStyle miniMapFullScreenStyle {
            get { return _miniMapFullScreenStyle; }
            set {
                if (value != _miniMapFullScreenStyle) {
                    _miniMapFullScreenStyle = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Size of the mini-map render texture in non-full screen mode")]
        [SerializeField]
        MiniMapResolution _miniMapResolution = MiniMapResolution._512;

        /// <summary>
        /// The capture resolution when minimap is in non full-screen mode.
        /// </summary>
        public MiniMapResolution miniMapResolution {
            get { return _miniMapResolution; }
            set {
                if (value != _miniMapResolution) {
                    _miniMapResolution = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Size of the render texture in full screen mode")]
        [SerializeField]
        MiniMapResolution _miniMapFullScreenResolution = MiniMapResolution._1024;

        /// <summary>
        /// The capture resolution when minimap is in full-screen mode.
        /// </summary>
        public MiniMapResolution miniMapFullScreenResolution {
            get { return _miniMapFullScreenResolution; }
            set {
                if (value != _miniMapFullScreenResolution) {
                    _miniMapFullScreenResolution = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("The zoom level used for the mini-map in full screen mode")]
        [SerializeField, Range(0, 1f)]
        float _miniMapFullScreenZoomLevel = 1f;

        /// <summary>
        /// The current mini-map zoom used in full screen mode
        /// </summary>
        public float miniMapFullScreenZoomLevel {
            get { return _miniMapFullScreenZoomLevel; }
            set {
                value = Mathf.Clamp(value, _miniMapZoomMin, _miniMapZoomMax);
                if (value != _miniMapFullScreenZoomLevel) {
                    _miniMapFullScreenZoomLevel = value;
                    lastViewConeCameraAspect = 0;
                    UpdateMiniMapContents();
                }
            }
        }

        [Tooltip("Optional UI element which serves as placeholder for exact positioning of the mini-map in fullscreen mode")]
        [SerializeField]
        RectTransform _miniMapFullScreenPlaceholder;

        /// <summary>
        /// Optionally assign an UI rect transform which will be used to render the mini-map in full-screen mode
        /// </summary>
        public RectTransform miniMapFullScreenPlaceholder {
            get { return _miniMapFullScreenPlaceholder; }
            set {
                if (value != _miniMapFullScreenPlaceholder) {
                    _miniMapFullScreenPlaceholder = value;
                    if (_miniMapFullScreenState) {
                        MiniMapZoomToggle(false);
                    } else {
                        SetupMiniMap();
                    }
                }
            }
        }


        [Tooltip("The distance of clamped icons to the edge of the mini-map in full screen mode")]
        [SerializeField]
        float _miniMapFullScreenClampBorder = 0.02f;

        /// <summary>
        /// The distance to the edge for the clamped icons on the minimap
        /// </summary>
        public float miniMapFullScreenClampBorder {
            get { return _miniMapFullScreenClampBorder; }
            set {
                if (value != _miniMapFullScreenClampBorder) {
                    _miniMapFullScreenClampBorder = value;
                    needUpdateMiniMapIcons = true;
                }
            }
        }

        [Tooltip("Enable this option if the minimap uses a circular shape in full screen mode")]
        [SerializeField]
        bool _miniMapFullScreenClampBorderCircular;

        /// <summary>
        /// Enable if the mini-map uses a circular shape
        /// </summary>
        public bool miniMapFullScreenClampBorderCircular {
            get { return _miniMapFullScreenClampBorderCircular; }
            set {
                if (value != _miniMapFullScreenClampBorderCircular) {
                    _miniMapFullScreenClampBorderCircular = value;
                    needUpdateMiniMapIcons = true;
                }
            }
        }


        [Tooltip("Percentage of screen size if full-screen mode. Image resolution will increase according to screen resolution")]
        [Range(0.5f, 1f)]
        [SerializeField]
        float _miniMapFullScreenSize = 0.9f;

        /// <summary>
        /// The percentage of screen size when minimap is in full screen mode.
        /// </summary>
        public float miniMapFullScreenSize {
            get { return _miniMapFullScreenSize; }
            set {
                if (value != _miniMapFullScreenSize) {
                    _miniMapFullScreenSize = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("What to show when minimap is in full screen mode")]
        [SerializeField]
        MiniMapContents _miniMapFullScreenContents = MiniMapContents.TopDownWorldView;

        /// <summary>
        /// Contents for the mini-map when in full screen mode
        /// </summary>
        public MiniMapContents miniMapFullScreenContents {
            get { return _miniMapFullScreenContents; }
            set {
                if (value != _miniMapFullScreenContents) {
                    _miniMapFullScreenContents = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("The texture to be used as background for the minimap in full screen mode.")]
        [SerializeField]
        Texture _miniMapFullScreenContentsTexture;

        /// <summary>
        /// Texture for the mini-map when in full screen mode
        /// </summary>
        public Texture miniMapFullScreenContentsTexture {
            get { return _miniMapFullScreenContentsTexture; }
            set {
                if (value != _miniMapFullScreenContentsTexture) {
                    _miniMapFullScreenContentsTexture = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("Keep aspect ration in full screen mode")]
        [SerializeField]
        bool _miniMapKeepAspectRatio = true;

        /// <summary>
        /// Keep aspect ration in full screen mode
        /// </summary>
        public bool miniMapKeepAspectRatio {
            get { return _miniMapKeepAspectRatio; }
            set {
                if (value != _miniMapKeepAspectRatio) {
                    _miniMapKeepAspectRatio = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Allow user to drag the map around")]
        [SerializeField]
        bool _miniMapAllowUserDrag;

        /// <summary>
        /// Allow user click and drag the minimap
        /// </summary>
        public bool miniMapAllowUserDrag {
            get { return _miniMapAllowUserDrag; }
            set {
                if (value != _miniMapAllowUserDrag) {
                    _miniMapAllowUserDrag = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("Allow user to drag the map around in full screen mode")]
        [SerializeField]
        bool _miniMapFullScreenAllowUserDrag;

        /// <summary>
        /// Allow user click and drag the minimap
        /// </summary>
        public bool miniMapFullScreenAllowUserDrag {
            get { return _miniMapFullScreenAllowUserDrag; }
            set {
                if (value != _miniMapFullScreenAllowUserDrag) {
                    _miniMapFullScreenAllowUserDrag = value;
                    SetupMiniMap();
                }
            }
        }

        [SerializeField]
        [Tooltip("Maximum allowed drag distance")]
        float _miniMapDragMaxDistance = 1000f;

        /// <summary>
        /// Maximum allowed drag
        /// </summary>
        public float miniMapDragMaxDistance {
            get { return _miniMapDragMaxDistance; }
            set {
                if (value != _miniMapDragMaxDistance) {
                    _miniMapDragMaxDistance = value;
                    ClampDragOffset();
                }
            }
        }
        

        [Tooltip("Orthographic or perspective mode for the mini-map camera")]
        [SerializeField] MiniMapCameraMode _miniMapCameraMode = MiniMapCameraMode.Orthographic;

        /// <summary>
        /// Mini-map projection mode
        /// </summary>
        public MiniMapCameraMode miniMapCameraMode {
            get { return _miniMapCameraMode; }
            set {
                if (value != _miniMapCameraMode) {
                    _miniMapCameraMode = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Frequency of camera capture")]
        [SerializeField]
        MiniMapCameraSnapshotFrequency _miniMapCameraSnapshotFrequency = MiniMapCameraSnapshotFrequency.DistanceTravelled;

        /// <summary>
        /// How often the mini-map camera will capture the scene
        /// </summary>
        public MiniMapCameraSnapshotFrequency miniMapCameraSnapshotFrequency {
            get { return _miniMapCameraSnapshotFrequency; }
            set {
                if (value != _miniMapCameraSnapshotFrequency) {
                    _miniMapCameraSnapshotFrequency = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("The orthographic size of the mini-map camera")]
        [SerializeField]
        float _miniMapCaptureSize = 256f;

        /// <summary>
        /// The orthographic camera size
        /// </summary>
        public float miniMapCaptureSize {
            get { return _miniMapCaptureSize; }
            set {
                if (value != _miniMapCaptureSize) {
                    _miniMapCaptureSize = value;
                    UpdateMiniMapContents();
                }
            }
        }


        [SerializeField]
        float _miniMapSnapshotInterval = 10f;

        /// <summary>
        /// The time interval between minimap camera shots
        /// </summary>
        public float miniMapSnapshotInterval {
            get { return _miniMapSnapshotInterval; }
            set {
                if (value != _miniMapSnapshotInterval) {
                    _miniMapSnapshotInterval = value;
                }
            }
        }

        [Tooltip("Distance in meters")]
        [SerializeField]
        float _miniMapSnapshotDistance = 10f;

        /// <summary>
        /// The distance interval between minimap camera shots
        /// </summary>
        public float miniMapSnapshotDistance {
            get { return _miniMapSnapshotDistance; }
            set {
                if (value != _miniMapSnapshotDistance) {
                    _miniMapSnapshotDistance = value;
                }
            }
        }

        [Tooltip("Contrast of the mini-map image")]
        [Range(0f, 2f)]
        [SerializeField]
        float _miniMapContrast = 1.02f;

        public float miniMapContrast {
            get { return _miniMapContrast; }
            set {
                if (value != _miniMapContrast) {
                    _miniMapContrast = value;
                }
            }
        }

        [Tooltip("Brightness of the mini-map image")]
        [Range(0, 2f)]
        [SerializeField]
        float _miniMapBrightness = 1.05f;

        public float miniMapBrightness {
            get { return _miniMapBrightness; }
            set {
                if (value != _miniMapBrightness) {
                    _miniMapBrightness = value;
                }
            }
        }

        [Tooltip("Tint color for the mini-map. Alpha controls the intensity.")]
        [SerializeField]
        Color _miniMapTintColor = new Color(1, 1, 1, 0);

        public Color miniMapTintColor {
            get { return _miniMapTintColor; }
            set {
                if (value != _miniMapTintColor) {
                    _miniMapTintColor = value;
                }
            }
        }


        [Tooltip("Enable to render shadows in mini-map")]
        [SerializeField]
        bool _miniMapEnableShadows;

        /// <summary>
        /// Enables/disables shadow casting when rendering mini-map
        /// </summary>
        public bool miniMapEnableShadows {
            get { return _miniMapEnableShadows; }
            set {
                if (value != _miniMapEnableShadows) {
                    _miniMapEnableShadows = value;
                    SetupMiniMap();
                }
            }
        }


        [SerializeField, Range(0, 1)]
        float _miniMapZoomMin = 0.01f;

        /// <summary>
        /// The orthographic minimum size for the camera
        /// </summary>
        public float miniMapZoomMin {
            get { return _miniMapZoomMin; }
            set {
                if (value != _miniMapZoomMin) {
                    _miniMapZoomMin = value;
                    UpdateMiniMapContents();
                }
            }
        }

        [SerializeField, Range(0, 1)]
        float _miniMapZoomMax = 1f;

        /// <summary>
        /// The orthographic maximum size for the camera
        /// </summary>
        public float miniMapZoomMax {
            get { return _miniMapZoomMax; }
            set {
                if (value != _miniMapZoomMax) {
                    _miniMapZoomMax = value;
                    UpdateMiniMapContents();
                }
            }
        }


        [SerializeField]
        GameObject _miniMapIconPrefab;

        /// <summary>
        /// Mini-map icon prefab
        /// </summary>
        public GameObject miniMapIconPrefab {
            get { return _miniMapIconPrefab; }
            set {
                if (value != _miniMapIconPrefab) {
                    _miniMapIconPrefab = value;
                }
            }
        }

        [Tooltip("Optional displacement for the icons in the mini-map")]
        [SerializeField]
        Vector2 _miniMapIconPositionShift;

        /// <summary>
        /// Optional shift for mini-map icons
        /// </summary>
        public Vector2 miniMapIconPositionShift {
            get { return _miniMapIconPositionShift; }
            set {
                if (value != _miniMapIconPositionShift) {
                    _miniMapIconPositionShift = value;
                    _miniMapIconPositionShift.x = Mathf.Clamp(_miniMapIconPositionShift.x, -1f, 1f);
                    _miniMapIconPositionShift.y = Mathf.Clamp(_miniMapIconPositionShift.y, -1f, 1f);
                    UpdateMiniMapContents();
                }
            }
        }


        [Tooltip("The current zoom for the mini-map based on the minimum / maximum ranges")]
        [SerializeField, Range(0, 1f)]
        float _miniMapZoomLevel = 0.5f;

        /// <summary>
        /// The current mini-map zoom based on the min/max size (orthographic mode) or altitude (perspective mode)
        /// </summary>
        public float miniMapZoomLevel {
            get { return _miniMapZoomLevel; }
            set {
                float clampedValue = Mathf.Clamp(value, _miniMapZoomMin, _miniMapZoomMax);
                if (clampedValue != _miniMapZoomLevel) {
                    _miniMapZoomLevel = clampedValue;
                    UpdateMiniMapContents();
                }
            }
        }

        [Tooltip("The minimum altitude of the mini-map camera respect with the follow target")]
        [SerializeField]
        float _miniMapCameraMinAltitude = 10;

        /// <summary>
        /// The min distance from the camera to the following target
        /// </summary>
        public float miniMapCameraMinAltitude {
            get { return _miniMapCameraMinAltitude; }
            set {
                if (value != _miniMapCameraMinAltitude) {
                    _miniMapCameraMinAltitude = value;
                    UpdateMiniMapContents();
                }
            }
        }

        [Tooltip("The maximum altitude of the mini-map camera respect with the follow target")]
        [SerializeField]
        float _miniMapCameraMaxAltitude = 100f;

        /// <summary>
        /// The max distance from the camera to the following target
        /// </summary>
        public float miniMapCameraMaxAltitude {
            get { return _miniMapCameraMaxAltitude; }
            set {
                value = Mathf.Max(_miniMapCameraMinAltitude, value);
                if (value != _miniMapCameraMaxAltitude) {
                    _miniMapCameraMaxAltitude = value;
                    UpdateMiniMapContents();
                }
            }
        }


        [Tooltip("The altitude of the mini-map camera relative to the main camera or followed gameobject")]
        [SerializeField]
        float _miniMapCameraHeightVSFollow = 200f;

        /// <summary>
        /// When mini-map is in orthographic projection, an optional height for the camera with respect to the main camera or followed item
        /// </summary>
        public float miniMapCameraHeightVSFollow {
            get { return _miniMapCameraHeightVSFollow; }
            set {
                if (value != _miniMapCameraHeightVSFollow) {
                    _miniMapCameraHeightVSFollow = value;
                    UpdateMiniMapContents();
                }
            }
        }


        [Tooltip("How far will capture the mini-map camera from the top-down position (this is the far clip plane of the mini-map camera).")]
        [SerializeField]
        float _miniMapCameraDepth = 1000f;

        /// <summary>
        /// How far will capture the mini-map camera from the top-down position (this is the far clip plane of the mini-map camera)
        /// </summary>
        public float miniMapCameraDepth {
            get { return _miniMapCameraDepth; }
            set {
                if (value != _miniMapCameraDepth) {
                    _miniMapCameraDepth = value;
                    SetupMiniMap();
                }
            }
        }


        [Tooltip("Which objects will be visible in the mini-map")]
        [SerializeField] LayerMask _miniMapLayerMask = -1;

        /// <summary>
        /// The layer mask for the mini-map camera
        /// </summary>
        public LayerMask miniMapLayerMask {
            get { return _miniMapLayerMask; }
            set {
                if (value != _miniMapLayerMask) {
                    _miniMapLayerMask = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("The size for the icons on the mini-map")]
        [SerializeField]
        float _miniMapIconSize = 0.5f;

        /// <summary>
        /// The size for the icons on the mini-map
        /// </summary>
        public float miniMapIconSize {
            get { return _miniMapIconSize; }
            set {
                if (value != _miniMapIconSize) {
                    _miniMapIconSize = value;
                }
            }
        }

        [Tooltip("Enables player/compass icon on the mini-map")]
        [SerializeField]
        bool _miniMapShowPlayerIcon = true;

        /// <summary>
        /// Enables player/compass icon on the mini-map
        /// </summary>
        public bool miniMapShowPlayerIcon {
            get { return _miniMapShowPlayerIcon; }
            set {
                if (value != _miniMapShowPlayerIcon) {
                    _miniMapShowPlayerIcon = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("The size for the player icon on the mini-map")]
        [SerializeField]
        float _miniMapPlayerIconSize = 1f;

        /// <summary>
        /// Scale multiplier for player's icon
        /// </summary>
        public float miniMapPlayerIconSize {
            get { return _miniMapPlayerIconSize; }
            set {
                if (value != _miniMapPlayerIconSize) {
                    _miniMapPlayerIconSize = value;
                }
            }
        }

        [Tooltip("The sprite for the player icon")]
        [SerializeField]
        Sprite _miniMapPlayerIconSprite;

        /// <summary>
        /// Sets the player icon on the minimap
        /// </summary>
        public Sprite miniMapPlayerIconSprite {
            get { return _miniMapPlayerIconSprite; }
            set {
                if (value != _miniMapPlayerIconSprite) {
                    _miniMapPlayerIconSprite = value;
                }
            }
        }

        [Tooltip("The color for the player icon")]
        [SerializeField]
        Color _miniMapPlayerIconColor = Color.white;

        /// <summary>
        /// Sets the player icon color
        /// </summary>
        public Color miniMapPlayerIconColor {
            get { return _miniMapPlayerIconColor; }
            set {
                if (value != _miniMapPlayerIconColor) {
                    _miniMapPlayerIconColor = value;
                }
            }
        }

        [Tooltip("Enables North icon on the mini-map")]
        [SerializeField]
        bool _miniMapShowCardinals = true;

        /// <summary>
        /// Enables North icon on the mini-map
        /// </summary>
        public bool miniMapShowNorth {
            get { return _miniMapShowCardinals; }
            set {
                if (value != _miniMapShowCardinals) {
                    _miniMapShowCardinals = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("The size for the North icon on the mini-map")]
        [SerializeField]
        float _miniMapCardinalsSize = 1f;

        /// <summary>
        /// Scale multiplier for the North icon
        /// </summary>
        public float miniMapCardinalsSize {
            get { return _miniMapCardinalsSize; }
            set {
                if (value != _miniMapCardinalsSize) {
                    _miniMapCardinalsSize = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("The sprite for the mini-map cardinals")]
        [SerializeField]
        Sprite _miniMapCardinalsSprite;

        /// <summary>
        /// Sets the player icon on the minimap
        /// </summary>
        public Sprite miniMapCardinalsSprite {
            get { return _miniMapCardinalsSprite; }
            set {
                if (value != _miniMapCardinalsSprite) {
                    _miniMapCardinalsSprite = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("The tint color for the mini-map cardinals")]
        [SerializeField]
        Color _miniMapCardinalsColor = Color.white;

        /// <summary>
        /// Sets the player icon color
        /// </summary>
        public Color miniMapCardinalsColor {
            get { return _miniMapCardinalsColor; }
            set {
                if (value != _miniMapCardinalsColor) {
                    _miniMapCardinalsColor = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Enables view cone on the mini-map")]
        [SerializeField]
        bool _miniMapShowViewCone = true;

        /// <summary>
        /// Enables view cone on the mini-map
        /// </summary>
        public bool miniMapShowViewCone {
            get { return _miniMapShowViewCone; }
            set {
                if (value != _miniMapShowViewCone) {
                    _miniMapShowViewCone = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Color of the view cone")]
        [SerializeField]
        Color _miniMapViewConeColor = new Color(1, 1, 1, 0.25f);

        /// <summary>
        /// Color of the view cone
        /// </summary>
        public Color miniMapViewConeColor {
            get { return _miniMapViewConeColor; }
            set {
                if (value != _miniMapViewConeColor) {
                    _miniMapViewConeColor = value;
                    SetupMiniMap();
                }
            }
        }

        [SerializeField]
        MiniMapViewConeFovSource _miniMapViewConeFoVSource = MiniMapViewConeFovSource.FromCamera;

        public MiniMapViewConeFovSource miniMapViewConeFoVSource {
            get { return _miniMapViewConeFoVSource; }
            set {
                if (value != _miniMapViewConeFoVSource) {
                    _miniMapViewConeFoVSource = value;
                    SetupMiniMap();
                }
            }
        }

        [SerializeField]
        [Range(0, 360)]
        float _miniMapViewConeFoV = 60;

        public float miniMapViewConeFoV {
            get { return _miniMapViewConeFoV; }
            set {
                if (value != _miniMapViewConeFoV) {
                    _miniMapViewConeFoV = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Distance of the view cone")]
        [SerializeField]
        float _miniMapViewConeDistance = 150f;

        /// <summary>
        /// Color of the view cone
        /// </summary>
        public float miniMapViewConeDistance {
            get { return _miniMapViewConeDistance; }
            set {
                if (value != _miniMapViewConeDistance) {
                    _miniMapViewConeDistance = value;
                    UpdateMiniMap();
                }
            }
        }


        [Tooltip("Fall-off/gradient for the view cone effect")]
        [SerializeField]
        [Range(0.0001f, 1)]
        float _miniMapViewConeFallOff = 0.75f;

        /// <summary>
        /// Fall-off/gradient for the view cone effect
        /// </summary>
        public float miniMapViewConeFallOff {
            get { return _miniMapViewConeFallOff; }
            set {
                if (value != _miniMapViewConeFallOff) {
                    _miniMapViewConeFallOff = value;
                    UpdateMiniMap();
                }
            }
        }


        [Tooltip("Enables view cone outline")]
        [SerializeField]
        bool _miniMapShowViewConeOutline;

        /// <summary>
        /// Enables view cone outline
        /// </summary>
        public bool miniMapShowViewConeOutline {
            get { return _miniMapShowViewConeOutline; }
            set {
                if (value != _miniMapShowViewConeOutline) {
                    _miniMapShowViewConeOutline = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Color of the view cone outline")]
        [SerializeField]
        Color _miniMapViewConeOutlineColor = new Color(1, 1, 1, 0.5f);

        /// <summary>
        /// Color of the view cone outline
        /// </summary>
        public Color miniMapViewConeOutlineColor {
            get { return _miniMapViewConeOutlineColor; }
            set {
                if (value != _miniMapViewConeOutlineColor) {
                    _miniMapViewConeOutlineColor = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("The distance of clamped icons to the edge of the mini-map")]
        [SerializeField]
        float _miniMapClampBorder = 0.02f;

        /// <summary>
        /// The distance to the edge for the clamped icons on the minimap
        /// </summary>
        public float miniMapClampBorder {
            get { return _miniMapClampBorder; }
            set {
                if (value != _miniMapClampBorder) {
                    _miniMapClampBorder = value;
                    needUpdateMiniMapIcons = true;
                }
            }
        }

        [Tooltip("Enable this option if the minimap uses a circular shape")]
        [SerializeField]
        bool _miniMapClampBorderCircular;

        /// <summary>
        /// Enable if the mini-map uses a circular shape
        /// </summary>
        public bool miniMapClampBorderCircular {
            get { return _miniMapClampBorderCircular; }
            set {
                if (value != _miniMapClampBorderCircular) {
                    _miniMapClampBorderCircular = value;
                    needUpdateMiniMapIcons = true;
                }
            }
        }


        [Tooltip("Enable this option if the minimap uses a circular shape and want to darken the inner border.")]
        [SerializeField]
        bool _miniMapVignette;

        /// <summary>
        /// Enables vignette on circular mini-map
        /// </summary>
        public bool miniMapVignette {
            get { return _miniMapVignette; }
            set {
                if (value != _miniMapVignette) {
                    _miniMapVignette = value;
                    needUpdateMiniMapIcons = true;
                }
            }
        }

        [Tooltip("Transparency of the mini-map")]
        [Range(0, 1)]
        [SerializeField]
        float _miniMapAlpha = 1.0f;

        /// <summary>
        /// The alpha (transparency) of the mini-map.
        /// </summary>
        public float miniMapAlpha {
            get { return _miniMapAlpha; }
            set {
                if (value != _miniMapAlpha) {
                    _miniMapAlpha = value;
                }
            }
        }

        [Tooltip("Show the zoom in/out minimap button")]
        [SerializeField]
        bool _miniMapShowZoomInOutButtons;

        public bool miniMapShowZoomInOutButtons {
            get { return _miniMapShowZoomInOutButtons; }
            set {
                if (value != _miniMapShowZoomInOutButtons) {
                    _miniMapShowZoomInOutButtons = value;
                    SetupMiniMap();
                }
            }
        }


        [SerializeField]
        [Range(0.001f, 10)]
        float _miniMapIconCircleAnimationDuration = 1f;

        public float miniMapIconCircleAnimationDuration {
            get { return _miniMapIconCircleAnimationDuration; }
            set {
                if (value != _miniMapIconCircleAnimationDuration) {
                    _miniMapIconCircleAnimationDuration = value;
                }
            }
        }

        [Tooltip("Show the maximize minimap button.")]
        [SerializeField]
        bool _miniMapShowMaximizeButton;

        public bool miniMapShowMaximizeButton {
            get { return _miniMapShowMaximizeButton; }
            set {
                if (value != _miniMapShowMaximizeButton) {
                    _miniMapShowMaximizeButton = value;
                    SetupMiniMap();
                }
            }
        }

        [Range(0.01f, 2f)]
        [SerializeField]
        float _miniMapButtonsScale = 1.0f;

        /// <summary>
        /// The size of the buttons of the mini-map
        /// </summary>
        public float miniMapButtonsScale {
            get { return _miniMapButtonsScale; }
            set {
                if (value != _miniMapButtonsScale) {
                    _miniMapButtonsScale = value;
                    SetupMiniMap();
                }
            }
        }

        [Tooltip("Raise pointer click, down, up, enter and exit events on icons")]
        [SerializeField]
        bool _miniMapIconEvents;

        public bool miniMapIconEvents {
            get { return _miniMapIconEvents; }
            set {
                if (_miniMapIconEvents != value) {
                    _miniMapIconEvents = value;
                }
            }
        }


        [Tooltip("Displays the distance to each ring")]
        [SerializeField]
        MiniMapRadarInfoType _miniMapRadarInfoDisplay = MiniMapRadarInfoType.RingIntervalDistance;

        public MiniMapRadarInfoType miniMapRadarInfoDisplay {
            get { return _miniMapRadarInfoDisplay; }
            set {
                if (_miniMapRadarInfoDisplay != value) {
                    _miniMapRadarInfoDisplay = value;
                }
            }
        }

        [SerializeField]
        Color _miniMapRadarRingsColor = Color.white;

        /// <summary>
        /// The color of the rings in radar mode
        /// </summary>
        public Color miniMapRadarRingsColor {
            get { return _miniMapRadarRingsColor; }
            set {
                if (value != _miniMapRadarRingsColor) {
                    _miniMapRadarRingsColor = value;
                    SetupMiniMap();
                }
            }
        }


        [SerializeField]
        float _miniMapRadarRingsDistance = 10f;

        /// <summary>
        /// The separation in meters between rings in radar mode
        /// </summary>
        public float miniMapRadarRingsDistance {
            get { return _miniMapRadarRingsDistance; }
            set {
                if (value != _miniMapRadarRingsDistance) {
                    _miniMapRadarRingsDistance = value;
                    SetupMiniMap();
                }
            }
        }

        [Range(0.01f, 4f)]
        [SerializeField]
        float _miniMapRadarRingsWidth = 1f;

        /// <summary>
        /// Width of radar rings
        /// </summary>
        public float miniMapRadarRingsWidth {
            get { return _miniMapRadarRingsWidth; }
            set {
                if (value != _miniMapRadarRingsWidth) {
                    _miniMapRadarRingsWidth = value;
                    SetupMiniMap();
                }
            }
        }



        [SerializeField]
        bool _miniMapRadarPulseEnabled = true;

        /// <summary>
        /// Enables/disables radar pulse
        /// </summary>
        public bool miniMapRadarPulseEnabled {
            get { return _miniMapRadarPulseEnabled; }
            set {
                if (value != _miniMapRadarPulseEnabled) {
                    _miniMapRadarPulseEnabled = value;
                    SetupMiniMap();
                }
            }
        }


        [SerializeField]
        MiniMapPulsePreset _miniMapRadarPulseAnimationPreset = MiniMapPulsePreset.Default;

        /// <summary>
        /// Opacity of radar pulse
        /// </summary>
        public MiniMapPulsePreset miniMapRadarPulseAnimationPreset {
            get { return _miniMapRadarPulseAnimationPreset; }
            set {
                if (value != _miniMapRadarPulseAnimationPreset) {
                    _miniMapRadarPulseAnimationPreset = value;
                    SetupMiniMap();
                }
            }
        }



        [Range(0.0f, 1f)]
        [SerializeField]
        float _miniMapRadarPulseOpacity = 0.25f;

        /// <summary>
        /// Opacity of radar pulse
        /// </summary>
        public float miniMapRadarPulseOpacity {
            get { return _miniMapRadarPulseOpacity; }
            set {
                if (value != _miniMapRadarPulseOpacity) {
                    _miniMapRadarPulseOpacity = value;
                    UpdateMiniMap();
                }
            }
        }


        [SerializeField]
        float _miniMapRadarPulseFrequency = 0.1f;

        /// <summary>
        /// Rings pulse frequency
        /// </summary>
        public float miniMapRadarPulseFrequency {
            get { return _miniMapRadarPulseFrequency; }
            set {
                if (value != _miniMapRadarPulseFrequency) {
                    _miniMapRadarPulseFrequency = value;
                    UpdateMiniMap();
                }
            }
        }

        [SerializeField]
        float _miniMapRadarPulseFallOff = 50;

        /// <summary>
        /// Rings pulse width
        /// </summary>
        public float miniMapRadarPulseFallOff {
            get { return _miniMapRadarPulseFallOff; }
            set {
                if (value != _miniMapRadarPulseFallOff) {
                    _miniMapRadarPulseFallOff = value;
                    UpdateMiniMap();
                }
            }
        }

        [SerializeField]
        float _miniMapRadarPulseSpeed = 50;

        /// <summary>
        /// Rings pulse speed
        /// </summary>
        public float miniMapRadarPulseSpeed {
            get { return _miniMapRadarPulseSpeed; }
            set {
                if (value != _miniMapRadarPulseSpeed) {
                    _miniMapRadarPulseSpeed = value;
                    UpdateMiniMap();
                }
            }
        }



        public void MiniMapZoomIn(float speed = 1f) {
            float amount = Time.deltaTime * speed;
            if (miniMapFullScreenState) {
                miniMapFullScreenZoomLevel += amount;
            } else {
                miniMapZoomLevel += amount;
            }
        }

        public void MiniMapZoomOut(float speed = 1f) {
            float amount = Time.deltaTime * speed;
            if (miniMapFullScreenState) {
                miniMapFullScreenZoomLevel -= amount;
            } else {
                miniMapZoomLevel -= amount;
            }
        }

        [SerializeField]
        bool _miniMapFullScreenState;

        /// <summary>
        /// Sets mini-map in full-screen mode or normal mode
        /// </summary>
        public bool miniMapFullScreenState {
            get {
                return _miniMapFullScreenState;
            }
            set {
                if (_miniMapFullScreenState != value) {
                    if (!_showMiniMap) {
                        showMiniMap = true;
                    }
                    OnMiniMapChangeFullScreenState?.Invoke(value);
                    MiniMapZoomToggle(value);
                }
            }
        }

        /// <summary>
        /// Forces an update of mini-map contents
        /// </summary>
        public void UpdateMiniMapContents(int numberOfFramesToRefresh = 1) {
            if (needMiniMapShot == 0) {
                needMiniMapShot += numberOfFramesToRefresh;
            }
            lastViewConeCameraAspect = 0;
            needUpdateMiniMapIcons = true;
        }


        /// <summary>
        /// Returns true if mouse pointer is over the mini-map
        /// </summary>
        /// <returns><c>true</c> if this instance is pointer over mini map; otherwise, <c>false</c>.</returns>
        public bool IsMouseOverMiniMap() {
            if (miniMapUIRootRT == null)
                return false;
            return RectTransformUtility.RectangleContainsScreenPoint(miniMapUIRootRT, Input.mousePosition);
        }

        [SerializeField]
        Vector3 _miniMapFullScreenWorldCenter;

        [Tooltip("Center of the world map in full screen mode")]
        public Vector3 miniMapFullScreenWorldCenter {
            get { return _miniMapFullScreenWorldCenter; }
            set {
                if (_miniMapFullScreenWorldCenter != value) {
                    _miniMapFullScreenWorldCenter = value;
                    needUpdateMiniMapIcons = true;
                }
            }
        }

        [SerializeField]
        Vector3 _miniMapFullScreenWorldSize = new Vector3(1000, 0, 1000);

        [Tooltip("Size of the world map")]
        public Vector3 miniMapFullScreenWorldSize {
            get {
                return _miniMapFullScreenWorldSize;
            }
            set {
                if (_miniMapFullScreenWorldSize != value) {
                    _miniMapFullScreenWorldSize = value;
                    needUpdateMiniMapIcons = true;
                }
            }
        }

        [Tooltip("Forces center of the world map to be the same position of the followed object")]
        public bool miniMapFullScreenWorldCenterFollows = true;

        [Tooltip("Prevents user move or rotate camera while in maximized mode")]
        public bool miniMapFullScreenFreezeCamera = true;


        [Tooltip("Ensures the map doesn't scroll beyond world size in maximized mode")]
        public bool miniMapFullScreenClampToWorldEdges = true;

        [Tooltip("Ensures the map doesn't scroll beyond world size in non-maximized mode")]
        public bool miniMapClampToWorldEdges;

        // used when contents set to Texture
        [Tooltip("Center of the world map")]
        [SerializeField]
        Vector3 _miniMapWorldCenter;

        public Vector3 miniMapWorldCenter {
            get { return _miniMapWorldCenter; }
            set {
                if (_miniMapWorldCenter != value) {
                    _miniMapWorldCenter = value;
                    UpdateMiniMap();
                }
            }
        }

        [Tooltip("Size of the world map")]
        [SerializeField]
        Vector3 _miniMapWorldSize = new Vector3(1000, 0, 1000);

        public Vector3 miniMapWorldSize {
            get { return _miniMapWorldSize; }
            set {
                if (_miniMapWorldSize != value) {
                    _miniMapWorldSize = value;
                    UpdateMiniMap();
                }
            }
        }


        [SerializeField]
        [Range(0f, 1f)]
        float _miniMapLutIntensity = 1f;

        public float miniMapLutIntensity {
            get { return _miniMapLutIntensity; }
            set {
                if (_miniMapLutIntensity != value) {
                    _miniMapLutIntensity = value;
                }
            }
        }

        [SerializeField]
        Texture2D _miniMapLutTexture;

        public Texture2D miniMapLutTexture {
            get { return _miniMapLutTexture; }
            set {
                if (_miniMapLutTexture != value) {
                    _miniMapLutTexture = value;
                }
            }
        }



        [SerializeField]
        Color _miniMapVignetteColor = new Color(0, 0, 0, 0.5f);

        /// <summary>
        /// Vignette color
        /// </summary>
        public Color miniMapVignetteColor {
            get { return _miniMapVignetteColor; }
            set {
                if (value != _miniMapVignetteColor) {
                    _miniMapVignetteColor = value;
                    SetupMiniMap();
                }
            }
        }

        #endregion

        #region MiniMap API

        /// <summary>
        /// Returns the altitude under the minimap camera
        /// </summary>
        public float GetAltitudeUnderMiniMapCamera() {
            if (miniMapCamera == null) return 0;
            Ray ray = miniMapCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Physics.Raycast(ray, out RaycastHit hitInfo);
            return miniMapCamera.transform.position.y - hitInfo.distance;
        }

        /// <summary>
        /// Re-centers the mini-map if user has dragged it
        /// </summary>
        public void ResetDragOffset() {
            miniMapFollowOffset = Misc.Vector3zero;
            needUpdateMiniMapIcons = true;
        }

        #endregion

    }

}



