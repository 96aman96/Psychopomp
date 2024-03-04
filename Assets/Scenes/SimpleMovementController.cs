using UnityEngine;

public class SimpleMovementController : MonoBehaviour
{
    public float regularSpeed;
     float speed = 5.0f; // Speed of the character
    public float fastSpeed;
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // Get left/right input
        float moveVertical = Input.GetAxis("Vertical"); // Get forward/backward input

        // Calculate movement direction
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Add up/down movement based on keys (change "Jump" and "Crouch" to your preferred keys in the Input Manager if needed)
        if(Input.GetKey(KeyCode.E)) // Up
        {
            movement.y += 1;
        }
        else if(Input.GetKey(KeyCode.Q)) // Down
        {
            movement.y -= 1;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = fastSpeed;
        }
        else
        {

            speed = regularSpeed;
        }
        // Apply the movement to the character
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}