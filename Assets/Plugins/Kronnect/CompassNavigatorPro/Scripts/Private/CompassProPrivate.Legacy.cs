using System;
using UnityEngine;

namespace CompassNavigatorPro {

    public partial class CompassPro : MonoBehaviour {

        [Obsolete("miniMapFollow is obsolete. Please use 'follow'.")]
        public Transform miniMapFollow { get { return _follow; } set { follow = value; } }

        [Obsolete("miniMapZoomState is obsolete. Please use 'miniMapFullScreenState'.")]
        public bool miniMapZoomState { get { return _miniMapFullScreenState; } set { miniMapFullScreenState = value; } }
    }

}