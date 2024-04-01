using UnityEngine;

namespace Hipernt
{
    public class FollowPlayer : MonoBehaviour
    {
        public string playerTag = "Player";

        private Transform player;

        void Start()
        {
            // Find the player GameObject using the tag
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

            // Check if the player object is found
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found with tag: " + playerTag);
            }
        }

        void Update()
        {
            if (player != null)
            {
                // Set the object's position to the player's position
                transform.position = player.position;
            }
            // No need for an else statement here since player is set in Start
        }
    }
}
