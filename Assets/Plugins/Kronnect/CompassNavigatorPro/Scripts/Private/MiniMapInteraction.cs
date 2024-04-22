using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CompassNavigatorPro {

    public class MiniMapInteraction : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

        [NonSerialized]
        public CompassPro compass;

        bool isDragging;
        Vector3 dragStartWorldPosition;
        Vector3 dampDir;
        int dragEndFrame;

        public void OnPointerEnter(PointerEventData eventData) {
            if (compass != null) {
                compass.BubbleEvent(compass.OnMiniMapMouseEnter, eventData.position);
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (compass != null) {
                compass.BubbleEvent(compass.OnMiniMapMouseExit, eventData.position);
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (compass != null && !isDragging && dragEndFrame != Time.frameCount && compass.miniMapIconEvents) {
                Vector3 worldPos = compass.GetWorldPositionFromPointerEvent(eventData.position);
                compass.BubbleEvent(compass.OnMiniMapMouseClick, worldPos, (int)eventData.button);
            }
            isDragging = false;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (eventData.button != 0) return;
            isDragging = true;
            if (compass != null && compass.currentMiniMapAllowsUserDrag) {
                dragStartWorldPosition = compass.GetWorldPositionFromPointerEvent(eventData.position);

            }
        }

        public void OnDrag(PointerEventData eventData) {
            if (isDragging && compass != null && compass.currentMiniMapAllowsUserDrag) {
                Vector3 worldPos = compass.GetWorldPositionFromPointerEvent(eventData.position);
                dampDir = worldPos - dragStartWorldPosition;
                UpdateOffset();
            }
        }

        void UpdateOffset() {
            if (compass == null) return;
            compass.miniMapFollowOffset -= dampDir;
        }

        public void OnEndDrag(PointerEventData eventData) {
            isDragging = false;
            dragEndFrame = Time.frameCount;
        }
    }


}