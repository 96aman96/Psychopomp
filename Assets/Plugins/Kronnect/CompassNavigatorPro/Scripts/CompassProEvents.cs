using System;
using UnityEngine;

namespace CompassNavigatorPro {


    public partial class CompassPro : MonoBehaviour {

        #region Generic events

        /// <summary>
        /// Event fired when a POI is added to the compass system
        /// </summary>
        public Action<CompassProPOI> OnPOIRegister;

        /// <summary>
        /// Event fired when a POI is removed from the compass system
        /// </summary>
        public Action<CompassProPOI> OnPOIUnregister;

        /// <summary>
        /// Event fired when this POI is visited.
        /// </summary>
        public Action<CompassProPOI> OnPOIVisited;

        /// <summary>
        /// Event fired when player enters the circle of a POI
        /// </summary>
        public Action<CompassProPOI> OnPOIEnterCircle;

        /// <summary>
        /// Event fired when a player exits the circle of a POI
        /// </summary>
        public Action<CompassProPOI> OnPOIExitCircle;

        #endregion


        #region Compass events

        /// <summary>
        /// Event fired when the POI appears in the compass bar (gets near than the visible distance)
        /// </summary>
        public Action<CompassProPOI> OnPOIVisible;

        /// <summary>
        /// Event fired when POI disappears from the compass bar (gets farther than the visible distance)
        /// </summary>
        public Action<CompassProPOI> OnPOIHide;

        #endregion


        #region Mini-map events

        /// <summary>
        /// Event fired when mouse enters an icon on the miniMap
        /// </summary>
        public Action<CompassProPOI> OnPOIMiniMapIconMouseEnter;

        /// <summary>
        /// Event fired when mouse exits an icon on the miniMap
        /// </summary>
        public Action<CompassProPOI> OnPOIMiniMapIconMouseExit;

        /// <summary>
        /// Event fired when button is pressed on an icon on the miniMap
        /// </summary>
        public Action<CompassProPOI, int> OnPOIMiniMapIconMouseDown;

        /// <summary>
        /// Event fired when button is released on an icon on the miniMap
        /// </summary>
        public Action<CompassProPOI, int> OnPOIMiniMapIconMouseUp;

        /// <summary>
        /// Event fired when an icon is clicked on the minimap
        /// </summary>
        public Action<CompassProPOI, int> OnPOIMiniMapIconMouseClick;

        /// <summary>
        /// Event fired when this POI appears in the Mini-Map
        /// </summary>
        public Action<CompassProPOI> OnPOIVisibleInMiniMap;

        /// <summary>
        /// Event fired when the POI disappears from the Mini-Map
        /// </summary>
        public Action<CompassProPOI> OnPOIHidesInMiniMap;

        /// <summary>
        /// Event fired when full screen mode changes
        /// </summary>
        public Action<bool> OnMiniMapChangeFullScreenState;

        /// <summary>
        /// Event fired when user clicks on minimap - sends the world space position of click
        /// </summary>
        public Action<Vector3, int> OnMiniMapMouseClick;

        /// <summary>
        /// Event fired when mouse enters minimap area in the screen
        /// </summary>
        public Action<Vector2> OnMiniMapMouseEnter;

        /// <summary>
        /// Event fired when mouse exits minimap area in the screen
        /// </summary>
        public Action<Vector2> OnMiniMapMouseExit;

        #endregion


        #region Indicators events

        /// <summary>
        /// Event fired when the indicator of this POI appears on screen
        /// </summary>
        public Action<CompassProPOI> OnPOIOnScreen;

        /// <summary>
        /// Event fired when the indicator of this POI goes off-screen
        /// </summary>
        public Action<CompassProPOI> OnPOIOffScreen;


        #endregion


    }

}



