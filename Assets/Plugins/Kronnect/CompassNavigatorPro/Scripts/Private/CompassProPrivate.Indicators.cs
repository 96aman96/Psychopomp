using TMPro;
using UnityEngine;

namespace CompassNavigatorPro {

    public partial class CompassPro : MonoBehaviour {

        #region Indicators

        const string INDICATORS_ROOT_NAME = "OnScreen Indicators Root";
        const int MAX_STORED_VPOS = 100;
        readonly Vector3[] lastVPos = new Vector3[MAX_STORED_VPOS];

        void InitIndicators() {
            if (_onScreenIndicatorPrefab == null) {
                _onScreenIndicatorPrefab = Resources.Load<GameObject>("CNPro/Prefabs/POIGizmo");
            }

            if (indicatorsRoot == null) {
                indicatorsRoot = transform.Find(INDICATORS_ROOT_NAME);
                if (indicatorsRoot == null) {
                    GameObject root = Resources.Load<GameObject>("CNPro/Prefabs/OnScreenIndicatorsRoot");
                    if (root != null) {
                        GameObject rootGO = Instantiate(root, transform, false);
                        rootGO.name = INDICATORS_ROOT_NAME;
                        indicatorsRoot = rootGO.transform;
                    }
                }
            }
            indicatorsRoot.gameObject.SetActive(_showOnScreenIndicators || _showOffScreenIndicators);
        }

        void UpdateIndicators() {

            float aspect = _cameraMain.aspect;

            float overlapDir = 1f;
            float distThreshold = _offScreenIndicatorOverlapDistance * 0.9f;

            float vextentsX = 0.5f - _offScreenIndicatorMargin;
            float vextentsY = 0.5f - _offScreenIndicatorMargin * aspect;
            float minX = _offScreenIndicatorMargin;
            float maxX = 1f - minX;
            float minY = _offScreenIndicatorMargin * aspect;
            float maxY = 1f - minY;
            Vector3 scaleVector = Misc.Vector3one;
            float scaleAnimSpeed = Time.deltaTime * 10f;
            int frameCount = Time.frameCount;
            int vPosCount = 0;
            int poiCount = pois.Count;

            for (int k = 0; k < poiCount; k++) {
                CompassProPOI poi = pois[k];

                if (!poi.isActiveAndEnabled) {
                    poi.ToggleIndicatorVisibility(false);
                    continue;
                }

                bool visible = !_miniMapFullScreenState;
                if (poi.isVisited && poi.hideWhenVisited) visible = false;

                if (!visible) {
                    poi.ToggleIndicatorVisibility(false);
                    continue;
                }

                // Update POI viewport position and distance
                if (frameCount != poi.viewportPosFrameCount) {
                    poi.viewportPosFrameCount = frameCount;
                    ComputePOIViewportPos(poi);
                }

                Vector3 vpos = poi.viewportPos;

                bool isOnScreen = vpos.z > 0 && vpos.x >= minX && vpos.x < maxX && vpos.y >= minY && vpos.y < maxY;
                float ang = 0;
                float scale = 1f;

                if (isOnScreen) {
                    visible = _showOnScreenIndicators && poi.showOnScreenIndicator;
                    if (visible && poi.isOnScreen >= 0) {
                        poi.isOnScreen = -1;
                        OnPOIOnScreen?.Invoke(poi);
                    }
                    scale = _onScreenIndicatorScale * 0.25f;
                } else {
                    visible = _showOffScreenIndicators && poi.showOffScreenIndicator;
                    if (visible) {
                        if (poi.isOnScreen <= 0) {
                            poi.isOnScreen = 1;
                            OnPOIOffScreen?.Invoke(poi);
                        }
                        scale = _offScreenIndicatorScale * 0.25f;
                        vpos.x -= 0.5f;
                        vpos.y -= 0.5f;
                        if (vpos.z < 0) {
                            vpos *= -1f;
                            if (vpos.y > 0) vpos.y = -vpos.y; // when behind, always show indicator on the bottom half of the screen
                        }
                        ang = Mathf.Atan2(vpos.y, vpos.x);
                        float s = Mathf.Tan(ang);
                        if (vpos.x > 0) {
                            vpos.x = vextentsX;
                            vpos.y = vextentsX * s;
                        } else {
                            vpos.x = -vextentsX;
                            vpos.y = -vextentsX * s;
                        }
                        if (vpos.y > vextentsY) {
                            vpos.x = vextentsY / s;
                            vpos.y = vextentsY;
                        } else if (vpos.y < -vextentsY) {
                            vpos.x = -vextentsY / s;
                            vpos.y = -vextentsY;
                        }

                        // check collision
                        if (_offScreenIndicatorAvoidOverlap) {
                            float disp = 0;
                            bool vert = vpos.x * vpos.x > vpos.y * vpos.y;
                            int maxj = Mathf.Min(vPosCount, MAX_STORED_VPOS);
                            if (vert) {
                                for (int j = 0; j < maxj; j++) {
                                    float dx = lastVPos[j].x - vpos.x;
                                    if (dx < 0) dx = -dx;
                                    float dy = lastVPos[j].y - vpos.y;
                                    if (dy < 0) dy = -dy;
                                    if (dx < distThreshold && dy < distThreshold) {
                                        if (disp <= 0) {
                                            vpos = lastVPos[j];
                                            disp = _offScreenIndicatorOverlapDistance * overlapDir;
                                        }
                                        vpos.y += disp;
                                        if (vpos.y < -0.4f || vpos.y > 0.4f) break;
                                        j = -1;
                                    }
                                }
                            } else {
                                for (int j = 0; j < maxj; j++) {
                                    float dx = lastVPos[j].x - vpos.x;
                                    if (dx < 0) dx = -dx;
                                    float dy = lastVPos[j].y - vpos.y;
                                    if (dy < 0) dy = -dy;
                                    if (dx < distThreshold && dy < distThreshold) {
                                        if (disp <= 0) {
                                            vpos = lastVPos[j];
                                            disp = _offScreenIndicatorOverlapDistance * overlapDir;
                                        }
                                        vpos.x += disp;
                                        if (vpos.x < -0.4f || vpos.x > 0.4f) break;
                                        j = -1;
                                    }
                                }
                            }
                            overlapDir = -overlapDir;
                            lastVPos[vPosCount++] = vpos;
                        }

                        vpos.x += 0.5f;
                        vpos.y += 0.5f;
                    }
                }

                if (poi.indicatorImage != null) {
                    poi.ToggleIndicatorVisibility(visible);
                    if (!visible) continue;
                } else {
                    if (!visible) continue;

                    // Add a dummy child gameObject
                    GameObject go = GetIndicator();
                    poi.indicatorRT = go.GetComponent<RectTransform>();
                    poi.indicatorCanvasGroup = go.GetComponent<CanvasGroup>();
                    GizmoElements elements = go.GetComponentInChildren<GizmoElements>();
                    if (elements == null) {
                        Debug.LogError("Gizmo prefab missing GizmoElements component.");
                        DestroyImmediate(go);
                        continue;
                    }
                    poi.indicatorImage = elements.iconImage;
                    poi.indicatorDistanceText = elements.distanceText;
                    poi.indicatorTitleText = elements.titleText;
                    poi.indicatorArrowRT = elements.arrowPivot;
                    poi.indicatorRT.localScale = Misc.Vector3zero;
                }

                RectTransform t = poi.indicatorRT;
                scaleVector.x = scaleVector.y = scale;
                Vector3 newScale = Vector3.Lerp(t.localScale, scaleVector, scaleAnimSpeed);
                t.localScale = newScale;

                if (poi.lastIndicatorViewportPos == vpos) continue;
                poi.lastIndicatorViewportPos = vpos;

                poi.indicatorRT.anchorMin = poi.indicatorRT.anchorMax = vpos;
                poi.indicatorImage.sprite = poi.isVisited && poi.iconVisited != null ? poi.iconVisited : poi.iconNonVisited;
                bool distanceVisible = isOnScreen && poi.onScreenIndicatorShowDistance && _onScreenIndicatorShowDistance;
                if (poi.indicatorDistanceText.isActiveAndEnabled != distanceVisible) {
                    poi.indicatorDistanceText.gameObject.SetActive(distanceVisible);
                }
                bool titleVisible = isOnScreen && poi.onScreenIndicatorShowTitle && _onScreenIndicatorShowTitle;
                if (poi.indicatorTitleText.isActiveAndEnabled != titleVisible) {
                    poi.indicatorTitleText.gameObject.SetActive(titleVisible);
                }

                float iconAlpha;
                if (isOnScreen) {
                    float nearFadeMin = poi.onScreenIndicatorNearFadeMin > 0 ? poi.onScreenIndicatorNearFadeMin : _onScreenIndicatorNearFadeMin;
                    float nearFadeDistance = poi.onScreenIndicatorNearFadeDistance > 0 ? poi.onScreenIndicatorNearFadeDistance : _onScreenIndicatorNearFadeDistance;
                    float gizmoAlphaFactor = nearFadeDistance <= nearFadeMin ? 1f : Mathf.Clamp01((poi.distanceToFollow - nearFadeMin) / (nearFadeDistance - nearFadeMin));
                    iconAlpha = _onScreenIndicatorAlpha * gizmoAlphaFactor;
                    if (poi.onScreenIndicatorShowDistance && _onScreenIndicatorShowDistance) {
                        if (poi.prevIndicatorDistance != poi.distanceToFollow) {
                            poi.prevIndicatorDistance = poi.distanceToFollow;
                            poi.lastIndicatorDistanceText = poi.distanceToFollow.ToString(_onScreenIndicatorShowDistanceFormat);
                            poi.indicatorDistanceText.text = poi.lastIndicatorDistanceText;
                        }
                    }

                    if (poi.onScreenIndicatorShowTitle && _onScreenIndicatorShowTitle) {
                        if (!poi.indicatorTitleText.enabled) {
                            poi.indicatorTitleText.enabled = true;
                        }
                        poi.indicatorTitleText.text = poi.title;
                        if (vpos.x > 0.85f) {
                            poi.indicatorTitleText.alignment = TextAlignmentOptions.MidlineRight;
                        } else if (vpos.x < 0.15f) {
                            poi.indicatorTitleText.alignment = TextAlignmentOptions.MidlineLeft;
                        } else {
                            poi.indicatorTitleText.alignment = TextAlignmentOptions.Midline;
                        }
                    }
                } else {
                    iconAlpha = _offScreenIndicatorAlpha;
                    poi.indicatorArrowRT.localRotation = Quaternion.Euler(0, 0, ang * Mathf.Rad2Deg);
                }

                poi.indicatorImage.color = poi.tintColor;
                poi.indicatorCanvasGroup.alpha = iconAlpha;
                poi.indicatorArrowRT.gameObject.SetActive(!isOnScreen);
            }
        }

        GameObject GetIndicator() {
            GameObject indicatorGO = Instantiate(_onScreenIndicatorPrefab, indicatorsRoot, false);
            return indicatorGO;
        }
        #endregion

    }

}

