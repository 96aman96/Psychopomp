using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CompassNavigatorPro {

    public enum CompassStyle {
        Angled = 0,
        Rounded = 1,
        Celtic_White = 2,
        Celtic_Black = 3,
        Fantasy1 = 10,
        Fantasy2 = 11,
        Fantasy3 = 12,
        Fantasy4 = 13,
        SciFi1 = 20,
        SciFi2 = 21,
        SciFi3 = 22,
        SciFi4 = 23,
        SciFi5 = 24,
        SciFi6 = 27,
        Clean = 28,
        Custom = 99
    }

    public enum WorldMappingMode {
        LimitedToBarWidth = 0,
        CameraFrustum = 1,
        Full180Degrees = 2,
        Full360Degrees = 3
    }

    public enum UpdateMode {
        NumberOfFrames,
        Time,
        Continuous,
        Scripting
    }


    [HelpURL("https://kronnect.com/guides-category/compass-navigator-pro-2/")]
    public partial class CompassPro : MonoBehaviour {

        #region Public properties

        [Tooltip("Camera used for computing the indicators and POI screen positions")]
        [SerializeField]
        Camera _cameraMain;

        public Camera cameraMain {
            get {
                if (_cameraMain == null) {
                    _cameraMain = Camera.main;
                    if (_cameraMain == null) {
                        _cameraMain = Misc.FindObjectOfType<Camera>(true);
                    }
                }
                return _cameraMain;
            }
            set {
                if (_cameraMain != value) {
                    _cameraMain = value;
                    Refresh();
                }
            }
        }


        [Tooltip("The pivot used to compute distances. In a third person setup, the follow could be the root of the player game object which is different than the orbiting camera.")]
        [SerializeField]
        Transform _follow;

        public Transform follow {
            get {
                return _follow;
            }
            set {
                if (_follow != value) {
                    _follow = value;
                    Refresh();
                }
            }
        }

        [Tooltip("Contents are always updated if camera moves or rotates. If not, this property specifies the interval between POI change checks")]
        [SerializeField]
        UpdateMode _updateMode = UpdateMode.NumberOfFrames;

        public UpdateMode updateMode {
            get { return _updateMode; }
            set {
                if (value != _updateMode) {
                    _updateMode = value;
                }
            }
        }

        [Tooltip("Frames between compass bar updates.")]
        [SerializeField]
        int _updateIntervalFrameCount = 60;

        public int updateIntervalFrameCount {
            get { return _updateIntervalFrameCount; }
            set {
                if (value != _updateIntervalFrameCount) {
                    _updateIntervalFrameCount = value;
                }
            }
        }

        [Tooltip("Seconds between compass bar updates")]
        [SerializeField]
        float _updateIntervalTime = 0.2f;

        public float updateIntervalTime {
            get { return _updateIntervalTime; }
            set {
                if (value != _updateIntervalTime) {
                    _updateIntervalTime = value;
                }
            }
        }

        [Tooltip("Shows the compass bar")]
        [SerializeField]
        bool _showCompassBar = true;

        /// <summary>
        /// Show/Hide compass
        /// </summary>
        public bool showCompassBar {
            get { return _showCompassBar; }
            set {
                if (value != _showCompassBar) {
                    _showCompassBar = value;
                    UpdateCompassBarAppearance();
                    UpdateCompassBarAlpha();
                    needUpdateCompassBarIcons = true;
                }
            }
        }

        [Tooltip("Compass bar style")]
        [SerializeField]
        CompassStyle _style = CompassStyle.Celtic_White;

        public CompassStyle style {
            get { return _style; }
            set {
                if (value != _style) {
                    _style = value;
                    UpdateCompassBarAppearance();
                }
            }
        }


        [Tooltip("Custom sprite for the compass bar. Check the documentation for configuring a sprite.")]
        [SerializeField]
        Sprite _compassBackSprite;

        public Sprite compassBackSprite {
            get { return _compassBackSprite; }
            set {
                if (value != _compassBackSprite) {
                    _compassBackSprite = value;
                    UpdateCompassBarAppearance();
                }
            }
        }



        [SerializeField]
        Color _compassTintColor = Color.white;

        /// <summary>
        /// The compass bar tint color.
        /// </summary>
        public Color compassTintColor {
            get { return _compassTintColor; }
            set {
                if (value != _compassTintColor) {
                    _compassTintColor = value;
                    UpdateCompassBarAppearance();
                }
            }
        }

        [Tooltip("The position of the North in degrees (0-360)")]
        [Range(-180, 180)]
        [SerializeField]
        float _northDegrees;

        /// <summary>
        /// Gets or sets the North position
        /// </summary>
        public float northDegrees {
            get { return _northDegrees; }
            set {
                if (value != _northDegrees) {
                    _northDegrees = value;
                    needUpdateCompassBarIcons = true;
                }
            }
        }

        [Tooltip("POIs beyond visible distance (meters) will not be shown in the compass bar")]
        [SerializeField]
        float _visibleMaxDistance = 500f;

        /// <summary>
        /// Gets or sets the maximum distance to a POI so it's visible in the compass bar.
        /// </summary>
        public float visibleMaxDistance {
            get { return _visibleMaxDistance; }
            set {
                if (value != _visibleMaxDistance) {
                    _visibleMaxDistance = value;
                }
            }
        }

        [Tooltip("POIs nearer than this distance (meters) will not be shown in the compass bar")]
        [SerializeField]
        float _visibleMinDistance;

        /// <summary>
        /// Gets or sets the minimum distance to a POI so it's visible in the compass bar.
        /// </summary>
        public float visibleMinDistance {
            get { return _visibleMinDistance; }
            set {
                if (value != _visibleMinDistance) {
                    _visibleMinDistance = value;
                }
            }
        }

        [Tooltip("Minimum POI distance to display its title. Distance text won't be shown for objects within this distance")]
        [SerializeField]
        float _titleMinPOIDistance = 10f;

        /// <summary>
        /// Minimum distance to show as text under the compass bar. Distance text won't be shown for objects within this distance.
        /// </summary>
        public float titleMinPOIDistance {
            get { return _titleMinPOIDistance; }
            set {
                if (value != _titleMinPOIDistance) {
                    _titleMinPOIDistance = value;
                }
            }
        }

        [Tooltip("Distance to a POI where the icon will start to grow as player approaches")]
        [SerializeField]
        float _nearDistance = 75f;

        /// <summary>
        /// Gets or sets the distance to a POI where the icon will start to grow as player approaches.
        /// </summary>
        public float nearDistance {
            get { return _nearDistance; }
            set {
                if (value != _nearDistance) {
                    _nearDistance = value;
                }
            }
        }

        [Tooltip("Minimum distance to a POI to be considered as explored/visited")]
        [SerializeField]
        float _visitedDistance = 25f;

        /// <summary>
        /// Gets or sets the minimum distance required to consider a POI as visited. Once the player gets near this POI below this distance, the POI will be marked as visited.
        /// </summary>
        public float visitedDistance {
            get { return _visitedDistance; }
            set {
                if (value != _visitedDistance) {
                    _visitedDistance = value;
                }
            }
        }

        [Tooltip("Shows on-screen indicators in the scene during playmode for the POIs (can be enabled/disabled per POI)")]
        [SerializeField]
        bool _showOnScreenIndicators = true;

        /// <summary>
        /// Render on-screen indicators.
        /// </summary>
        public bool showOnScreenIndicators {
            get { return _showOnScreenIndicators; }
            set {
                if (value != _showOnScreenIndicators) {
                    _showOnScreenIndicators = value;
                }
            }
        }

        [SerializeField]
        GameObject _compassIconPrefab;

        /// <summary>
        /// Compass icon prefab
        /// </summary>
        public GameObject compassIconPrefab {
            get { return _compassIconPrefab; }
            set {
                if (value != _compassIconPrefab) {
                    _compassIconPrefab = value;
                }
            }
        }

        [Tooltip("Indicator prefab. Used for both on-screen and off-screen modes.")]
        [SerializeField]
        GameObject _onScreenIndicatorPrefab;

        /// <summary>
        /// On-screen indicator prefab
        /// </summary>
        public GameObject onScreenIndicatorPrefab {
            get { return _onScreenIndicatorPrefab; }
            set {
                if (value != _onScreenIndicatorPrefab) {
                    _onScreenIndicatorPrefab = value;
                }
            }
        }

        [Tooltip("Transparency level of on-screen indicators")]
        [SerializeField]
        [Range(0, 1f)]
        float _onScreenIndicatorAlpha = 0.85f;

        /// <summary>
        /// Transparency level for indicator icons.
        /// </summary>
        public float onScreenIndicatorAlpha {
            get { return onScreenIndicatorAlpha; }
            set {
                if (value != onScreenIndicatorAlpha) {
                    onScreenIndicatorAlpha = value;
                }
            }
        }

        [Tooltip("Scaling applied to indicators shown during playmode")]
        [SerializeField]
        float _onScreenIndicatorScale = 1f;

        /// <summary>
        /// Gets or sets the on-screen indicator scale during playmode.
        /// </summary>
        public float onScreenIndicatorScale {
            get { return _onScreenIndicatorScale; }
            set {
                if (value != _onScreenIndicatorScale) {
                    _onScreenIndicatorScale = value;
                }
            }
        }

        [Tooltip("Distance at which the on-screen indicator will start to fade when it approaches camera")]
        [SerializeField]
        float _onScreenIndicatorNearFadeDistance = 10f;

        /// <summary>
        /// Distance at which the screen indicator will start to fade.
        /// </summary>
        public float onScreenIndicatorNearFadeDistance {
            get { return _onScreenIndicatorNearFadeDistance; }
            set {
                if (value != _onScreenIndicatorNearFadeDistance) {
                    _onScreenIndicatorNearFadeDistance = value;
                }
            }
        }

        [Tooltip("Minimum distance at which the on-screen indicator disappear")]
        [SerializeField]
        float _onScreenIndicatorNearFadeMin = 1f;

        public float onScreenIndicatorNearFadeMin {
            get { return _onScreenIndicatorNearFadeMin; }
            set {
                if (value != _onScreenIndicatorNearFadeMin) {
                    _onScreenIndicatorNearFadeMin = value;
                }
            }
        }

        [Tooltip("Whether the distance in meters should be shown under the indicator")]
        [SerializeField]
        bool _onScreenIndicatorShowDistance = true;

        /// <summary>
        /// Whether the distance in meters should be shown next to the title
        /// </summary>
        public bool onScreenIndicatorShowDistance {
            get { return _onScreenIndicatorShowDistance; }
            set {
                if (value != _onScreenIndicatorShowDistance) {
                    _onScreenIndicatorShowDistance = value;
                }
            }
        }

        [Tooltip("The string format for displaying the distance on the indicators. The syntax for this string format corresponds with the available options for ToString(format) method of C#")]
        [SerializeField]
        string _onScreenIndicatorShowDistanceFormat = "0m";

        /// <summary>
        /// The string format for displaying the distance.
        /// This string format corresponds with the available options for ToString(format) method of C#.
        /// </summary>
        public string onScreenIndicatorShowDistanceFormat {
            get { return _onScreenIndicatorShowDistanceFormat; }
            set {
                if (value != _onScreenIndicatorShowDistanceFormat) {
                    _onScreenIndicatorShowDistanceFormat = value;
                }
            }
        }


        [Tooltip("Whether the title of the POI should also be displayed")]
        [SerializeField]
        bool _onScreenIndicatorShowTitle;

        /// <summary>
        /// Whether the title of the POI should also be displayed
        /// </summary>
        public bool onScreenIndicatorShowTitle {
            get { return _onScreenIndicatorShowTitle; }
            set {
                if (value != _onScreenIndicatorShowTitle) {
                    _onScreenIndicatorShowTitle = value;
                }
            }
        }

        [Tooltip("Show indicators on the edges of screen during playmode for POIs not visible in the screen (can be enabled/disabled per POI)")]
        [SerializeField]
        bool _showOffScreenIndicators = true;

        /// <summary>
        /// Renders off-screen indicators.
        /// </summary>
        public bool showOffScreenIndicators {
            get { return _showOffScreenIndicators; }
            set {
                if (value != _showOffScreenIndicators) {
                    _showOffScreenIndicators = value;
                }
            }
        }

        [Tooltip("Scaling applied to offscreen indicators shown during playmode.")]
        [SerializeField]
        float _offScreenIndicatorScale = 1f;

        /// <summary>
        /// Gets or sets the offscreen indicator scale during playmode.
        /// </summary>
        public float offScreenIndicatorScale {
            get { return _offScreenIndicatorScale; }
            set {
                if (value != _offScreenIndicatorScale) {
                    _offScreenIndicatorScale = value;
                }
            }
        }

        [Tooltip("Margin between the indicator and screen edge")]
        [Range(0, 0.4f)]
        [SerializeField]
        float _offScreenIndicatorMargin = 0.05f;

        /// <summary>
        /// Separation between offscreen indicator and screen edges.
        /// </summary>
        public float offScreenIndicatorMargin {
            get { return _offScreenIndicatorMargin; }
            set {
                if (value != _offScreenIndicatorMargin) {
                    _offScreenIndicatorMargin = value;
                }
            }
        }


        [Tooltip("Enable to avoid off-screen icons to overlap.")]
        [SerializeField]
        bool _offScreenIndicatorAvoidOverlap = true;

        /// <summary>
        /// Enable to avoid off-screen icons to overlap
        /// </summary>
        public bool offScreenIndicatorAvoidOverlap {
            get { return _offScreenIndicatorAvoidOverlap; }
            set {
                if (value != _offScreenIndicatorAvoidOverlap) {
                    _offScreenIndicatorAvoidOverlap = value;
                }
            }
        }

        [Tooltip("Overlap distance")]
        [Range(0, 0.1f)]
        [SerializeField]
        float _offScreenIndicatorOverlapDistance = 0.04f;

        /// <summary>
        /// Overlap distance.
        /// </summary>
        public float offScreenIndicatorOverlapDistance {
            get { return _offScreenIndicatorOverlapDistance; }
            set {
                if (value != _offScreenIndicatorOverlapDistance) {
                    _offScreenIndicatorOverlapDistance = value;
                }
            }
        }


        [Tooltip("Transparency level of offscreen indicators")]
        [SerializeField]
        [Range(0, 1f)]
        float _offScreenIndicatorAlpha = 0.85f;

        /// <summary>
        /// Transparency level for indicator icons.
        /// </summary>
        public float offScreenIndicatorAlpha {
            get { return _offScreenIndicatorAlpha; }
            set {
                if (value != _offScreenIndicatorAlpha) {
                    _offScreenIndicatorAlpha = value;
                }
            }
        }

        [Tooltip("Transparency of the compass bar")]
        [Range(0, 1f)]
        [SerializeField]
        float _alpha = 1.0f;

        /// <summary>
        /// The alpha (transparency) of the compass bar. Setting this value will make the bar shift smoothly from current alpha to the new value (see fadeDuration).
        /// </summary>
        public float alpha {
            get { return _alpha; }
            set {
                if (value != _alpha) {
                    _alpha = value;
                    UpdateCompassBarAlpha();
                }
            }
        }

        [Tooltip("Hides the compass bar if no POIs are below visible distance")]
        [SerializeField]
        bool _autoHide;

        /// <summary>
        /// If no POIs are below the visible distance param, hide the compass bar
        /// </summary>
        public bool autoHide {
            get { return _autoHide; }
            set {
                if (value != _autoHide) {
                    _autoHide = value;
                }
            }
        }


        [Tooltip("Duration of alpha changes in seconds")]
        [Range(0, 8f)]
        [SerializeField]
        float _fadeDuration = 2.0f;

        /// <summary>
        /// Sets the duration for any alpha change.
        /// </summary>
        public float fadeDuration {
            get { return _fadeDuration; }
            set {
                if (value != _fadeDuration) {
                    _fadeDuration = value;
                }
            }
        }

        [Tooltip("Makes the bar always visible (ignores alpha property) while in Edit Mode")]
        [SerializeField]
        bool _alwaysVisibleInEditMode = true;

        /// <summary>
        /// Set this value to true to make the compass bar always visible in Edit Mode (ignores alpha property while editing).
        /// </summary>
        public bool alwaysVisibleInEditMode {
            get { return _alwaysVisibleInEditMode; }
            set {
                if (value != _alwaysVisibleInEditMode) {
                    _alwaysVisibleInEditMode = value;
                    UpdateCompassBarAlpha();
                }
            }
        }

        [Tooltip("Distance from the bottom of the screen in %")]
        [Range(-0.2f, 1.2f)]
        [SerializeField]
        float _verticalPosition = 0.97f;

        /// <summary>
        /// Distance in % of the screen from the bottom edge of the screen.
        /// </summary>
        public float verticalPosition {
            get { return _verticalPosition; }
            set {
                if (value != _verticalPosition) {
                    _verticalPosition = value;
                    UpdateCompassBarAppearance();
                }
            }
        }

        [Tooltip("Distance from the center of the screen in %")]
        [Range(-0.5f, 0.5f)]
        [SerializeField]
        float _horizontalPosition;

        /// <summary>
        /// Horizontal position from the center of screen
        /// </summary>
        public float horizontalPosition {
            get { return _horizontalPosition; }
            set {
                if (value != _horizontalPosition) {
                    _horizontalPosition = value;
                    UpdateCompassBarAppearance();
                }
            }
        }


        [Tooltip("Bending amount. Set this to zero to disable bending effect")]
        [Range(-1f, 1f)]
        [SerializeField]
        float _bendAmount;

        /// <summary>
        /// Bending amount
        /// </summary>
        public float bendAmount {
            get { return _bendAmount; }
            set {
                if (value != _bendAmount) {
                    _bendAmount = value;
                    if (_bendAmount == 0) {
                        _verticalPosition = 0.94f;
                    }
                    UpdateCompassBarAppearance();
                    UpdateTitleAppearance();
                }
            }
        }


        [Tooltip("Width of the compass bar in % of the screen width")]
        [Range(0.05f, 1f)]
        [SerializeField]
        float _width = 0.65f;

        /// <summary>
        /// Width of the compass bar in % of the screen width.
        /// </summary>
        public float width {
            get { return _width; }
            set {
                if (value != _width) {
                    _width = value;
                    UpdateCompassBarAppearance();
                    UpdateHalfWindsAppearance();
                }
            }
        }

        [Tooltip("Vertical scale of the compass bar.")]
        [Range(0.05f, 5f)]
        [SerializeField]
        float _height = 1f;

        /// <summary>
        /// Vertical scale of the compass bar.
        /// </summary>
        public float height {
            get { return _height; }
            set {
                if (value != _height) {
                    _height = value;
                    UpdateCompassBarAppearance();
                }
            }
        }


        [Tooltip("Enables edge fade out effect")]
        [SerializeField]
        bool _edgeFadeOut;

        /// <summary>
        /// Enabled edge fade out effect
        /// </summary>
        public bool edgeFadeOut {
            get { return _edgeFadeOut; }
            set {
                if (value != _edgeFadeOut) {
                    _edgeFadeOut = value;
                    UpdateCompassBarAppearance();
                }
            }
        }



        [Tooltip("If edge fade out affects title and text below compass bar")]
        [SerializeField]
        bool _edgeFadeOutText = true;

        /// <summary>
        /// If the edge fade out should affect the title and text below the compass bar
        /// </summary>
        public bool edgeFadeOutText {
            get { return _edgeFadeOutText; }
            set {
                if (value != _edgeFadeOutText) {
                    _edgeFadeOutText = value;
                    UpdateCompassBarAppearance();
                }
            }
        }


        [Tooltip("Width of the edge fade out")]
        [Range(0, 1f)]
        [SerializeField]
        float _edgeFadeOutWidth = 0.1f;

        /// <summary>
        /// Width of the edge fade out
        /// </summary>
        public float edgeFadeOutWidth {
            get { return _edgeFadeOutWidth; }
            set {
                if (value != _edgeFadeOutWidth) {
                    _edgeFadeOutWidth = value;
                    UpdateCompassBarAppearance();
                }
            }
        }


        [Tooltip("Start of the edge fade out.")]
        [Range(0f, 1f)]
        [SerializeField]
        float _edgeFadeOutStart = 0;

        /// <summary>
        /// Start of the edge fade out
        /// </summary>
        public float edgeFadeOutStart {
            get { return _edgeFadeOutStart; }
            set {
                if (value != _edgeFadeOutStart) {
                    _edgeFadeOutStart = value;
                    UpdateCompassBarAppearance();
                }
            }
        }


        [Tooltip("Width of the end caps of the compass bar. This setting limits the usable horizontal range of the bar in the screen to prevent icons being drawn over the art of the end caps of the bar")]
        [Range(0, 100f)]
        [SerializeField]
        float _endCapsWidth = 54f;

        /// <summary>
        /// Width of the end caps for the compass bar.
        /// </summary>
        public float endCapsWidth {
            get { return _endCapsWidth; }
            set {
                if (value != _endCapsWidth) {
                    _endCapsWidth = value;
                    UpdateCompassBarAppearance();
                    needUpdateCompassBarIcons = true;
                }
            }
        }

        [Tooltip("Whether N, W, S, E should be visible in the compass bar")]
        [SerializeField]
        bool _showCardinalPoints = true;

        /// <summary>
        /// Whether cardinal points (N, W, S, E) should be visible in the compass bar
        /// </summary>
        public bool showCardinalPoints {
            get { return _showCardinalPoints; }
            set {
                if (value != _showCardinalPoints) {
                    _showCardinalPoints = value;
                    needUpdateCompassBarIcons = true;
                }
            }
        }

        [Tooltip("Whether NW, NE, SW, SE should be visible in the compass bar")]
        [SerializeField]
        bool _showOrdinalPoints = true;

        public bool showOrdinalPoints {
            get { return _showOrdinalPoints; }
            set {
                if (value != _showOrdinalPoints) {
                    _showOrdinalPoints = value;
                    needUpdateCompassBarIcons = true;
                }
            }
        }


        [Range(0.1f, 3f)]
        [SerializeField]
        float _cardinalScale = 1f;

        /// <summary>
        /// Scaling factor for cardinal letters
        /// </summary>
        public float cardinalScale {
            get { return _cardinalScale; }
            set {
                if (value != _cardinalScale) {
                    _cardinalScale = value;
                    needUpdateCompassBarIcons = true;
                }
            }
        }

        [Range(0.1f, 3f)]
        [SerializeField]
        float _ordinalScale = 1f;

        /// <summary>
        /// Scaling factor for ordinal letters
        /// </summary>
        public float ordinalScale {
            get { return _ordinalScale; }
            set {
                if (value != _ordinalScale) {
                    _ordinalScale = value;
                    needUpdateCompassBarIcons = true;
                }
            }
        }


        [Tooltip("Optional vertical displacement for both cardinal and ordinal points")]
        [SerializeField]
        float _cardinalPointsVerticalOffset;

        /// <summary>
        /// Optional vertical offset for compass cardinal/ordinal points
        /// </summary>
        public float cardinalPointsVerticalOffset {
            get { return _cardinalPointsVerticalOffset; }
            set {
                if (value != _cardinalPointsVerticalOffset) {
                    _cardinalPointsVerticalOffset = value;
                    needUpdateCompassBarIcons = true;
                }
            }
        }


        [Tooltip("Enable vertical interval marks in the compass bar")]
        [SerializeField]
        bool _showHalfWinds = true;

        /// <summary>
        /// Whether bar ticks should be visible
        /// </summary>
        public bool showHalfWinds {
            get { return _showHalfWinds; }
            set {
                if (value != _showHalfWinds) {
                    _showHalfWinds = value;
                    UpdateHalfWindsAppearance();
                    needUpdateCompassBarIcons = true;
                }
            }
        }


        [SerializeField, Range(0.01f, 0.5f)]
        float _halfWindsHeight = 0.125f;

        /// <summary>
        /// The compass bar ticks height.
        /// </summary>
        public float halfWindsHeight {
            get { return _halfWindsHeight; }
            set {
                if (value != _halfWindsHeight) {
                    _halfWindsHeight = value;
                    UpdateHalfWindsAppearance();
                }
            }
        }

        [SerializeField, Range(0.01f, 2f)]
        float _halfWindsWidth = 0.2f;

        /// <summary>
        /// The compass bar ticks width.
        /// </summary>
        public float halfWindsWidth {
            get { return _halfWindsWidth; }
            set {
                if (value != _halfWindsWidth) {
                    _halfWindsWidth = value;
                    UpdateHalfWindsAppearance();
                }
            }
        }

        [SerializeField, Range(1f, 45f)]
        float _halfWindsInterval = 5f;

        /// <summary>
        /// The compass bar ticks interval.
        /// </summary>
        public float halfWindsInterval {
            get { return _halfWindsInterval; }
            set {
                if (value != _halfWindsInterval) {
                    _halfWindsInterval = value;
                    UpdateHalfWindsAppearance();
                }
            }
        }

        [SerializeField]
        Color _halfWindsTintColor = new Color(1f, 1f, 1f, 0.5f);

        /// <summary>
        /// The compass bar ticks tint color.
        /// </summary>
        public Color halfWindsTintColor {
            get { return _halfWindsTintColor; }
            set {
                if (value != _halfWindsTintColor) {
                    _halfWindsTintColor = value;
                    UpdateHalfWindsAppearance();
                }
            }
        }

        [Tooltip("The distance from the center of the compass bar where a POI's label is visible")]
        [Range(0.001f, 0.2f)]
        [SerializeField]
        float _labelHotZone = 0.015f;

        /// <summary>
        /// The distance from the center of the compass bar where a POI label can be shown.
        /// </summary>
        public float labelHotZone {
            get { return _labelHotZone; }
            set {
                if (value != _labelHotZone) {
                    _labelHotZone = value;
                }
            }
        }

        [SerializeField]
        float _maxIconSize = 1.15f;

        /// <summary>
        /// Maximum icon size. Icons grow or shrinks in the compass bar depending on distance.
        /// </summary>
        public float maxIconSize {
            get { return _maxIconSize; }
            set {
                if (value != _maxIconSize) {
                    _maxIconSize = value;
                }
            }
        }

        [SerializeField]
        float _minIconSize = 0.5f;

        /// <summary>
        /// Minimum icon size. Icons grow or shrinks in the compass bar depending on distance.
        /// </summary>
        public float minIconSize {
            get { return _minIconSize; }
            set {
                if (value != _minIconSize) {
                    _minIconSize = value;
                }
            }
        }

        [Tooltip("Duration for the scale animation when the POI appears on the compass bar")]
        [Range(0, 5)]
        [SerializeField]
        float _scaleInDuration = 0.3f;

        /// <summary>
        /// Duration for the poi's icon scaling effect when it appears on the compass bar
        /// </summary>
        public float scaleInDuration {
            get { return _scaleInDuration; }
            set {
                if (value != _scaleInDuration) {
                    _scaleInDuration = value;
                }
            }
        }


        [SerializeField]
        CompassProPOI _focusedPOI;

        /// <summary>
        /// A focused POI will be always visible in the compass bar
        /// </summary>
        public CompassProPOI focusedPOI {
            get { return _focusedPOI; }
            set {
                if (value != _focusedPOI) {
                    _focusedPOI = value;
                    needUpdateCompassBarIcons = true;
                }
            }
        }

        [Tooltip("How POIs positions are mapped to the bar. 1) Limited To Bar Width = the bar width determines the view angle, 2) Camera Frustum = the entire camera frustum is mapped to the bar width, 3) Full 180 degrees = all POIs in front of the camera will appear in the compass bar. 4) Full 360 degrees = all POIs are visible in the compass bar")]
        [SerializeField]
        WorldMappingMode _worldMappingMode = WorldMappingMode.CameraFrustum;

        /// <summary>
        /// Set this value to true to consider the width of the bar equal to the width of the viewport so the angle is not reduced.
        /// </summary>
        public WorldMappingMode worldMappingMode {
            get { return _worldMappingMode; }
            set {
                if (value != _worldMappingMode) {
                    _worldMappingMode = value;
                    UpdateHalfWindsAppearance();
                    needUpdateCompassBarIcons = true;
                }
            }
        }

        [Tooltip("Vertical offset in pixels for the text with respect to the compass bar.")]
        [Range(-200, 200)]
        [SerializeField]
        float _textVerticalPosition = -30;

        /// <summary>
        /// Vertical offset for the text of POIs when visited for first time
        /// </summary>
        public float textVerticalPosition {
            get { return _textVerticalPosition; }
            set {
                if (value != _textVerticalPosition) {
                    _textVerticalPosition = value;
                    UpdateTextAppearanceEditMode();
                }
            }
        }

        [Tooltip("Scaling applied to the text")]
        [Range(0.02f, 3f)]
        [SerializeField]
        float _textScale = 0.2f;

        /// <summary>
        /// Scaling applied to text
        /// </summary>
        public float textScale {
            get { return _textScale; }
            set {
                if (value != _textScale) {
                    _textScale = value;
                    UpdateTextAppearanceEditMode();
                }
            }
        }

        [Tooltip("Controls the spacing between each letter in the reveal text.")]
        [Range(0.02f, 3f)]
        [SerializeField]
        float _textLetterSpacing = 1f;

        /// <summary>
        /// Scaling applied to animated letters
        /// </summary>
        public float textLetterSpacing {
            get { return _textLetterSpacing; }
            set {
                if (value != _textLetterSpacing) {
                    _textLetterSpacing = value;
                }
            }
        }

        [Tooltip("Show a revealing text effect when discovering POIs for the first time")]
        [SerializeField]
        bool _textRevealEnabled = true;

        /// <summary>
        /// Enabled text revealing effect when discovering a POI for the first time
        /// </summary>
        public bool textRevealEnabled {
            get { return _textRevealEnabled; }
            set {
                if (value != _textRevealEnabled) {
                    _textRevealEnabled = value;
                }
            }
        }

        [Tooltip("Text reveal duration in seconds")]
        [Range(0, 3)]
        [SerializeField]
        float _textRevealDuration = 0.5f;

        /// <summary>
        /// Duration of the text reveal
        /// </summary>
        public float textRevealDuration {
            get { return _textRevealDuration; }
            set {
                if (value != _textRevealDuration) {
                    _textRevealDuration = value;
                }
            }
        }

        [Tooltip("Delay in appearance of each letter during a text reveal")]
        [Range(0, 1)]
        [SerializeField]
        float _textRevealLetterDelay = 0.05f;

        /// <summary>
        /// Delay in appearance of each letter during a text reveal
        /// </summary>
        public float textRevealLetterDelay {
            get { return _textRevealLetterDelay; }
            set {
                if (value != _textRevealLetterDelay) {
                    _textRevealLetterDelay = value;
                }
            }
        }

        [Tooltip("Text duration in screen")]
        [Range(0, 20)]
        [SerializeField]
        float _textDuration = 5f;

        /// <summary>
        /// Duration of the text on screen before fading out
        /// </summary>
        public float textDuration {
            get { return _textDuration; }
            set {
                if (value != _textDuration) {
                    _textDuration = value;
                }
            }
        }

        [Tooltip("Duration of the text fade out.")]
        [Range(0, 10)]
        [SerializeField]
        float _textFadeOutDuration = 2f;

        /// <summary>
        /// Duration of the text fade out
        /// </summary>
        public float textFadeOutDuration {
            get { return _textFadeOutDuration; }
            set {
                if (value != _textFadeOutDuration) {
                    _textFadeOutDuration = value;
                }
            }
        }

        [Tooltip("Enable or disable text shadow")]
        [SerializeField]
        bool _textShadowEnabled = true;

        /// <summary>
        /// Shows a drop shadow under the text
        /// </summary>
        public bool textShadowEnabled {
            get { return _textShadowEnabled; }
            set {
                if (value != _textShadowEnabled) {
                    _textShadowEnabled = value;
                    if (!Application.isPlaying) {
                        UpdateTextAppearanceEditMode();
                    }
                }
            }
        }

        [SerializeField]
        Font _textFont;

        /// <summary>
        /// Font for the text
        /// </summary>
        public Font textFont {
            get {
                if (_textFont == null) {
                    _textFont = Resources.Load<Font>("CNPro/Fonts/Vollkorn-Regular");
                }
                return _textFont;
            }
            set {
                if (value != _textFont) {
                    _textFont = value;
                    UpdateTextAppearanceEditMode();
                }
            }
        }


        [Tooltip("Vertical offset in pixels for the title with respect to the compass bar")]
        [Range(-200, 200)]
        [SerializeField]
        float _titleVerticalPosition = 18f;

        /// <summary>
        /// Vertical offset for the title of the (visited/known) centered POI in the compass bar
        /// </summary>
        public float titleVerticalPosition {
            get { return _titleVerticalPosition; }
            set {
                if (value != _titleVerticalPosition) {
                    _titleVerticalPosition = value;
                    UpdateTitleAppearanceEditMode();
                }
            }
        }

        [Tooltip("Scaling applied to the title")]
        [Range(0.02f, 3)]
        [SerializeField]
        float _titleScale = 0.1f;

        /// <summary>
        /// Scaling applied to title
        /// </summary>
        public float titleScale {
            get { return _titleScale; }
            set {
                if (value != _titleScale) {
                    _titleScale = value;
                    UpdateTitleAppearanceEditMode();
                }
            }
        }

        [SerializeField]
        Font _titleFont;

        /// <summary>
        /// Font for the title
        /// </summary>
        public Font titleFont {
            get {
                return _titleFont;
            }
            set {
                if (value != _titleFont) {
                    _titleFont = value;
                    UpdateTitleAppearanceEditMode();
                }
            }
        }

        [Tooltip("Font used for the title when bending is enabled")]
        [SerializeField]
        bool _titleShadowEnabled = true;

        /// <summary>
        /// Shows a drop shadow under the title
        /// </summary>
        public bool titleShadowEnabled {
            get { return _titleShadowEnabled; }
            set {
                if (value != _titleShadowEnabled) {
                    _titleShadowEnabled = value;
                    if (!Application.isPlaying) {
                        UpdateTitleAppearanceEditMode();
                    }
                }
            }
        }


        [Tooltip("Font used for the title when bending is disabled")]
        [SerializeField]
        TMP_FontAsset _titleFontTMP;

        /// <summary>
        /// Font for the title (SDF)
        /// </summary>
        public TMP_FontAsset titleFontTMP {
            get {
                return _titleFontTMP;
            }
            set {
                if (value != _titleFontTMP) {
                    _titleFontTMP = value;
                    UpdateTitleAppearanceEditMode();
                }
            }
        }


        [Tooltip("Whether the distance in meters should be shown in the title")]
        [SerializeField]
        bool _titleShowDistance;

        /// <summary>
        /// Whether the distance in meters should be shown next to the title
        /// </summary>
        public bool titleShowDistance {
            get { return _titleShowDistance; }
            set {
                if (value != _titleShowDistance) {
                    _titleShowDistance = value;
                }
            }
        }


        [Tooltip("The string format for displaying the distance in the title. The syntax for this string format corresponds with the available options for ToString(format) method of C#")]
        [SerializeField]
        string _titleShowDistanceFormat = "0.0 m";

        /// <summary>
        /// The string format for displaying the distance in the tile.
        /// This string format corresponds with the available options for ToString(format) method of C#.
        /// </summary>
        public string titleShowDistanceFormat {
            get { return _titleShowDistanceFormat; }
            set {
                if (value != _titleShowDistanceFormat) {
                    _titleShowDistanceFormat = value;
                }
            }
        }

        [Tooltip("Whether 3D distance should be computed instead of planar X/Z distance")]
        [SerializeField]
        bool _use3Ddistance = false;

        /// <summary>
        /// Check whether 3D distance should be computed instead of planar X/Z distance.
        /// </summary>
        public bool use3Ddistance {
            get { return _use3Ddistance; }
            set {
                if (value != _use3Ddistance) {
                    _use3Ddistance = value;
                }
            }
        }


        [Tooltip("Minimum difference in altitude from camera to show 'above' or 'below'")]
        [Range(1f, 50f)]
        [SerializeField]
        float _sameAltitudeThreshold = 3f;

        /// <summary>
        /// Minimum difference in altitude from camera to show "above" or "below"
        /// </summary>
        public float sameAltitudeThreshold {
            get { return _sameAltitudeThreshold; }
            set {
                if (value != _sameAltitudeThreshold) {
                    _sameAltitudeThreshold = value;
                }
            }
        }

        [Tooltip("Whether the distance in meters should be shown in the POI indicator")]
        [SerializeField]
        bool _showDistance = true;

        /// <summary>
        /// Whether the distance in meters should be shown next to the title
        /// </summary>
        public bool showDistance {
            get { return _showDistance; }
            set {
                if (value != _showDistance) {
                    _showDistance = value;
                }
            }
        }

        [Tooltip("The string format for displaying the distance under the icons on the compass bar. The syntax for this string format corresponds with the available options for ToString(format) method of C#")]
        [SerializeField]
        string _showDistanceFormat = "0m";

        /// <summary>
        /// The string format for displaying the distance.
        /// This string format corresponds with the available options for ToString(format) method of C#.
        /// </summary>
        public string showDistanceFormat {
            get { return _showDistanceFormat; }
            set {
                if (value != _showDistanceFormat) {
                    _showDistanceFormat = value;
                }
            }
        }

        [Tooltip("Default audio clip to be played when a POI is visited for the first time. Note that you can specify a different audio clip in the POI script itself")]
        [SerializeField]
        AudioClip _visitedDefaultAudioClip;

        /// <summary>
        /// Default audio clip to play when a POI is visited the first time. Note that you can specify a different audio clip in the POI script itself.
        /// </summary>
        public AudioClip visitedDefaultAudioClip {
            get { return _visitedDefaultAudioClip; }
            set {
                if (value != _visitedDefaultAudioClip) {
                    _visitedDefaultAudioClip = value;
                }
            }
        }

        [Tooltip("Default audio clip to be played when a POI beacon is shown. Note that you can specify a different audio clip in the POI script itself")]
        [SerializeField]
        AudioClip _beaconDefaultAudioClip;

        /// <summary>
        /// Default audio clip to play when a POI beacon is shown (see manual for more info about POI beacons).
        /// </summary>
        public AudioClip beaconDefaultAudioClip {
            get { return _beaconDefaultAudioClip; }
            set {
                if (value != _beaconDefaultAudioClip) {
                    _beaconDefaultAudioClip = value;
                }
            }
        }

        [Tooltip("Default audio clip to play for the heartbeat effect. This effect is enabled on each POI and will play a custom sound with variable speed depending on distance")]
        [SerializeField]
        AudioClip _heartbeatDefaultAudioClip;

        /// <summary>
        /// Default audio clip to play for the heartbeat effect. This effect is enabled on each POI and will play a custom sound with variable speed depending on distance.
        /// </summary>
        public AudioClip heartbeatDefaultAudioClip {
            get { return _heartbeatDefaultAudioClip; }
            set {
                if (value != _heartbeatDefaultAudioClip) {
                    _heartbeatDefaultAudioClip = value;
                }
            }
        }

        [Tooltip("Preserve compass bar between scene changes.")]
        [SerializeField]
        bool _dontDestroyOnLoad;

        /// <summary>
        /// Preserves compass bar between scene changes
        /// </summary>
        public bool dontDestroyOnLoad {
            get { return _dontDestroyOnLoad; }
            set {
                if (value != _dontDestroyOnLoad) {
                    _dontDestroyOnLoad = value;
                }
            }
        }

        /// <summary>
        /// Keeps a reference to the nearest POI to the camera
        /// </summary>
        [NonSerialized]
        public CompassProPOI nearestPOI;

        #endregion
        

        #region Public API

        /// <summary>
        /// Gets a reference to the Compass API.
        /// </summary>
        public static CompassPro instance {
            get {
                if (_instance == null) {
                    _instance = Misc.FindObjectOfType<CompassPro>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Call this to force a refresh of contents of compass bar & minimap
        /// </summary>
        public void Refresh() {
            needUpdateMiniMapIcons = true;
            needUpdateCompassBarIcons = true;
        }

        /// <summary>
        /// Used to add a POI to the compass. Returns false if POI is already registered.
        /// </summary>
        public bool POIRegister(CompassProPOI newPOI) {
            bool res = false;
            foreach (var compass in compasses) {
                if (compass != null) {
                    if (!compass.POIisRegistered(newPOI)) {
                        res = compass.POIRegister_internal(newPOI);
                    }
                }
            }
            return res;
        }

        bool POIRegister_internal(CompassProPOI newPOI) {
#if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfAnyPrefab(gameObject)) {
                return false;
            }
#endif

            newPOI.compass = this;
            pois.Add(newPOI);
            needsIconSorting = true;
            Refresh();

            OnPOIRegister?.Invoke(newPOI);

            return true;
        }

        /// <summary>
        /// Returns whether the POI is currently registered.
        /// </summary>
        public bool POIisRegistered(CompassProPOI poi) {
            int iconsCount = pois.Count;
            for (int k = 0; k < iconsCount; k++) {
                CompassProPOI p = pois[k];
                if (p == null) continue;
                if (p.id == poi.id) {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Updates POI rendering order. Call this method if you change the priority property manually.
        /// </summary>
        public void POIResort() {
            foreach (var compass in compasses) {
                if (compass != null) {
                    compass.needsIconSorting = true;
                }
            }
        }

        /// <summary>
        /// Call this method to remove a POI from the compass.
        /// </summary>
        public void POIUnregister(CompassProPOI newPOI) {
            foreach (var compass in compasses) {
                if (compass != null) {
                    POIUnregister_internal(newPOI);
                }
            }
        }

        void POIUnregister_internal(CompassProPOI poi) {
            int iconsCount = pois.Count;
            for (int k = 0; k < iconsCount; k++) {
                CompassProPOI p = pois[k];
                if (p == null) continue;
                if (p.id == poi.id) {
                    OnPOIUnregister?.Invoke(poi);
                    p.Release();
                    pois[k] = null;
                    Refresh();
                    break;
                }
            }
        }

        /// <summary>
        /// Shows given POI as on-screen indicator in the scene and makes its icon always visible in the compass bar
        /// </summary>
        public void POIFocus(CompassProPOI poi) {
            poi.showOnScreenIndicator = true;
            foreach (var compass in compasses) {
                if (compass != null) {
                    compass.focusedPOI = poi;
                }
            }
        }

        /// <summary>
        /// Unfocus any focused POI.
        /// </summary>
        public void POIBlur() {
            foreach (var compass in compasses) {
                if (compass != null && compass.focusedPOI != null) {
                    compass.focusedPOI.showOnScreenIndicator = false;
                    compass.focusedPOI = null;
                }
            }
        }

        /// <summary>
        /// Starts the circle animation around a POI in the minimap
        /// </summary>
        public void POIStartCircleAnimation(CompassProPOI poi) {
            if (poi != null) {
                poi.StartCircleAnimation();
            }
        }

        /// <summary>
        /// Show a light beacon over the specified POI.
        /// </summary>
        public GameObject POIShowBeacon(CompassProPOI existingPOI, float duration, float horizontalScale = 1f) {
            return POIShowBeacon(existingPOI, duration, horizontalScale, 1f, Color.white);
        }

        /// <summary>
        /// Show a light beacon over the specified POI.
        /// </summary>
        public GameObject POIShowBeacon(CompassProPOI existingPOI, float duration, float horizontalScale, float intensity, Color tintColor) {
            Transform beacon = existingPOI.transform.Find("POIBeacon");
            if (beacon != null)
                return beacon.gameObject;

            GameObject beaconObj = Instantiate(Resources.Load<GameObject>("CNPro/Prefabs/POIBeacon"));
            beaconObj.name = "POIBeacon";
            beacon = beaconObj.transform;
            beacon.localScale = new Vector3(beacon.localScale.x * horizontalScale, beacon.localScale.y, beacon.localScale.z);
            beacon.position = existingPOI.transform.position + new Vector3(0, beacon.localScale.y * 0.5f, 0);
            beacon.SetParent(existingPOI.transform, true);
            BeaconAnimator anim = beacon.GetComponent<BeaconAnimator>();
            anim.duration = duration;
            anim.tintColor = tintColor;
            anim.intensity = intensity;

            if (audioSource != null) {
                if (existingPOI.beaconAudioClip != null) {
                    audioSource.PlayOneShot(existingPOI.beaconAudioClip);
                } else if (_beaconDefaultAudioClip != null) {
                    audioSource.PlayOneShot(_beaconDefaultAudioClip);
                }
            }

            return beaconObj;
        }


        /// <summary>
        /// Show a light beacon over the specified POI.
        /// </summary>
        public void POIShowBeacon(Vector3 position, float duration, float horizontalScale, float intensity, Color tintColor) {
            string beaconName = "POIBeacon " + position;
            GameObject beaconObj = GameObject.Find(beaconName);
            if (beaconObj != null)
                return;

            beaconObj = Instantiate(Resources.Load<GameObject>("CNPro/Prefabs/POIBeacon"));
            beaconObj.name = beaconName;
            Transform beacon = beaconObj.transform;
            beacon.localScale = new Vector3(beacon.localScale.x * horizontalScale, beacon.localScale.y, beacon.localScale.z);
            beacon.position = position + Misc.Vector3up * beacon.transform.localScale.y * 0.5f;
            BeaconAnimator anim = beacon.gameObject.GetComponent<BeaconAnimator>();
            anim.duration = duration;
            anim.tintColor = tintColor;
            anim.intensity = intensity;

            if (audioSource != null) {
                if (_beaconDefaultAudioClip != null) {
                    audioSource.PlayOneShot(_beaconDefaultAudioClip);
                }
            }
        }



        /// <summary>
        /// Show a light beacon over all non-visited POIs for duration in seconds and with optional custom horizontal scale for the bright cylinder.
        /// </summary>
        public void POIShowBeacon(float duration, float horizontalScale = 1f) {
            POIShowBeacon(duration, horizontalScale, 1f, Color.white);
        }

        /// <summary>
        /// Show a light beacon over all non-visited POIs for duration in seconds and with optional custom horizontal scale for the bright cylinder.
        /// </summary>
        public void POIShowBeacon(float duration, float horizontalScale, float intensity, Color tintColor) {
            for (int k = 0; k < pois.Count; k++) {
                CompassProPOI poi = pois[k];
                if (poi == null || poi.isVisited || !poi.isVisible)
                    continue;
                POIShowBeacon(poi, duration, horizontalScale, intensity, tintColor);
            }
        }


        /// <summary>
        /// Initiates a fade in effect with duration in seconds.
        /// </summary>
        public void FadeIn(float duration) {
            fadeDuration = duration;
            fadeStartTime = Time.time;
            prevAlpha = canvasGroup.alpha;
            alpha = 1f;
        }

        /// <summary>
        /// Initiates a fade out effect with duration in seconds.
        /// </summary>
        public void FadeOut(float duration) {
            fadeDuration = duration;
            fadeStartTime = Time.time;
            prevAlpha = canvasGroup.alpha;
            alpha = 0f;
        }


        public void ShowAnimatedText(string text) {
            StartCoroutine(AnimateDiscoverText(text));
        }


        public Canvas canvas {
            get {
                return _canvas;
            }
        }


        /// <summary>
        /// Returns the degrees for the current compass orientation
        /// </summary>
        public float degrees {
            get {
                float angles = _cameraMain.transform.eulerAngles.y;
                return (angles + 360f - _northDegrees) % 360;
            }
        }

        /// <summary>
        /// Returns a list of visited POIs
        /// </summary>
        /// <param name="pois"></param>
        public void POIGetVisited(List<CompassProPOI> pois) {
            pois.Clear();
            foreach (CompassProPOI poi in this.pois) {
                if (poi != null && poi.isVisited) {
                    pois.Add(poi);
                }
            }
        }


        /// <summary>
        /// Returns a list of visited POIs
        /// </summary>
        /// <param name="pois"></param>
        public void POIGetUnvisited(List<CompassProPOI> pois) {
            pois.Clear();
            foreach (CompassProPOI poi in this.pois) {
                if (poi != null && !poi.isVisited) {
                    pois.Add(poi);
                }
            }
        }


        /// <summary>
        /// Returns a list of visited POIs
        /// </summary>
        /// <param name="pois"></param>
        public void POIGetAll(List<CompassProPOI> pois) {
            pois.Clear();
            foreach (CompassProPOI poi in this.pois) {
                if (poi != null) {
                    pois.Add(poi);
                }
            }
        }

        #endregion


    }

}



