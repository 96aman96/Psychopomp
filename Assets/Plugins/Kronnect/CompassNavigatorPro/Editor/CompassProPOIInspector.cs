using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CompassNavigatorPro {

    [CustomEditor(typeof(CompassProPOI))]
    public class CompassProPOIInspector : Editor {

        bool expandGeneralSettings, expandCompassBarSettings, expandMiniMapSettings, expandScreenIndicatorSettings, expandOtherOptions;

        SerializedProperty id;
        SerializedProperty priority;
        SerializedProperty dontDestroyOnLoad;

        SerializedProperty visibility, visibleDistanceOverride, visibleMinDistanceOverride, titleMinPOIDistanceOverride;
        SerializedProperty iconScale, iconNonVisited, iconVisited, tintColor, clampPosition;
        SerializedProperty title, titleVisibility;
        SerializedProperty positionOffset;

        SerializedProperty canBeVisited, radius, visitedDistanceOverride, hideWhenVisited, isVisited, visitedText, playAudioClipWhenVisited, visitedAudioClipOverride;

        SerializedProperty showOnScreenIndicator, onScreenIndicatorScale, onScreenIndicatorShowDistance, onScreenIndicatorShowTitle, onScreenIndicatorNearFadeDistance, onScreenIndicatorNearFadeMin;
        SerializedProperty showOffScreenIndicator;

        SerializedProperty beaconAudioClip;
        SerializedProperty heartbeatEnabled, heartbeatAudioClip, heartbeatDistance, heartbeatInterval;

        SerializedProperty miniMapVisibility, miniMapVisibleDistanceOverride, miniMapClampPosition, miniMapShowRotation, miniMapRotationAngleOffset, miniMapIconScale;
        SerializedProperty miniMapIconPrefabOverride;

        SerializedProperty miniMapShowCircle, miniMapCircleColor, miniMapCircleInnerColor, miniMapCircleStartRadius, miniMapCircleAnimationWhenAppears, miniMapCircleAnimationRepetitions;

        static GUIStyle sectionHeaderStyle, titleLabelStyle;
        static Color titleColor;

        CompassProPOI poi;

        void OnEnable() {

            titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);

            id = serializedObject.FindProperty("id");
            priority = serializedObject.FindProperty("priority");
            dontDestroyOnLoad = serializedObject.FindProperty("dontDestroyOnLoad");

            visibility = serializedObject.FindProperty("visibility");
            visibleDistanceOverride = serializedObject.FindProperty("visibleDistanceOverride");
            visibleMinDistanceOverride = serializedObject.FindProperty("visibleMinDistanceOverride");
            titleMinPOIDistanceOverride = serializedObject.FindProperty("titleMinPOIDistanceOverride");
            iconScale = serializedObject.FindProperty("iconScale");
            clampPosition = serializedObject.FindProperty("clampPosition");
            positionOffset = serializedObject.FindProperty("positionOffset");

            canBeVisited = serializedObject.FindProperty("canBeVisited");
            iconNonVisited = serializedObject.FindProperty("iconNonVisited");
            iconVisited = serializedObject.FindProperty("iconVisited");
            tintColor = serializedObject.FindProperty("tintColor");
            title = serializedObject.FindProperty("title");
            titleVisibility = serializedObject.FindProperty("titleVisibility");
            radius = serializedObject.FindProperty("radius");
            visitedDistanceOverride = serializedObject.FindProperty("visitedDistanceOverride");
            hideWhenVisited = serializedObject.FindProperty("hideWhenVisited");
            isVisited = serializedObject.FindProperty("isVisited");
            visitedText = serializedObject.FindProperty("visitedText");
            playAudioClipWhenVisited = serializedObject.FindProperty("playAudioClipWhenVisited");
            visitedAudioClipOverride = serializedObject.FindProperty("visitedAudioClipOverride");

            miniMapVisibility = serializedObject.FindProperty("miniMapVisibility");
            miniMapClampPosition = serializedObject.FindProperty("miniMapClampPosition");
            miniMapShowRotation = serializedObject.FindProperty("miniMapShowRotation");
            miniMapRotationAngleOffset = serializedObject.FindProperty("miniMapRotationAngleOffset");
            miniMapIconScale = serializedObject.FindProperty("miniMapIconScale");
            miniMapVisibleDistanceOverride = serializedObject.FindProperty("miniMapVisibleDistanceOverride");
            miniMapIconPrefabOverride = serializedObject.FindProperty("miniMapIconPrefabOverride");

            showOnScreenIndicator = serializedObject.FindProperty("showOnScreenIndicator");
            onScreenIndicatorScale = serializedObject.FindProperty("onScreenIndicatorScale");
            onScreenIndicatorShowDistance = serializedObject.FindProperty("onScreenIndicatorShowDistance");
            onScreenIndicatorShowTitle = serializedObject.FindProperty("onScreenIndicatorShowTitle");
            onScreenIndicatorNearFadeDistance = serializedObject.FindProperty("onScreenIndicatorNearFadeDistance");
            onScreenIndicatorNearFadeMin = serializedObject.FindProperty("onScreenIndicatorNearFadeMin");

            showOffScreenIndicator = serializedObject.FindProperty("showOffScreenIndicator");

            heartbeatEnabled = serializedObject.FindProperty("heartbeatEnabled");
            heartbeatAudioClip = serializedObject.FindProperty("heartbeatAudioClip");
            heartbeatDistance = serializedObject.FindProperty("heartbeatDistance");
            heartbeatInterval = serializedObject.FindProperty("heartbeatInterval");

            beaconAudioClip = serializedObject.FindProperty("beaconAudioClip");

            miniMapShowCircle = serializedObject.FindProperty("miniMapShowCircle");
            miniMapCircleColor = serializedObject.FindProperty("miniMapCircleColor");
            miniMapCircleInnerColor = serializedObject.FindProperty("miniMapCircleInnerColor");
            miniMapCircleStartRadius = serializedObject.FindProperty("miniMapCircleStartRadius");
            miniMapCircleAnimationWhenAppears = serializedObject.FindProperty("miniMapCircleAnimationWhenAppears");
            miniMapCircleAnimationRepetitions = serializedObject.FindProperty("miniMapCircleAnimationRepetitions");

            poi = (CompassProPOI)target;

            expandGeneralSettings = EditorPrefs.GetBool("CNPOIGeneralSettings", true);
            expandCompassBarSettings = EditorPrefs.GetBool("CNPOICompassBarSettings", true);
            expandMiniMapSettings = EditorPrefs.GetBool("CNPOIMiniMapSettings", true);
            expandScreenIndicatorSettings = EditorPrefs.GetBool("CNPOIScreenIndicatorSettings", true);
            expandOtherOptions = EditorPrefs.GetBool("CNPOIOtherSettings", true);
        }

        private void OnDisable() {
            EditorPrefs.SetBool("CNPOIGeneralSettings", expandGeneralSettings);
            EditorPrefs.SetBool("CNPOICompassBarSettings", expandCompassBarSettings);
            EditorPrefs.SetBool("CNPOIMiniMapSettings", expandMiniMapSettings);
            EditorPrefs.SetBool("CNPOIScreenIndicatorSettings", expandScreenIndicatorSettings);
            EditorPrefs.SetBool("CNPOIOtherSettings", expandOtherOptions);
        }

        public override void OnInspectorGUI() {

            if (sectionHeaderStyle == null) {
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldout);
                sectionHeaderStyle.fontStyle = FontStyle.Bold;
            }

            GUIContent overrideLabel = new GUIContent("Custom Overrides", "These settings can be used to override the global values set in Compass Navigator Pro component.");

            serializedObject.Update();

            expandGeneralSettings = DrawSectionTitle("General POI Settings", expandGeneralSettings);
            if (expandGeneralSettings) {
                EditorGUILayout.PropertyField(id);
                EditorGUILayout.PropertyField(iconNonVisited, new GUIContent("Icon"));
                EditorGUILayout.PropertyField(radius);
                EditorGUILayout.PropertyField(positionOffset);
                EditorGUILayout.PropertyField(canBeVisited);
                if (canBeVisited.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(isVisited);
                    EditorGUILayout.PropertyField(iconVisited, new GUIContent("Icon When Visited"));
                    EditorGUILayout.PropertyField(visitedDistanceOverride);
                    EditorGUILayout.PropertyField(playAudioClipWhenVisited);
                    if (playAudioClipWhenVisited.boolValue) {
                        EditorGUILayout.PropertyField(visitedAudioClipOverride, new GUIContent("Custom Audio Clip"));
                    }
                    EditorGUILayout.PropertyField(hideWhenVisited);
                    EditorGUILayout.PropertyField(heartbeatEnabled, new GUIContent("Enable Heartbeat"));
                    if (heartbeatEnabled.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(heartbeatDistance, new GUIContent("Start Distance"));
                        EditorGUILayout.PropertyField(heartbeatAudioClip, new GUIContent("Audio Clip"));
                        EditorGUILayout.PropertyField(heartbeatInterval, new GUIContent("Interval"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Separator();
            }

            expandCompassBarSettings = DrawSectionTitle("Compass Bar Settings", expandCompassBarSettings);
            if (expandCompassBarSettings) {
                EditorGUILayout.PropertyField(visibility, new GUIContent("Visibility In Compass Bar"));
                EditorGUILayout.LabelField("Is Visible Now", poi.isVisible.ToString());
                if (visibility.intValue != (int)POIVisibility.AlwaysHidden) {
                    EditorGUILayout.PropertyField(iconScale);
                    EditorGUILayout.PropertyField(clampPosition);
                    EditorGUILayout.PropertyField(tintColor);
                    EditorGUILayout.PropertyField(title);
                    EditorGUILayout.PropertyField(titleVisibility);
                    EditorGUILayout.PropertyField(visitedText);
                    EditorGUILayout.LabelField(overrideLabel, EditorStyles.miniBoldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(visibleDistanceOverride, new GUIContent("Max Distance"));
                    EditorGUILayout.PropertyField(visibleMinDistanceOverride, new GUIContent("Min Distance"));
                    EditorGUILayout.PropertyField(titleMinPOIDistanceOverride, new GUIContent("Min Distance Title"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Separator();
            }

            expandMiniMapSettings = DrawSectionTitle("Mini-Map Settings", expandMiniMapSettings);
            if (expandMiniMapSettings) {
                EditorGUILayout.PropertyField(miniMapVisibility, new GUIContent("Visibility In Minimap")); ;
                if (miniMapVisibility.intValue != (int)POIVisibility.AlwaysHidden) {
                    EditorGUILayout.LabelField("Is Visible Now", poi.miniMapIsVisible.ToString());
                    EditorGUILayout.PropertyField(miniMapIconScale, new GUIContent("Icon Scale"));
                    EditorGUILayout.PropertyField(miniMapClampPosition, new GUIContent("Clamp Position"));
                    EditorGUILayout.PropertyField(miniMapShowRotation, new GUIContent("Show Rotation"));
                    EditorGUILayout.PropertyField(miniMapRotationAngleOffset, new GUIContent("Rotation Angle Offset"));
                    EditorGUILayout.PropertyField(miniMapShowCircle, new GUIContent("Show Circle Radius"));
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(radius);
                    EditorGUILayout.PropertyField(miniMapCircleColor, new GUIContent("Border Color"));
                    EditorGUILayout.PropertyField(miniMapCircleInnerColor, new GUIContent("Inner Color"));
                    EditorGUILayout.PropertyField(miniMapCircleStartRadius, new GUIContent("Start Radius"));
                    EditorGUI.indentLevel--;
                    EditorGUILayout.PropertyField(miniMapCircleAnimationWhenAppears, new GUIContent("Enable Circle Animation"));
                    if (miniMapCircleAnimationWhenAppears.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(miniMapCircleAnimationRepetitions, new GUIContent("Repetitions"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.LabelField(overrideLabel, EditorStyles.miniBoldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(miniMapIconPrefabOverride, new GUIContent("Icon Prefab"));
                    EditorGUILayout.PropertyField(miniMapVisibleDistanceOverride, new GUIContent("Visible Max Distance"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Separator();
            }

            expandScreenIndicatorSettings = DrawSectionTitle("Screen Indicators Settings", expandScreenIndicatorSettings);
            if (expandScreenIndicatorSettings) {
                EditorGUILayout.PropertyField(showOnScreenIndicator, new GUIContent("Show On-Screen Indicator"));
                if (showOnScreenIndicator.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(onScreenIndicatorScale, new GUIContent("Scale"));
                    EditorGUILayout.PropertyField(onScreenIndicatorShowDistance, new GUIContent("Show Distance"));
                    EditorGUILayout.PropertyField(onScreenIndicatorShowTitle, new GUIContent("Show Title"));
                    EditorGUILayout.LabelField(overrideLabel, EditorStyles.miniBoldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(onScreenIndicatorNearFadeDistance, new GUIContent("Near Fade Distance"));
                    EditorGUILayout.PropertyField(onScreenIndicatorNearFadeMin, new GUIContent("Near Fade Min"));
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(showOffScreenIndicator, new GUIContent("Show Off-Screen Indicator"));
                EditorGUILayout.Separator();
            }

            expandOtherOptions = DrawSectionTitle("Other Options", expandOtherOptions);
            if (expandOtherOptions) {
                EditorGUILayout.PropertyField(priority);
                EditorGUILayout.PropertyField(dontDestroyOnLoad);
                EditorGUILayout.PropertyField(beaconAudioClip);
            }

            if (serializedObject.ApplyModifiedProperties()) {
                if (poi.compass != null) poi.compass.Refresh();
            }
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

    }
}
