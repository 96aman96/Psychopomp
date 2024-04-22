using UnityEngine;
using System.Collections;
using CompassNavigatorPro;

namespace CompassNavigatorProDemos
{
    public class LevelManager : MonoBehaviour {

        public int initialPoiCount = 1;
        public Material sphereMaterial;
        public Sprite[] icons;
        public AudioClip[] soundClips;

        int poiNumber;
        CompassPro compass;
        CompassProPOI POIUnderMouse;

        IEnumerator Start() {
            // Get a reference to the Compass Pro Navigator component
            compass = CompassPro.instance;

            // Add a callback when POIs are reached
            compass.OnPOIVisited += OnPOIVisited;

            // Subscribe to Compass minimap events
            compass.OnMiniMapMouseClick += OnMiniMapClick;
            compass.OnPOIMiniMapIconMouseEnter += OnPOIHover;
            compass.OnPOIMiniMapIconMouseExit += OnPOIExit;
            compass.OnPOIMiniMapIconMouseClick += OnPOIClick;
            compass.OnPOIEnterCircle += OnPOIEnterCircle;
            compass.OnPOIExitCircle += OnPOIExitCircle;

            // Populate the scene with initial POIs
            WaitForSeconds w = new WaitForSeconds(0.5f);
            for (int k = 1; k <= initialPoiCount; k++) {
                yield return w;
                AddRandomPOI();
            }
        }

        void Update() {

            if (Input.GetKeyDown(KeyCode.B)) {
                compass.POIShowBeacon(5f, 1.1f, 1f, new Color(1, 1, 0.25f));
            }
            if (Input.GetKey(KeyCode.Z)) {
                compass.miniMapZoomLevel -= Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.X)) {
                compass.miniMapZoomLevel += Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                compass.showCompassBar = !compass.showCompassBar;
            }
            if (Input.GetKeyDown(KeyCode.M)) {
                compass.showMiniMap = !compass.showMiniMap;
            }
            if (Input.GetKeyDown(KeyCode.V)) {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    compass.POIShowBeacon(hit.point, 5f, 0.4f, 1f, new Color(0, 0.5f, 1f));
                }
            }
            if (Input.GetKeyDown(KeyCode.T)) {
                compass.miniMapFullScreenState = !compass.miniMapFullScreenState;
            }
            if (Input.GetKeyDown(KeyCode.H)) {
                foreach (var poi in compass.pois) {
                    poi.StartCircleAnimation();
                }
            }
        }

        void OnGUI() {
            if (POIUnderMouse != null) {
                Rect rect = POIUnderMouse.GetMiniMapIconScreenRect();
                rect = new Rect(rect.center.x - 100, Screen.height - rect.y - 85, 200, 25);
                GUIStyle style = GUI.skin.GetStyle("Label");
                style.alignment = TextAnchor.UpperCenter;
                style.normal.textColor = Color.yellow;
                GUI.Label(rect, POIUnderMouse.title, style);
            }
        }

        void AddRandomPOI() {
            Vector3 position = new Vector3(Random.Range(-50, 50), 1, Random.Range(-50, 50));
            AddPOI(position);
        }


        void AddPOI(Vector3 position) {
            // Create placeholder
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = position;
            obj.GetComponent<Renderer>().material = sphereMaterial;

            // Add POI info
            CompassProPOI poi = obj.AddComponent<CompassProPOI>();

            // Title name and reveal text
            poi.title = "Target " + (++poiNumber).ToString();
            poi.titleVisibility = TitleVisibility.Always;
            poi.visitedText = "Target " + poiNumber + " acquired!";

            // Assign icons
            int j = Random.Range(0, icons.Length / 2);
            poi.iconNonVisited = icons[j * 2];
            poi.iconVisited = icons[j * 2 + 1];

            // Enable indicators
            poi.showOnScreenIndicator = true;
            poi.showOffScreenIndicator = true;

            // Hide poi when visited and random sound
            poi.hideWhenVisited = true;
            j = Random.Range(0, soundClips.Length);
            poi.visitedAudioClipOverride = soundClips[j];

            // Make a circle animation appear
            poi.miniMapCircleAnimationWhenAppears = true;
            poi.miniMapCircleAnimationRepetitions = 1;
       }

        void OnPOIVisited(CompassProPOI poi) {
            Debug.Log($"{poi.title} has been reached.");
            StartCoroutine(PickUpPOI(poi));
        }

        IEnumerator PickUpPOI(CompassProPOI poi) {
            while (poi.transform.position.y < 5) {
                poi.transform.position += Vector3.up * Time.deltaTime;
                poi.transform.localScale *= 0.9f;
                yield return null;
            }
            Destroy(poi.gameObject);
            AddRandomPOI();
        }

        void OnPOIClick(CompassProPOI poi, int mouseButtonIndex) {
            Debug.Log(poi.title + " has been clicked on minimap.");
            if (mouseButtonIndex == 1) {
                Debug.Log($"Removing POI...");
                Destroy(poi.gameObject);
            }
        }

        void OnPOIHover(CompassProPOI poi) {
            POIUnderMouse = poi;
        }

        void OnPOIExit(CompassProPOI poi) {
            POIUnderMouse = null;
        }

        void OnMiniMapClick(Vector3 position, int buttonIndex) {
            if (buttonIndex == 0) {
                Debug.Log($"User clicked on mini-map. Creating a POI at world position: {position}");
                position.y = 1f;
                AddPOI(position);
            } else {
                // re-center mini-map with secondary mouse button
                compass.ResetDragOffset();
            }
        }

        void OnPOIEnterCircle(CompassProPOI poi) {
            Debug.Log($"Entering circle of poi {poi.title}");
        }

        void OnPOIExitCircle(CompassProPOI poi) {
            Debug.Log($"Exiting circle of poi {poi.title}");
        }


    }
}