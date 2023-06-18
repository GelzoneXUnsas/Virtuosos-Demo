using UnityEngine;

public class BounceHard : MonoBehaviour
{
    public float bounceHeight; // The maximum height of the bounce
    public float bounceSpeed; // The speed of the bounce

    private Vector3 initialPosition;

    public void Start()
    {
        // Store the initial position of the GameObject
        initialPosition = transform.position;
    }

    public void Update()
    {
        // Calculate the new vertical position based on time and bounce parameters
        float newY = initialPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

        // Update the position of the GameObject with the new vertical position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
