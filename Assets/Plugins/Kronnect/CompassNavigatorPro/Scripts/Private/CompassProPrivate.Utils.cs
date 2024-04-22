using UnityEngine;

namespace CompassNavigatorPro {

    public partial class CompassPro : MonoBehaviour {

        void DestroySafe(Object o) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                DestroyImmediate(o);
            }
            else
#endif
            {
                Destroy(o);
            }
        }

    }

}



