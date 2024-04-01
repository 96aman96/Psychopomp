using UnityEngine;
using Hipernt;

namespace Hipernt
{
    public class MoveCamera : MonoBehaviour
    {
        public Transform player;

        void Update()
        {
            transform.position = player.transform.position;
        }
    }
}
