using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float pelletLifetime = 4f; // Lifetime of the pellets before they are destroyed

    void Start()
    {
        // Destroy the bullet after a specified lifetime
        Destroy(gameObject, pelletLifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object has a Rigidbody or a CharacterController
        if (collision.gameObject.GetComponent<Rigidbody>() != null || collision.gameObject.GetComponent<CharacterController>() != null)
        {
            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
