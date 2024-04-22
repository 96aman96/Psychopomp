using UnityEngine;
using UnityEditor;

namespace CompassNavigatorPro {
    [CustomEditor(typeof(CompassPro))]
    public class CompassNavigatorProInspector : Editor {

        CompassPro _compass;
        Texture2D _headerTexture;
        static GUIStyle sectionHeaderStyle, titleLabelStyle;
        static Color titleColor;
        bool expandGeneralSettings, expandCompassBarSettings, expandMiniMapSettings, expandOnScreenIndicatorSettings, expandOffScreenIndicatorSettings;
        bool expandCompassStyleSettings, expandCompassPOISettings, expandTitleSettings, expandTextSettings, expandMiniMapStyleSettings, expandMiniMapPOISettings, expandMiniMapRenderingSettings, expandMiniMapFullScreenModeSettings, expandMiniMapFogOfWarSettings, expandMiniMapInteractionSettings;
        bool updateTitleInEditMode, updateTextInEditMode;
        bool showMiniMapTextures, showCompassBarTextures;

        SerializedProperty cameraMain, follow;
        SerializedProperty showCompassBar, style, compassBackSprite, compassTintColor, bendAmount, width, height, verticalPosition, horizontalPosition, endCapsWidth, edgeFadeOut, edgeFadeOutWidth, edgeFadeOutStart, edgeFadeOutText;
        SerializedProperty alpha, alwaysVisibleInEditMode, autoHide, fadeDuration;
        SerializedProperty worldMappingMode, use3Ddistance, sameAltitudeThreshold, northDegrees, showCardinalPoints, cardinalScale, showOrdinalPoints, ordinalScale, cardinalPointsVerticalOffset;
        SerializedProperty showHalfWinds, halfWindsInterval, halfWindsHeight, halfWindsWidth, halfWindsTintColor;
        SerializedProperty showDistance, showDistanceFormat, dontDestroyOnLoad, updateMode, updateIntervalFrameCount, updateIntervalTime;
        SerializedProperty titleFont, titleFontTMP, titleVerticalPosition, titleScale, titleShadowEnabled, titleShowDistance, titleShowDistanceFormat, titleMinPOIDistance;
        SerializedProperty textRevealEnabled, textFont, textVerticalPosition, textScale, textRevealDuration, textLetterSpacing, textRevealLetterDelay, textDuration, textFadeOutDuration, textShadowEnabled;
        SerializedProperty compassIconPrefab, minIconSize, maxIconSize, visibleMaxDistance, visibleMinDistance, nearDistance, visitedDistance, scaleInDuration, focusedPOI, labelHotZone, visitedDefaultAudioClip, beaconDefaultAudioClip, heartbeatDefaultAudioClip;

        SerializedProperty showOnScreenIndicators, onScreenIndicatorPrefab, onScreenIndicatorScale, onScreenIndicatorNearFadeDistance, onScreenIndicatorNearFadeMin, onScreenIndicatorAlpha, onScreenIndicatorShowDistance, onScreenIndicatorShowDistanceFormat, onScreenIndicatorShowTitle;

        SerializedProperty showOffScreenIndicators, offScreenIndicatorScale, offScreenIndicatorMargin, offScreenIndicatorAlpha, offScreenIndicatorAvoidOverlap, offScreenIndicatorOverlapDistance;

        SerializedProperty showMiniMap, miniMapFullScreenState, miniMapKeepStraight, miniMapOrientation, miniMapAllowUserDrag, miniMapFullScreenAllowUserDrag, miniMapDragMaxDistance, miniMapContents, miniMapContentsTexture, miniMapContentsTextureAllowRotation;
        SerializedProperty miniMapWorldCenter, miniMapWorldSize, miniMapStyle, miniMapClampBorderCircular, miniMapFullScreenClampBorderCircular, miniMapVignette;
        SerializedProperty miniMapVignetteColor, miniMapBackgroundColor, miniMapBackgroundOpaque, miniMapRadarRingsDistance, miniMapRadarRingsWidth, miniMapRadarRingsColor, miniMapRadarInfoDisplay;
        SerializedProperty miniMapRadarPulseEnabled, miniMapRadarPulseAnimationPreset, miniMapRadarPulseSpeed, miniMapRadarPulseFallOff, miniMapRadarPulseFrequency, miniMapRadarPulseOpacity;
        SerializedProperty miniMapPositionAndSize, miniMapLocation, miniMapScreenPositionOffset, miniMapSize, miniMapBorderTexture, miniMapMaskSprite;
        SerializedProperty miniMapAlpha, miniMapContrast, miniMapBrightness, miniMapLutTexture, miniMapLutIntensity, miniMapTintColor;
        SerializedProperty miniMapIconPrefab, miniMapVisibleMaxDistance, miniMapIconSize, miniMapIconPositionShift;
        SerializedProperty miniMapShowPlayerIcon, miniMapPlayerIconSize, miniMapPlayerIconSprite, miniMapPlayerIconColor;
        SerializedProperty miniMapShowCardinals, miniMapCardinalsSize, miniMapCardinalsSprite, miniMapCardinalsColor;
        SerializedProperty miniMapShowViewCone, miniMapViewConeFoVSource, miniMapViewConeFoV, miniMapViewConeDistance, miniMapViewConeFallOff, miniMapViewConeColor, miniMapShowViewConeOutline, miniMapViewConeOutlineColor;
        SerializedProperty miniMapClampBorder, miniMapFullScreenClampBorder, miniMapIconEvents, miniMapShowZoomInOutButtons, miniMapShowMaximizeButton, miniMapButtonsScale;
        SerializedProperty miniMapResolution, miniMapLayerMask, miniMapEnableShadows, miniMapCameraMode, miniMapCameraHeightVSFollow, miniMapClampToWorldEdges, miniMapCameraDepth;
        SerializedProperty miniMapCaptureSize, miniMapCameraSnapshotFrequency, miniMapSnapshotInterval, miniMapSnapshotDistance;
        SerializedProperty miniMapZoomMin, miniMapZoomMax, miniMapCameraMinAltitude, miniMapCameraMaxAltitude, miniMapZoomLevel;
        SerializedProperty miniMapCameraTilt, miniMapFullScreenContents, miniMapFullScreenContentsTexture, miniMapFullScreenResolution;
        SerializedProperty miniMapFullScreenSize, miniMapFullScreenPlaceholder, miniMapKeepAspectRatio, miniMapFullScreenWorldCenterFollows, miniMapFullScreenClampToWorldEdges;
        SerializedProperty miniMapFullScreenWorldCenter, miniMapFullScreenWorldSize, miniMapFullScreenZoomLevel, miniMapFullScreenFreezeCamera;
        SerializedProperty miniMapFullScreenStyle, miniMapBorderTextureFullScreenMode, miniMapMaskSpriteFullScreenMode;
        SerializedProperty miniMapIconCircleAnimationDuration;

        SerializedProperty fogOfWarEnabled, fogOfWarCenter, fogOfWarSize, fogOfWarTextureSize, fogOfWarDefaultAlpha, fogOfWarColor, fogOfWarAutoClear, fogOfWarAutoClearRadius;

        void OnEnable() {

            titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);

            cameraMain = serializedObject.FindProperty("_cameraMain");
            follow = serializedObject.FindProperty("_follow");
            showCompassBar = serializedObject.FindProperty("_showCompassBar");
            style = serializedObject.FindProperty("_style");
            compassBackSprite = serializedObject.FindProperty("_compassBackSprite");
            compassTintColor = serializedObject.FindProperty("_compassTintColor");
            bendAmount = serializedObject.FindProperty("_bendAmount");
            width = serializedObject.FindProperty("_width");
            height = serializedObject.FindProperty("_height");
            verticalPosition = serializedObject.FindProperty("_verticalPosition");
            horizontalPosition = serializedObject.FindProperty("_horizontalPosition");
            endCapsWidth = serializedObject.FindProperty("_endCapsWidth");
            edgeFadeOut = serializedObject.FindProperty("_edgeFadeOut");
            edgeFadeOutStart = serializedObject.FindProperty("_edgeFadeOutStart");
            edgeFadeOutWidth = serializedObject.FindProperty("_edgeFadeOutWidth");
            edgeFadeOutText = serializedObject.FindProperty("_edgeFadeOutText");
            alpha = serializedObject.FindProperty("_alpha");
            alwaysVisibleInEditMode = serializedObject.FindProperty("_alwaysVisibleInEditMode");
            autoHide = serializedObject.FindProperty("_autoHide");
            fadeDuration = serializedObject.FindProperty("_fadeDuration");
            worldMappingMode = serializedObject.FindProperty("_worldMappingMode");
            sameAltitudeThreshold = serializedObject.FindProperty("_sameAltitudeThreshold");
            use3Ddistance = serializedObject.FindProperty("_use3Ddistance");
            northDegrees = serializedObject.FindProperty("_northDegrees");
            showCardinalPoints = serializedObject.FindProperty("_showCardinalPoints");
            cardinalScale = serializedObject.FindProperty("_cardinalScale");
            showOrdinalPoints = serializedObject.FindProperty("_showOrdinalPoints");
            ordinalScale = serializedObject.FindProperty("_ordinalScale");
            cardinalPointsVerticalOffset = serializedObject.FindProperty("_cardinalPointsVerticalOffset");
            showHalfWinds = serializedObject.FindProperty("_showHalfWinds");
            halfWindsInterval = serializedObject.FindProperty("_halfWindsInterval");
            halfWindsHeight = serializedObject.FindProperty("_halfWindsHeight");
            halfWindsWidth = serializedObject.FindProperty("_halfWindsWidth");
            halfWindsTintColor = serializedObject.FindProperty("_halfWindsTintColor");
            showDistance = serializedObject.FindProperty("_showDistance");
            showDistanceFormat = serializedObject.FindProperty("_showDistanceFormat");
            fadeDuration = serializedObject.FindProperty("_fadeDuration");
            dontDestroyOnLoad = serializedObject.FindProperty("_dontDestroyOnLoad");
            updateMode = serializedObject.FindProperty("_updateMode");
            updateIntervalFrameCount = serializedObject.FindProperty("_updateIntervalFrameCount");
            updateIntervalTime = serializedObject.FindProperty("_updateIntervalTime");

            titleFont = serializedObject.FindProperty("_titleFont");
            titleFontTMP = serializedObject.FindProperty("_titleFontTMP");
            titleVerticalPosition = serializedObject.FindProperty("_titleVerticalPosition");
            titleScale = serializedObject.FindProperty("_titleScale");
            titleShadowEnabled = serializedObject.FindProperty("_titleShadowEnabled");
            titleShowDistance = serializedObject.FindProperty("_titleShowDistance");
            titleShowDistanceFormat = serializedObject.FindProperty("_titleShowDistanceFormat");

            textRevealEnabled = serializedObject.FindProperty("_textRevealEnabled");
            textFont = serializedObject.FindProperty("_textFont");
            textVerticalPosition = serializedObject.FindProperty("_textVerticalPosition");
            textScale = serializedObject.FindProperty("_textScale");
            textRevealDuration = serializedObject.FindProperty("_textRevealDuration");
            textLetterSpacing = serializedObject.FindProperty("_textLetterSpacing");
            textRevealLetterDelay = serializedObject.FindProperty("_textRevealLetterDelay");
            textRevealLetterDelay = serializedObject.FindProperty("_textRevealLetterDelay");
            textDuration = serializedObject.FindProperty("_textDuration");
            textFadeOutDuration = serializedObject.FindProperty("_textFadeOutDuration");
            textShadowEnabled = serializedObject.FindProperty("_textShadowEnabled");

            compassIconPrefab = serializedObject.FindProperty("_compassIconPrefab");
            minIconSize = serializedObject.FindProperty("_minIconSize");
            maxIconSize = serializedObject.FindProperty("_maxIconSize");
            visibleMaxDistance = serializedObject.FindProperty("_visibleMaxDistance");
            visibleMinDistance = serializedObject.FindProperty("_visibleMinDistance");
            titleMinPOIDistance = serializedObject.FindProperty("_titleMinPOIDistance");
            nearDistance = serializedObject.FindProperty("_nearDistance");
            visitedDistance = serializedObject.FindProperty("_visitedDistance");
            focusedPOI = serializedObject.FindProperty("_focusedPOI");
            scaleInDuration = serializedObject.FindProperty("_scaleInDuration");
            labelHotZone = serializedObject.FindProperty("_labelHotZone");
            visitedDefaultAudioClip = serializedObject.FindProperty("_visitedDefaultAudioClip");
            beaconDefaultAudioClip = serializedObject.FindProperty("_beaconDefaultAudioClip");
            heartbeatDefaultAudioClip = serializedObject.FindProperty("_heartbeatDefaultAudioClip");

            showOnScreenIndicators = serializedObject.FindProperty("_showOnScreenIndicators");
            onScreenIndicatorPrefab = serializedObject.FindProperty("_onScreenIndicatorPrefab");
            onScreenIndicatorScale = serializedObject.FindProperty("_onScreenIndicatorScale");
            onScreenIndicatorNearFadeDistance = serializedObject.FindProperty("_onScreenIndicatorNearFadeDistance");
            onScreenIndicatorNearFadeMin = serializedObject.FindProperty("_onScreenIndicatorNearFadeMin");
            onScreenIndicatorAlpha = serializedObject.FindProperty("_onScreenIndicatorAlpha");
            onScreenIndicatorShowDistance = serializedObject.FindProperty("_onScreenIndicatorShowDistance");
            onScreenIndicatorShowDistanceFormat = serializedObject.FindProperty("_onScreenIndicatorShowDistanceFormat");
            onScreenIndicatorShowTitle = serializedObject.FindProperty("_onScreenIndicatorShowTitle");

            showOffScreenIndicators = serializedObject.FindProperty("_showOffScreenIndicators");
            offScreenIndicatorScale = serializedObject.FindProperty("_offScreenIndicatorScale");
            offScreenIndicatorMargin = serializedObject.FindProperty("_offScreenIndicatorMargin");
            offScreenIndicatorAlpha = serializedObject.FindProperty("_offScreenIndicatorAlpha");
            offScreenIndicatorAvoidOverlap = serializedObject.FindProperty("_offScreenIndicatorAvoidOverlap");
            offScreenIndicatorOverlapDistance = serializedObject.FindProperty("_offScreenIndicatorOverlapDistance");

            showMiniMap = serializedObject.FindProperty("_showMiniMap");
            miniMapKeepStraight = serializedObject.FindProperty("_miniMapKeepStraight");
            miniMapOrientation = serializedObject.FindProperty("_miniMapOrientation");
            miniMapFullScreenState = serializedObject.FindProperty("_miniMapFullScreenState");
            miniMapAllowUserDrag = serializedObject.FindProperty("_miniMapAllowUserDrag");
            miniMapFullScreenAllowUserDrag = serializedObject.FindProperty("_miniMapFullScreenAllowUserDrag");
            miniMapDragMaxDistance = serializedObject.FindProperty("_miniMapDragMaxDistance");
            miniMapContents = serializedObject.FindProperty("_miniMapContents");
            miniMapContentsTexture = serializedObject.FindProperty("_miniMapContentsTexture");
            miniMapContentsTextureAllowRotation = serializedObject.FindProperty("_miniMapContentsTextureAllowRotation");
            miniMapWorldCenter = serializedObject.FindProperty("_miniMapWorldCenter");
            miniMapWorldSize = serializedObject.FindProperty("_miniMapWorldSize");
            miniMapContentsTexture = serializedObject.FindProperty("_miniMapContentsTexture");
            miniMapStyle = serializedObject.FindProperty("_miniMapStyle");
            miniMapClampBorderCircular = serializedObject.FindProperty("_miniMapClampBorderCircular");
            miniMapVignette = serializedObject.FindProperty("_miniMapVignette");
            miniMapVignetteColor = serializedObject.FindProperty("_miniMapVignetteColor");
            miniMapBackgroundColor = serializedObject.FindProperty("_miniMapBackgroundColor");
            miniMapBackgroundOpaque = serializedObject.FindProperty("_miniMapBackgroundOpaque");
            miniMapRadarRingsDistance = serializedObject.FindProperty("_miniMapRadarRingsDistance");
            miniMapRadarRingsWidth = serializedObject.FindProperty("_miniMapRadarRingsWidth");
            miniMapRadarRingsColor = serializedObject.FindProperty("_miniMapRadarRingsColor");
            miniMapRadarInfoDisplay = serializedObject.FindProperty("_miniMapRadarInfoDisplay");
            miniMapRadarPulseEnabled = serializedObject.FindProperty("_miniMapRadarPulseEnabled");
            miniMapRadarPulseAnimationPreset = serializedObject.FindProperty("_miniMapRadarPulseAnimationPreset");
            miniMapRadarPulseSpeed = serializedObject.FindProperty("_miniMapRadarPulseSpeed");
            miniMapRadarPulseFallOff = serializedObject.FindProperty("_miniMapRadarPulseFallOff");
            miniMapRadarPulseFrequency = serializedObject.FindProperty("_miniMapRadarPulseFrequency");
            miniMapRadarPulseOpacity = serializedObject.FindProperty("_miniMapRadarPulseOpacity");

            miniMapPositionAndSize = serializedObject.FindProperty("_miniMapPositionAndSize");
            miniMapLocation = serializedObject.FindProperty("_miniMapLocation");
            miniMapScreenPositionOffset = serializedObject.FindProperty("_miniMapScreenPositionOffset");
            miniMapSize = serializedObject.FindProperty("_miniMapSize");
            miniMapBorderTexture = serializedObject.FindProperty("_miniMapBorderTexture");
            miniMapMaskSprite = serializedObject.FindProperty("_miniMapMaskSprite");
            miniMapAlpha = serializedObject.FindProperty("_miniMapAlpha");
            miniMapContrast = serializedObject.FindProperty("_miniMapContrast");
            miniMapBrightness = serializedObject.FindProperty("_miniMapBrightness");
            miniMapLutTexture = serializedObject.FindProperty("_miniMapLutTexture");
            miniMapLutIntensity = serializedObject.FindProperty("_miniMapLutIntensity");
            miniMapTintColor = serializedObject.FindProperty("_miniMapTintColor");
            miniMapIconPrefab = serializedObject.FindProperty("_miniMapIconPrefab");
            miniMapVisibleMaxDistance = serializedObject.FindProperty("_miniMapVisibleMaxDistance");
            miniMapIconSize = serializedObject.FindProperty("_miniMapIconSize");
            miniMapIconPositionShift = serializedObject.FindProperty("_miniMapIconPositionShift");
            miniMapShowPlayerIcon = serializedObject.FindProperty("_miniMapShowPlayerIcon");
            miniMapPlayerIconSize = serializedObject.FindProperty("_miniMapPlayerIconSize");
            miniMapPlayerIconSprite = serializedObject.FindProperty("_miniMapPlayerIconSprite");
            miniMapPlayerIconColor = serializedObject.FindProperty("_miniMapPlayerIconColor");
            miniMapShowCardinals = serializedObject.FindProperty("_miniMapShowCardinals");
            miniMapCardinalsSize = serializedObject.FindProperty("_miniMapCardinalsSize");
            miniMapCardinalsSprite = serializedObject.FindProperty("_miniMapCardinalsSprite");
            miniMapCardinalsColor = serializedObject.FindProperty("_miniMapCardinalsColor");
            miniMapShowViewCone = serializedObject.FindProperty("_miniMapShowViewCone");
            miniMapViewConeDistance = serializedObject.FindProperty("_miniMapViewConeDistance");
            miniMapViewConeFallOff = serializedObject.FindProperty("_miniMapViewConeFallOff");
            miniMapViewConeFoVSource = serializedObject.FindProperty("_miniMapViewConeFoVSource");
            miniMapViewConeFoV = serializedObject.FindProperty("_miniMapViewConeFoV");
            miniMapViewConeColor = serializedObject.FindProperty("_miniMapViewConeColor");
            miniMapShowViewConeOutline = serializedObject.FindProperty("_miniMapShowViewConeOutline");
            miniMapViewConeOutlineColor = serializedObject.FindProperty("_miniMapViewConeOutlineColor");
            miniMapClampBorder = serializedObject.FindProperty("_miniMapClampBorder");
            miniMapIconEvents = serializedObject.FindProperty("_miniMapIconEvents");
            miniMapShowZoomInOutButtons = serializedObject.FindProperty("_miniMapShowZoomInOutButtons");
            miniMapShowMaximizeButton = serializedObject.FindProperty("_miniMapShowMaximizeButton");
            miniMapButtonsScale = serializedObject.FindProperty("_miniMapButtonsScale");
            miniMapResolution = serializedObject.FindProperty("_miniMapResolution");
            miniMapLayerMask = serializedObject.FindProperty("_miniMapLayerMask");
            miniMapCameraMode = serializedObject.FindProperty("_miniMapCameraMode");
            miniMapEnableShadows = serializedObject.FindProperty("_miniMapEnableShadows");
            miniMapCameraHeightVSFollow = serializedObject.FindProperty("_miniMapCameraHeightVSFollow");
            miniMapCameraDepth = serializedObject.FindProperty("_miniMapCameraDepth");
            miniMapCaptureSize = serializedObject.FindProperty("_miniMapCaptureSize");
            miniMapCameraSnapshotFrequency = serializedObject.FindProperty("_miniMapCameraSnapshotFrequency");
            miniMapSnapshotInterval = serializedObject.FindProperty("_miniMapSnapshotInterval");
            miniMapSnapshotDistance = serializedObject.FindProperty("_miniMapSnapshotDistance");
            miniMapZoomMin = serializedObject.FindProperty("_miniMapZoomMin");
            miniMapZoomMax = serializedObject.FindProperty("_miniMapZoomMax");
            miniMapCameraMinAltitude = serializedObject.FindProperty("_miniMapCameraMinAltitude");
            miniMapCameraMaxAltitude = serializedObject.FindProperty("_miniMapCameraMaxAltitude");
            miniMapCameraTilt = serializedObject.FindProperty("_miniMapCameraTilt");
            miniMapZoomLevel = serializedObject.FindProperty("_miniMapZoomLevel");
            miniMapClampToWorldEdges = serializedObject.FindProperty("miniMapClampToWorldEdges");
            miniMapFullScreenContents = serializedObject.FindProperty("_miniMapFullScreenContents");
            miniMapFullScreenContentsTexture = serializedObject.FindProperty("_miniMapFullScreenContentsTexture");
            miniMapFullScreenResolution = serializedObject.FindProperty("_miniMapFullScreenResolution");
            miniMapFullScreenSize = serializedObject.FindProperty("_miniMapFullScreenSize");
            miniMapFullScreenPlaceholder = serializedObject.FindProperty("_miniMapFullScreenPlaceholder");
            miniMapKeepAspectRatio = serializedObject.FindProperty("_miniMapKeepAspectRatio");
            miniMapFullScreenWorldCenter = serializedObject.FindProperty("_miniMapFullScreenWorldCenter");
            miniMapFullScreenWorldCenterFollows = serializedObject.FindProperty("miniMapFullScreenWorldCenterFollows");
            miniMapFullScreenClampToWorldEdges = serializedObject.FindProperty("miniMapFullScreenClampToWorldEdges");
            miniMapFullScreenWorldSize = serializedObject.FindProperty("_miniMapFullScreenWorldSize");
            miniMapFullScreenZoomLevel = serializedObject.FindProperty("_miniMapFullScreenZoomLevel");
            miniMapFullScreenFreezeCamera = serializedObject.FindProperty("miniMapFullScreenFreezeCamera");
            miniMapFullScreenClampBorderCircular = serializedObject.FindProperty("_miniMapFullScreenClampBorderCircular");
            miniMapFullScreenClampBorder = serializedObject.FindProperty("_miniMapFullScreenClampBorder");
            miniMapFullScreenStyle = serializedObject.FindProperty("_miniMapFullScreenStyle");
            miniMapBorderTextureFullScreenMode = serializedObject.FindProperty("_miniMapBorderTextureFullScreenMode");
            miniMapMaskSpriteFullScreenMode = serializedObject.FindProperty("_miniMapMaskSpriteFullScreenMode");
            miniMapIconCircleAnimationDuration = serializedObject.FindProperty("_miniMapIconCircleAnimationDuration");

            fogOfWarEnabled = serializedObject.FindProperty("_fogOfWarEnabled");
            fogOfWarCenter = serializedObject.FindProperty("_fogOfWarCenter");
            fogOfWarSize = serializedObject.FindProperty("_fogOfWarSize");
            fogOfWarTextureSize = serializedObject.FindProperty("_fogOfWarTextureSize");
            fogOfWarDefaultAlpha = serializedObject.FindProperty("_fogOfWarDefaultAlpha");
            fogOfWarColor = serializedObject.FindProperty("_fogOfWarColor");
            fogOfWarAutoClear = serializedObject.FindProperty("_fogOfWarAutoClear");
            fogOfWarAutoClearRadius = serializedObject.FindProperty("_fogOfWarAutoClearRadius");

            _compass = (CompassPro)target;
            _headerTexture = Resources.Load<Texture2D>("CNPro/CompassNavigatorProHeader");

            expandGeneralSettings = EditorPrefs.GetBool("CNProGeneralSettings", true);
            expandCompassBarSettings = EditorPrefs.GetBool("CNProCompassBarSettings", true);
            expandMiniMapSettings = EditorPrefs.GetBool("CNProMiniMapSettings", true);
            expandOnScreenIndicatorSettings = EditorPrefs.GetBool("CNProOnScreenIndicatorSettings", true);
            expandOffScreenIndicatorSettings = EditorPrefs.GetBool("CNProOffScreenIndicatorSettings", true);

            expandCompassStyleSettings = EditorPrefs.GetBool("CNProCompassStyleSettings", false);
            expandCompassPOISettings = EditorPrefs.GetBool("CNProCompassPOISettings", false);
            expandTitleSettings = EditorPrefs.GetBool("CNProTitleSettings", false);
            expandTextSettings = EditorPrefs.GetBool("CNProTextSettings", false);
            expandMiniMapStyleSettings = EditorPrefs.GetBool("CNProMiniMapStyleSettings", true);
            expandMiniMapPOISettings = EditorPrefs.GetBool("expandMiniMapPOISettings", false);
            expandMiniMapRenderingSettings = EditorPrefs.GetBool("expandMiniMapRenderingSettings", false);
            expandMiniMapFullScreenModeSettings = EditorPrefs.GetBool("expandMiniMapFullScreenModeSettings", false);
            expandMiniMapFogOfWarSettings = EditorPrefs.GetBool("expandMiniMapFogOfWarSettings", false);
            expandMiniMapInteractionSettings = EditorPrefs.GetBool("expandMiniMapInteractionSettings", false);
        }

        private void OnDisable() {
            EditorPrefs.SetBool("CNProGeneralSettings", expandGeneralSettings);
            EditorPrefs.SetBool("CNProCompassBarSettings", expandCompassBarSettings);
            EditorPrefs.SetBool("CNProMiniMapSettings", expandMiniMapSettings);
            EditorPrefs.SetBool("CNProOnScreenIndicatorSettings", expandOnScreenIndicatorSettings);
            EditorPrefs.SetBool("CNProOffScreenIndicatorSettings", expandOffScreenIndicatorSettings);

            EditorPrefs.SetBool("CNProCompassStyleSettings", expandCompassStyleSettings);
            EditorPrefs.SetBool("CNProTitleSettings", expandTitleSettings);
            EditorPrefs.SetBool("CNProTextSettings", expandTextSettings);
            EditorPrefs.SetBool("CNProCompassPOISettings", expandCompassPOISettings);
            EditorPrefs.SetBool("CNProMiniMapStyleSettings", expandMiniMapStyleSettings);
            EditorPrefs.SetBool("expandMiniMapPOISettings", expandMiniMapPOISettings);
            EditorPrefs.SetBool("expandMiniMapRenderingSettings", expandMiniMapRenderingSettings);
            EditorPrefs.SetBool("expandMiniMapFullScreenModeSettings", expandMiniMapFullScreenModeSettings);
            EditorPrefs.SetBool("expandMiniMapFogOfWarSettings", expandMiniMapFogOfWarSettings);
            EditorPrefs.SetBool("expandMiniMapInteractionSettings", expandMiniMapInteractionSettings);
        }

        public override void OnInspectorGUI() {
            if (_compass == null)
                return;

            if (sectionHeaderStyle == null) {
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldout);
                sectionHeaderStyle.fontStyle = FontStyle.Bold;
            }

            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(_headerTexture, GUILayout.ExpandWidth(true));
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            EditorGUILayout.Separator();

            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Main Features", EditorStyles.boldLabel);
            if (GUILayout.Button("Help", GUILayout.Width(50)))
                Application.OpenURL("https://kronnect.com/guides-category/compass-navigator-pro-2/");
            if (GUILayout.Button("About", GUILayout.Width(60))) {
                CompassProAbout.ShowAboutWindow();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(showCompassBar, new GUIContent("Compass Bar"));
            EditorGUILayout.PropertyField(showMiniMap, new GUIContent("MiniMap"));
            EditorGUILayout.PropertyField(showOnScreenIndicators, new GUIContent("On-Screen Indicators"));
            EditorGUILayout.PropertyField(showOffScreenIndicators, new GUIContent("Off-Screen Indicators"));
            EditorGUILayout.Separator();
            expandGeneralSettings = DrawSectionTitle("General Settings", expandGeneralSettings);
            if (expandGeneralSettings) {
                EditorGUILayout.PropertyField(cameraMain, new GUIContent("Camera"));
                EditorGUILayout.PropertyField(follow, new GUIContent("Follow"));
                if (showCompassBar.boolValue || showMiniMap.boolValue) {
                    EditorGUILayout.PropertyField(focusedPOI);
                    EditorGUILayout.PropertyField(use3Ddistance, new GUIContent("Use 3D Distance"));
                    EditorGUILayout.PropertyField(sameAltitudeThreshold);
                    EditorGUILayout.PropertyField(northDegrees, new GUIContent("North Position"));
                    EditorGUILayout.PropertyField(updateMode, new GUIContent("Idle Update Mode"));
                    EditorGUI.indentLevel++;
                    switch (updateMode.intValue) {
                        case (int)UpdateMode.NumberOfFrames:
                            EditorGUILayout.PropertyField(updateIntervalFrameCount, new GUIContent("Frame Count"));
                            break;
                        case (int)UpdateMode.Time:
                            EditorGUILayout.PropertyField(updateIntervalTime, new GUIContent("Seconds"));
                            break;
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(dontDestroyOnLoad);
                EditorGUILayout.Separator();
            }

            if (showCompassBar.boolValue) {

                expandCompassBarSettings = DrawSectionTitle("Compass Bar Settings", expandCompassBarSettings);
                if (expandCompassBarSettings) {

                    EditorGUILayout.PropertyField(worldMappingMode);
                    EditorGUILayout.Separator();

                    expandCompassStyleSettings = DrawExpandableSectionTitle(expandCompassStyleSettings, "Style & Design");
                    if (expandCompassStyleSettings) {

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(style, new GUIContent("Style"));
                        if (GUILayout.Button("?", GUILayout.Width(30))) {
                            showCompassBarTextures = !showCompassBarTextures;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel++;
                        if (style.intValue == (int)CompassStyle.Custom) {
                            EditorGUILayout.PropertyField(compassBackSprite, new GUIContent("Sprite"));
                            EditorGUILayout.HelpBox("Please follow the guidelines in the manual to add your own art.", MessageType.Info);
                        } else {
                            if (showCompassBarTextures) {
                                GUI.enabled = false;
                                EditorGUILayout.ObjectField(new GUIContent("Sprite"), _compass.GetCompassBarSprite(), typeof(Sprite), false);
                                GUI.enabled = true;
                            }
                        }
                        EditorGUI.indentLevel--;
                        EditorGUILayout.PropertyField(compassTintColor, new GUIContent("Tint Color"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(bendAmount);
                        if (GUILayout.Button("Disable", GUILayout.Width(80))) {
                            bendAmount.floatValue = 0;
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.PropertyField(width);
                        EditorGUILayout.PropertyField(height);
                        EditorGUILayout.PropertyField(verticalPosition);
                        EditorGUILayout.PropertyField(horizontalPosition);
                        EditorGUILayout.PropertyField(endCapsWidth);
                        EditorGUILayout.PropertyField(edgeFadeOut);
                        if (edgeFadeOut.boolValue) {
                            EditorGUI.indentLevel++; ;
                            EditorGUILayout.PropertyField(edgeFadeOutWidth, new GUIContent("Fade Width"));
                            EditorGUILayout.PropertyField(edgeFadeOutStart, new GUIContent("Fade Start"));
                            EditorGUILayout.PropertyField(edgeFadeOutText, new GUIContent("Fade Title & Text"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(alpha);
                        if (alpha.floatValue > 0) {
                            if (GUILayout.Button("Hide", GUILayout.Width(60))) {
                                alpha.floatValue = 0;
                            }
                        } else {
                            if (GUILayout.Button("Show", GUILayout.Width(60))) {
                                alpha.floatValue = 1;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(alwaysVisibleInEditMode, new GUIContent("Visible In Edit Mode"));
                        EditorGUILayout.PropertyField(autoHide, new GUIContent("Auto Hide If Empty"));
                        EditorGUILayout.PropertyField(fadeDuration);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.PropertyField(showCardinalPoints);
                        if (showCardinalPoints.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(cardinalScale, new GUIContent("Scale"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(showOrdinalPoints);
                        if (showOrdinalPoints.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(ordinalScale, new GUIContent("Scale"));
                            EditorGUI.indentLevel--;
                        }
                        if (showCardinalPoints.boolValue || showOrdinalPoints.boolValue) {
                            EditorGUILayout.PropertyField(cardinalPointsVerticalOffset, new GUIContent("Cardinals Vertical Offset"));
                        }

                        EditorGUILayout.PropertyField(showHalfWinds, new GUIContent("Show Ticks"));
                        if (showHalfWinds.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(halfWindsInterval, new GUIContent("Interval (degrees)"));
                            EditorGUILayout.PropertyField(halfWindsHeight, new GUIContent("Height"));
                            EditorGUILayout.PropertyField(halfWindsWidth, new GUIContent("Width"));
                            EditorGUILayout.PropertyField(halfWindsTintColor, new GUIContent("Color"));
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUILayout.Separator();

                    expandCompassPOISettings = DrawExpandableSectionTitle(expandCompassPOISettings, "POI Settings");
                    if (expandCompassPOISettings) {
                        EditorGUILayout.PropertyField(compassIconPrefab, new GUIContent("Icon Prefab"));
                        EditorGUILayout.BeginHorizontal();
                        float minIconSizeValue = minIconSize.floatValue;
                        float maxIconSizeValue = maxIconSize.floatValue;
                        EditorGUILayout.MinMaxSlider(new GUIContent("Icon Size Range", "Minimum and maximum icon sizes. Icons grow/shrink depending on distance."), ref minIconSizeValue, ref maxIconSizeValue, 0.1f, 2f);
                        minIconSize.floatValue = minIconSizeValue;
                        maxIconSize.floatValue = maxIconSizeValue;
                        GUILayout.Label(minIconSizeValue.ToString("F2") + "-" + maxIconSizeValue.ToString("F2"));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(showDistance, new GUIContent("Show Distance"));
                        if (showDistance.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(showDistanceFormat, new GUIContent("String Format"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(visibleMaxDistance);
                        EditorGUILayout.PropertyField(visibleMinDistance);
                        EditorGUILayout.PropertyField(nearDistance);
                        EditorGUILayout.PropertyField(visitedDistance);
                        EditorGUILayout.PropertyField(scaleInDuration);
                        EditorGUILayout.PropertyField(labelHotZone);
                        EditorGUILayout.PropertyField(visitedDefaultAudioClip, new GUIContent("Visited Sound"));
                        EditorGUILayout.PropertyField(beaconDefaultAudioClip, new GUIContent("Beacon Sound"));
                        EditorGUILayout.PropertyField(heartbeatDefaultAudioClip, new GUIContent("Heartbeat Sound"));
                    }
                    EditorGUILayout.Separator();

                    expandTitleSettings = DrawExpandableSectionTitle(expandTitleSettings, "Title Settings");
                    if (expandTitleSettings) {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(titleFont, new GUIContent("Font"));
                        EditorGUILayout.PropertyField(titleFontTMP, new GUIContent("Font (Text Mesh Pro)"));
                        EditorGUILayout.PropertyField(titleVerticalPosition, new GUIContent("Vertical Offset"));
                        EditorGUILayout.PropertyField(titleScale, new GUIContent("Scale"));
                        EditorGUILayout.PropertyField(titleShadowEnabled, new GUIContent("Text Shadow"));
                        EditorGUILayout.PropertyField(titleMinPOIDistance, new GUIContent("Min POI Distance"));
                        EditorGUILayout.PropertyField(titleShowDistance, new GUIContent("Show Distance"));
                        if (titleShowDistance.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(titleShowDistanceFormat, new GUIContent("Distance Format"));
                            EditorGUI.indentLevel--;
                        }
                        if (EditorGUI.EndChangeCheck()) {
                            updateTitleInEditMode = true;
                        }
                    }
                    EditorGUILayout.Separator();

                    expandTextSettings = DrawExpandableSectionTitle(expandTextSettings, "Reveal Text Option");
                    if (expandTextSettings) {
                        EditorGUILayout.PropertyField(textRevealEnabled, new GUIContent("Enable"));
                        if (textRevealEnabled.boolValue) {
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(textFont, new GUIContent("Font"));
                            EditorGUILayout.PropertyField(textVerticalPosition, new GUIContent("Vertical Offset"));
                            EditorGUILayout.PropertyField(textScale, new GUIContent("Scale"));
                            EditorGUILayout.PropertyField(textRevealDuration, new GUIContent("Reveal Duration"));
                            EditorGUILayout.PropertyField(textLetterSpacing, new GUIContent("Letter Spacing"));
                            EditorGUILayout.PropertyField(textRevealLetterDelay, new GUIContent("Letter Delay"));
                            EditorGUILayout.PropertyField(textDuration, new GUIContent("Duration"));
                            EditorGUILayout.PropertyField(textFadeOutDuration, new GUIContent("Fade Out Duration"));
                            EditorGUILayout.PropertyField(textShadowEnabled, new GUIContent("Text Shadow"));
                            if (EditorGUI.EndChangeCheck()) {
                                updateTextInEditMode = true;
                            }
                        }
                    }
                    EditorGUILayout.Separator();
                }
            }

            if (showMiniMap.boolValue) {

                expandMiniMapSettings = DrawSectionTitle("Mini-Map Settings", expandMiniMapSettings);
                if (expandMiniMapSettings) {

                    EditorGUILayout.PropertyField(miniMapOrientation, new GUIContent("Orientation"));
                    EditorGUILayout.PropertyField(miniMapKeepStraight, new GUIContent("Keep Straight"));
                    EditorGUILayout.Separator();

                    expandMiniMapStyleSettings = DrawExpandableSectionTitle(expandMiniMapStyleSettings, "Style & Design");
                    if (expandMiniMapStyleSettings) {

                        EditorGUILayout.PropertyField(miniMapContents, new GUIContent("Contents"));
                        if (miniMapContents.intValue == (int)MiniMapContents.WorldMappedTexture) {
                            EditorGUILayout.PropertyField(miniMapContentsTexture, new GUIContent("Texture"));
                            if (_compass.miniMapCamera != null && !_compass.miniMapCamera.orthographic) {
                                EditorGUILayout.HelpBox("Feature only available with mini camera set to orthographic mode.", MessageType.Warning);
                            }
                            EditorGUILayout.PropertyField(miniMapWorldCenter, new GUIContent("World Center"));
                            EditorGUILayout.PropertyField(miniMapWorldSize, new GUIContent("World Size"));
                        } else if (miniMapContents.intValue == (int)MiniMapContents.UITexture) {
                            EditorGUILayout.PropertyField(miniMapContentsTexture, new GUIContent("Texture"));
                            if (!miniMapKeepStraight.boolValue) {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(miniMapContentsTextureAllowRotation, new GUIContent("Allow Rotation"));
                                EditorGUI.indentLevel--;
                            }
                        }

                        if (miniMapContents.intValue != (int)MiniMapContents.Radar) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(miniMapStyle, new GUIContent("Style"));
                            if (GUILayout.Button("?", GUILayout.Width(30))) {
                                showMiniMapTextures = !showMiniMapTextures;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel++;
                            if (miniMapStyle.intValue == (int)MiniMapStyle.Custom) {
                                EditorGUILayout.PropertyField(miniMapBorderTexture, new GUIContent("Border Texture"));
                                EditorGUILayout.PropertyField(miniMapMaskSprite, new GUIContent("Mask Texture"));
                            } else {
                                if (showMiniMapTextures) {
                                    GUI.enabled = false;
                                    EditorGUILayout.ObjectField(new GUIContent("Border Texture"), _compass.GetMiniMapMaterialBorderTexture(), typeof(Texture2D), false);
                                    EditorGUILayout.ObjectField(new GUIContent("Mask Texture"), _compass.GetMiniMapMaterialMaskTexture(), typeof(Texture2D), false);
                                    GUI.enabled = true;
                                }
                            }
                            EditorGUI.indentLevel--;
                            EditorGUILayout.PropertyField(miniMapClampBorderCircular, new GUIContent("Border Is Circular"));
                            EditorGUILayout.PropertyField(miniMapVignette, new GUIContent("Vignette"));
                            if (miniMapVignette.boolValue) {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(miniMapVignetteColor, new GUIContent("Color"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.PropertyField(miniMapBackgroundColor, new GUIContent("Background Color"));
                            EditorGUILayout.PropertyField(miniMapBackgroundOpaque, new GUIContent("Background Is Opaque"));
                        } else {
                            float radarRange = miniMapCaptureSize.floatValue * 0.5f;
                            radarRange = EditorGUILayout.FloatField(new GUIContent("Radar Range"), radarRange);
                            miniMapCaptureSize.floatValue = radarRange * 2f;
                            EditorGUILayout.PropertyField(miniMapRadarRingsDistance, new GUIContent("Rings Separation (m)"));
                            EditorGUILayout.PropertyField(miniMapRadarInfoDisplay, new GUIContent("Show Distance"));
                            EditorGUILayout.PropertyField(miniMapRadarRingsWidth, new GUIContent("Rings Width"));
                            EditorGUILayout.PropertyField(miniMapRadarRingsColor, new GUIContent("Rings Color"));
                            EditorGUILayout.PropertyField(miniMapRadarPulseEnabled, new GUIContent("Enable Pulse Effect"));
                            if (miniMapRadarPulseEnabled.boolValue) {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(miniMapRadarPulseOpacity, new GUIContent("Opacity"));
                                EditorGUILayout.PropertyField(miniMapRadarPulseAnimationPreset, new GUIContent("Animation Preset"));
                                if (miniMapRadarPulseAnimationPreset.intValue == (int)MiniMapPulsePreset.Custom) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(miniMapRadarPulseSpeed, new GUIContent("Speed"));
                                    EditorGUILayout.PropertyField(miniMapRadarPulseFallOff, new GUIContent("Fall Off"));
                                    EditorGUILayout.PropertyField(miniMapRadarPulseFrequency, new GUIContent("Frequency"));
                                    EditorGUI.indentLevel--;
                                }
                                EditorGUI.indentLevel--;
                            }
                        }

                        EditorGUILayout.PropertyField(miniMapPositionAndSize, new GUIContent("Position & Size"));
                        if (miniMapPositionAndSize.intValue == (int)MiniMapPositionAndScaleMode.ControlledByCompassNavigatorPro) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(miniMapLocation, new GUIContent("Screen Location"));
                            EditorGUILayout.PropertyField(miniMapScreenPositionOffset, new GUIContent("Offset"));
                            EditorGUILayout.PropertyField(miniMapSize, new GUIContent("Size"));
                            EditorGUI.indentLevel--;
                        }

                        EditorGUILayout.PropertyField(miniMapAlpha, new GUIContent("Alpha"));
                        if (miniMapContents.intValue != (int)MiniMapContents.Radar) {
                            EditorGUILayout.PropertyField(miniMapContrast, new GUIContent("Contrast"));
                            EditorGUILayout.PropertyField(miniMapBrightness, new GUIContent("Brightness"));
                            EditorGUILayout.PropertyField(miniMapLutTexture, new GUIContent("LUT Texture"));
                            if (_compass.miniMapLutTexture != null) {
                                CheckLUTSettings(_compass.miniMapLutTexture);
                                EditorGUILayout.PropertyField(miniMapLutIntensity, new GUIContent("LUT Intensity"));
                            }
                            EditorGUILayout.PropertyField(miniMapTintColor, new GUIContent("Tint Color"));
                        }
                        EditorGUILayout.PropertyField(miniMapShowCardinals, new GUIContent("Show Cardinals"));
                        if (miniMapShowCardinals.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(miniMapCardinalsSize, new GUIContent("Size"));
                            EditorGUILayout.PropertyField(miniMapCardinalsSprite, new GUIContent("Sprite"));
                            EditorGUILayout.PropertyField(miniMapCardinalsColor, new GUIContent("Color"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(miniMapShowPlayerIcon, new GUIContent("Show Player/Compass Icon"));
                        if (miniMapShowPlayerIcon.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(miniMapPlayerIconSize, new GUIContent("Size"));
                            EditorGUILayout.PropertyField(miniMapPlayerIconSprite, new GUIContent("Sprite"));
                            EditorGUILayout.PropertyField(miniMapPlayerIconColor, new GUIContent("Color"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(miniMapShowViewCone, new GUIContent("Show View Cone"));
                        if (miniMapShowViewCone.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(miniMapViewConeFoVSource, new GUIContent("FoV Source"));
                            if (miniMapViewConeFoVSource.intValue == (int)MiniMapViewConeFovSource.UserDefined) {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(miniMapViewConeFoV, new GUIContent("Custom FoV"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.PropertyField(miniMapViewConeDistance, new GUIContent("Distance (m)"));
                            EditorGUILayout.PropertyField(miniMapViewConeFallOff, new GUIContent("Fall Off"));
                            EditorGUILayout.PropertyField(miniMapViewConeColor, new GUIContent("Color"));
                            EditorGUILayout.PropertyField(miniMapShowViewConeOutline, new GUIContent("Outline"));
                            if (miniMapShowViewConeOutline.boolValue) {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(miniMapViewConeOutlineColor, new GUIContent("Color"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUILayout.Separator();

                    expandMiniMapPOISettings = DrawExpandableSectionTitle(expandMiniMapPOISettings, "POI Settings");
                    if (expandMiniMapPOISettings) {
                        EditorGUILayout.PropertyField(miniMapIconPrefab, new GUIContent("Icon Prefab"));
                        EditorGUILayout.PropertyField(miniMapVisibleMaxDistance, new GUIContent("Visible Max Distance"));
                        EditorGUILayout.PropertyField(miniMapIconSize, new GUIContent("Size"));
                        EditorGUILayout.PropertyField(miniMapIconPositionShift, new GUIContent("Position Shift"));
                        EditorGUILayout.PropertyField(miniMapClampBorder, new GUIContent("Clamp Border Margin"));
                        EditorGUILayout.PropertyField(miniMapIconCircleAnimationDuration, new GUIContent("Circle Animation Duration"));
                    }
                    EditorGUILayout.Separator();

                    expandMiniMapRenderingSettings = DrawExpandableSectionTitle(expandMiniMapRenderingSettings, "World Capture & Rendering");
                    if (expandMiniMapRenderingSettings) {
                        if (!_compass.miniMapContents.usesTexture()) {
                            EditorGUILayout.PropertyField(miniMapResolution, new GUIContent("Resolution"));
                            EditorGUILayout.PropertyField(miniMapLayerMask, new GUIContent("Layer Mask"));
                            EditorGUILayout.PropertyField(miniMapEnableShadows, new GUIContent("Render Shadows"));
                        }
                        if (miniMapContents.intValue == (int)MiniMapContents.Radar) {
                            EditorGUILayout.PropertyField(miniMapZoomMin, new GUIContent("Zoom Min"));
                            EditorGUILayout.PropertyField(miniMapZoomMax, new GUIContent("Zoom Max"));
                        } else {
                            EditorGUILayout.PropertyField(miniMapClampToWorldEdges, new GUIContent("Clamp To World Edges"));
                            EditorGUILayout.PropertyField(miniMapCameraMode, new GUIContent("Camera Projection"));
                            if (miniMapCameraMode.intValue == (int)MiniMapCameraMode.Orthographic) {
                                EditorGUILayout.PropertyField(miniMapCameraHeightVSFollow, new GUIContent("Camera Altitude vs Follow"));
                                EditorGUILayout.PropertyField(miniMapCaptureSize, new GUIContent("Captured World Size"));
                                EditorGUILayout.PropertyField(miniMapCameraDepth, new GUIContent("Captured Depth"));
                                if (!_compass.miniMapContents.usesTexture()) {
                                    EditorGUILayout.PropertyField(miniMapCameraSnapshotFrequency, new GUIContent("Snapshot Frequency"));
                                    if (_compass.miniMapCameraSnapshotFrequency != MiniMapCameraSnapshotFrequency.Continuous) {
                                        EditorGUILayout.BeginHorizontal();
                                        switch (_compass.miniMapCameraSnapshotFrequency) {
                                            case MiniMapCameraSnapshotFrequency.TimeInterval:
                                                EditorGUILayout.PropertyField(miniMapSnapshotInterval, new GUIContent("Time Interval (s)"));
                                                break;
                                            case MiniMapCameraSnapshotFrequency.DistanceTravelled:
                                                EditorGUILayout.PropertyField(miniMapSnapshotDistance, new GUIContent("Distance (m)"));
                                                break;
                                        }
                                        if (GUILayout.Button("Now!", GUILayout.Width(60))) {
                                            _compass.UpdateMiniMapContents();
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                                EditorGUILayout.PropertyField(miniMapZoomMin, new GUIContent("Zoom Min"));
                                EditorGUILayout.PropertyField(miniMapZoomMax, new GUIContent("Zoom Max"));
                            } else {
                                EditorGUILayout.PropertyField(miniMapCameraMinAltitude, new GUIContent("Altitude Min"));
                                EditorGUILayout.PropertyField(miniMapCameraMaxAltitude, new GUIContent("Altitude Max"));
                            }
                        }
                        GUI.enabled = !_compass.miniMapFullScreenState;
                        EditorGUILayout.PropertyField(miniMapZoomLevel, new GUIContent("Current Zoom"));
                        GUI.enabled = true;
                        if (_compass.miniMapCameraMode == MiniMapCameraMode.Perspective) {
                            EditorGUILayout.PropertyField(miniMapCameraTilt, new GUIContent("Camera Tilt"));
                        }

                        if (CNP2HDRPCameraSetup.usesHDRP) {
                            if (_compass.miniMapCamera != null) {
                                if (GUILayout.Button("Configure additional mini-map camera settings")) {
                                    Selection.activeObject = _compass.miniMapCamera;
                                    EditorGUIUtility.PingObject(_compass.miniMapCamera.gameObject);
                                    GUIUtility.ExitGUI();
                                }
                            }
                        }
                    }
                    EditorGUILayout.Separator();

                    expandMiniMapFullScreenModeSettings = DrawExpandableSectionTitle(expandMiniMapFullScreenModeSettings, "Maximized Mode Settings");
                    if (expandMiniMapFullScreenModeSettings) {
                        EditorGUILayout.PropertyField(miniMapFullScreenState, new GUIContent("Maximized Mode"));
                        EditorGUILayout.PropertyField(miniMapFullScreenContents, new GUIContent("Contents"));
                        if (miniMapFullScreenContents.intValue == (int)MiniMapContents.WorldMappedTexture) {
                            EditorGUILayout.PropertyField(miniMapFullScreenContentsTexture, new GUIContent("Texture"));
                            if (_compass.miniMapCamera != null && !_compass.miniMapCamera.orthographic) {
                                EditorGUILayout.HelpBox("Feature only available with mini camera set to orthographic mode.", MessageType.Warning);
                            }
                        } else if (miniMapFullScreenContents.intValue == (int)MiniMapContents.UITexture) {
                            EditorGUILayout.PropertyField(miniMapFullScreenContentsTexture, new GUIContent("Texture"));
                        }
                        EditorGUILayout.PropertyField(miniMapFullScreenWorldCenterFollows, new GUIContent("Center On Followed"));
                        if (!_compass.miniMapFullScreenWorldCenterFollows) {
                            EditorGUILayout.PropertyField(miniMapFullScreenWorldCenter, new GUIContent("World Center"));
                        }
                        EditorGUILayout.PropertyField(miniMapFullScreenWorldSize, new GUIContent("World Size"));
                        EditorGUILayout.PropertyField(miniMapFullScreenClampToWorldEdges, new GUIContent("Clamp To World Edges"));
                        if (_compass.miniMapFullScreenContents != MiniMapContents.Radar) {
                            EditorGUILayout.PropertyField(miniMapFullScreenStyle, new GUIContent("Full Screen Style"));
                            if (_compass.miniMapFullScreenStyle == MiniMapStyle.Custom) {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(miniMapBorderTextureFullScreenMode, new GUIContent("Border Texture"));
                                EditorGUILayout.PropertyField(miniMapMaskSpriteFullScreenMode, new GUIContent("Mask Texture"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.PropertyField(miniMapFullScreenClampBorderCircular, new GUIContent("Border Is Circular"));
                        }
                        EditorGUILayout.PropertyField(miniMapFullScreenClampBorder, new GUIContent("Clamp To Border Margin (POIs)"));
                        EditorGUILayout.PropertyField(miniMapFullScreenPlaceholder, new GUIContent("UI Placeholder"));
                        EditorGUILayout.PropertyField(miniMapFullScreenSize, new GUIContent("Screen Size"));
                        if (_compass.miniMapFullScreenPlaceholder == null) {
                            EditorGUILayout.PropertyField(miniMapKeepAspectRatio, new GUIContent("Keep Aspect Ratio"));
                        }
                        if (!_compass.miniMapFullScreenContents.usesTexture()) {
                            EditorGUILayout.PropertyField(miniMapFullScreenResolution, new GUIContent("Resolution"));
                        }
                        GUI.enabled = _compass.miniMapFullScreenState;
                        EditorGUILayout.PropertyField(miniMapFullScreenZoomLevel, new GUIContent("Zoom Level"));
                        GUI.enabled = true;
                        EditorGUILayout.PropertyField(miniMapFullScreenFreezeCamera, new GUIContent("Freeze Camera"));
                    }
                    EditorGUILayout.Separator();

                    if (!_compass.miniMapContents.usesTexture()) {

                        EditorGUILayout.BeginHorizontal();
                        expandMiniMapFogOfWarSettings = DrawExpandableSectionTitle(expandMiniMapFogOfWarSettings, "Fog Of War Settings");
                        if (GUILayout.Button("Help", GUILayout.Width(50))) {
                            EditorUtility.DisplayDialog("Mini-Map Fog Of War", "This feature renders animated fog on top of the mini-map. To clear certain areas from fog, select option menu GameObject -> Create Other -> Compass Navigator Pro -> Mini-Map Fog of War Volume.\n\nReposition/scale fog volumes anywhere on the map.\n\nYou can also use scripting to control fog of war opacity at any position. Please check the documentation for details.", "Ok");
                        }
                        EditorGUILayout.EndHorizontal();

                        if (expandMiniMapFogOfWarSettings) {

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(fogOfWarEnabled, new GUIContent("Enable"));
                            if (fogOfWarEnabled.boolValue) {
                                if (GUILayout.Button("Fit to Active Terrain")) {
                                    FitFogOfWarLayerToActiveTerrain();
                                }
                                if (GUILayout.Button("Redraw")) {
                                    _compass.UpdateFogOfWar();
                                }

                                EditorGUILayout.EndHorizontal();

                                if (_compass.fogOfWarEnabled && _compass.miniMapCameraMode != MiniMapCameraMode.Orthographic) {
                                    EditorGUILayout.HelpBox("Fog of war requires mini-map in orthographic mode.", MessageType.Warning);
                                }

                                EditorGUILayout.PropertyField(fogOfWarCenter, new GUIContent("Center"));
                                EditorGUILayout.PropertyField(fogOfWarSize, new GUIContent("Size"));
                                EditorGUILayout.PropertyField(fogOfWarTextureSize, new GUIContent("Resolution"));
                                EditorGUILayout.PropertyField(fogOfWarDefaultAlpha, new GUIContent("Default Opacity"));
                                EditorGUILayout.PropertyField(fogOfWarColor, new GUIContent("Fog Color"));
                                EditorGUILayout.PropertyField(fogOfWarAutoClear, new GUIContent("Auto Clear Fog"));

                                if (_compass.fogOfWarAutoClear) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(fogOfWarAutoClearRadius, new GUIContent("Clear Radius"));
                                    EditorGUI.indentLevel--;
                                }
                            } else {
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.Separator();

                    expandMiniMapInteractionSettings = DrawExpandableSectionTitle(expandMiniMapInteractionSettings, "Interaction Settings");
                    if (expandMiniMapInteractionSettings) {
                        EditorGUILayout.PropertyField(miniMapShowZoomInOutButtons, new GUIContent("Show Zoom In/Out Buttons"));
                        EditorGUILayout.PropertyField(miniMapShowMaximizeButton, new GUIContent("Show Maximize Button"));
                        if (miniMapShowZoomInOutButtons.boolValue || miniMapShowMaximizeButton.boolValue) {
                            EditorGUILayout.PropertyField(miniMapButtonsScale, new GUIContent("Buttons Scale"));
                        }
                        EditorGUILayout.PropertyField(miniMapIconEvents, new GUIContent("Generate Click Events"));
                        EditorGUILayout.LabelField("Allow User Drag");
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(miniMapAllowUserDrag, new GUIContent("In Normal Mini-map"));
                        EditorGUILayout.PropertyField(miniMapFullScreenAllowUserDrag, new GUIContent("In Maximized Mode"));
                        if (miniMapAllowUserDrag.boolValue || miniMapFullScreenAllowUserDrag.boolValue) {
                            EditorGUILayout.PropertyField(miniMapDragMaxDistance, new GUIContent("Maximum Drag Distance"));
                        }
                        EditorGUI.indentLevel--;

                    }
                    EditorGUILayout.Separator();
                }
            }

            if (showOnScreenIndicators.boolValue) {
                expandOnScreenIndicatorSettings = DrawSectionTitle("On-Screen Indicators Settings", expandOnScreenIndicatorSettings);
                if (expandOnScreenIndicatorSettings) {
                    EditorGUILayout.PropertyField(onScreenIndicatorPrefab, new GUIContent("Indicator Prefab"));
                    EditorGUILayout.PropertyField(onScreenIndicatorScale, new GUIContent("Scale"));
                    EditorGUILayout.PropertyField(onScreenIndicatorNearFadeDistance, new GUIContent("Near Fade Distance"));
                    EditorGUILayout.PropertyField(onScreenIndicatorNearFadeMin, new GUIContent("Near Fade Min"));
                    EditorGUILayout.PropertyField(onScreenIndicatorAlpha, new GUIContent("Alpha"));
                    EditorGUILayout.PropertyField(onScreenIndicatorShowDistance, new GUIContent("Show Distance"));
                    if (onScreenIndicatorShowDistance.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(onScreenIndicatorShowDistanceFormat, new GUIContent("Distance Format"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(onScreenIndicatorShowTitle, new GUIContent("Show Title"));
                    EditorGUILayout.Separator();
                }
            }

            if (_compass.showOffScreenIndicators) {
                expandOffScreenIndicatorSettings = DrawSectionTitle("Off-Screen Indicators Settings", expandOffScreenIndicatorSettings);
                if (expandOffScreenIndicatorSettings) {
                    EditorGUILayout.PropertyField(onScreenIndicatorPrefab, new GUIContent("Indicator Prefab"));
                    EditorGUILayout.PropertyField(offScreenIndicatorScale, new GUIContent("Scale"));
                    EditorGUILayout.PropertyField(offScreenIndicatorMargin, new GUIContent("Margins"));
                    EditorGUILayout.PropertyField(offScreenIndicatorAlpha, new GUIContent("Alpha"));
                    EditorGUILayout.PropertyField(offScreenIndicatorAvoidOverlap, new GUIContent("Avoid Icon Overlap"));
                    if (offScreenIndicatorAvoidOverlap.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(offScreenIndicatorOverlapDistance, new GUIContent("Overlap Distance"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.Separator();
                }
            }

            if (serializedObject.ApplyModifiedProperties() || "UndoRedoPerformed".Equals(Event.current.commandName)) {
                if (updateTitleInEditMode) {
                    _compass.UpdateTitleAppearanceEditMode();
                }
                if (updateTextInEditMode) {
                    _compass.UpdateTextAppearanceEditMode();
                }
                _compass.UpdateSettings();
            }
        }

        bool DrawExpandableSectionTitle(bool expanded, string s) {
            return EditorGUILayout.Foldout(expanded, new GUIContent(s), sectionHeaderStyle);
        }

        bool DrawSectionTitle(string s, bool expanded) {
            if (titleLabelStyle == null) {
                GUIStyle skurikenModuleTitleStyle = "ShurikenModuleTitle";
                titleLabelStyle = new GUIStyle(skurikenModuleTitleStyle);
                titleLabelStyle.contentOffset = new Vector2(5f, -2f);
                titleLabelStyle.normal.textColor = titleColor;
                titleLabelStyle.fixedHeight = 22;
                titleLabelStyle.fontStyle = FontStyle.Bold;
            }

            if (GUILayout.Button(s, titleLabelStyle)) expanded = !expanded;
            return expanded;
        }

        void FitFogOfWarLayerToActiveTerrain() {
            Terrain activeTerrain = Terrain.activeTerrain;
            if (activeTerrain == null) {
                EditorUtility.DisplayDialog("Fit to Terrain", "No active terrain found!", "Ok");
                return;
            }
            Vector3 size = activeTerrain.terrainData.size;
            _compass.fogOfWarCenter = new Vector3(activeTerrain.transform.position.x + size.x * 0.5f, 0, activeTerrain.transform.position.z + size.z * 0.5f);
            _compass.fogOfWarSize = new Vector3(size.x, 0, size.z);
        }

        public static void CheckLUTSettings(Texture2D tex) {
            if (Application.isPlaying || tex == null)
                return;
            string path = AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path))
                return;
            TextureImporter imp = (TextureImporter)AssetImporter.GetAtPath(path);
            if (imp == null)
                return;
            if (imp.textureType != TextureImporterType.Default || imp.sRGBTexture || imp.mipmapEnabled || imp.textureCompression != TextureImporterCompression.Uncompressed || imp.wrapMode != TextureWrapMode.Clamp || imp.filterMode != FilterMode.Bilinear) {
                EditorGUILayout.HelpBox("Texture has invalid import settings.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Fix Texture Import Settings", GUILayout.Width(200))) {
                    imp.textureType = TextureImporterType.Default;
                    imp.sRGBTexture = false;
                    imp.mipmapEnabled = false;
                    imp.textureCompression = TextureImporterCompression.Uncompressed;
                    imp.wrapMode = TextureWrapMode.Clamp;
                    imp.filterMode = FilterMode.Bilinear;
                    imp.anisoLevel = 0;
                    imp.SaveAndReimport();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }


    }

}
