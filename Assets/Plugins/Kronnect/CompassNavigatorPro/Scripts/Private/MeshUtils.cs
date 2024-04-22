using System.Collections.Generic;
using UnityEngine;

namespace CompassNavigatorPro {

    public static class MeshUtils {

        static Mesh _quadMesh;

        public static Mesh quadMesh {
            get {
                if (_quadMesh != null) {
                    return _quadMesh;
                }
                const float num = 1f;
                const float num2 = 0f;
                Mesh val = new Mesh();
                _quadMesh = val;
                _quadMesh.SetVertices(new List<Vector3> {
            new Vector3 (-1f, -1f, 0f),
            new Vector3 (-1f, 1f, 0f),
            new Vector3 (1f, -1f, 0f),
            new Vector3 (1f, 1f, 0f)
        });
                _quadMesh.SetUVs(0, new List<Vector2> {
            new Vector2 (0f, num2),
            new Vector2 (0f, num),
            new Vector2 (1f, num2),
            new Vector2 (1f, num)
        });
                _quadMesh.SetIndices(new int[6] { 0, 1, 2, 2, 1, 3 }, 0, 0, false);
                _quadMesh.UploadMeshData(true);
                return _quadMesh;
            }
        }
    }


}