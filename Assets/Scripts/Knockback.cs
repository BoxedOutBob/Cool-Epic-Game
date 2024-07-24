using UnityEngine;

public class Knockback : MonoBehaviour
{
    public CharacterController characterController; // The CharacterController component of the player
    public Camera playerCamera; // The camera that the player is using
    public Collider playerCollider; // The player's collider
    public float knockbackForce = 10f; // The amount of force applied for knockback
    public float knockbackDuration = 1f; // Duration of knockback effect
    public float friction = 5f; // The amount of friction applied to slow down sliding
    public float groundCheckDistance = 1.5f; // Distance for raycast to check for ground

    private Vector3 knockbackDirection;
    private bool isKnockedBack;
    private float knockbackTimer;
    private Vector3 currentVelocity;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isKnockedBack)
        {
            ApplyKnockback();
        }

        if (isKnockedBack)
        {
            // Apply knockback effect by moving the character controller
            Vector3 moveDirection = knockbackDirection * knockbackForce * Time.deltaTime;
            currentVelocity = moveDirection;
            characterController.Move(moveDirection);

            // Apply friction to gradually reduce movement
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, friction * Time.deltaTime);
            characterController.Move(currentVelocity * Time.deltaTime);

            // Update knockback timer
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                StopKnockback(); // Stop the knockback after the duration
            }
        }

        // Stop knockback if grounded
        if (isKnockedBack && IsGrounded())
        {
            StopKnockback();
        }
    }

    public void ApplyKnockback()
    {
        // Check if CharacterController, Camera, and Collider are assigned
        if (characterController == null || playerCamera == null || playerCollider == null)
        {
            Debug.LogError("CharacterController, Camera, or Collider not assigned!");
            return;
        }

        // Calculate the direction of knockback (opposite of camera forward direction)
        knockbackDirection = -playerCamera.transform.forward;
        isKnockedBack = true;
        knockbackTimer = knockbackDuration; // Reset the knockback timer
    }

    private bool IsGrounded()
    {
        // Get the bottom position of the collider
        Vector3 colliderBottom = playerCollider.bounds.center - new Vector3(0, playerCollider.bounds.extents.y, 0);

        // Cast a ray downward from the bottom of the collider
        RaycastHit hit;
        return Physics.Raycast(colliderBottom, Vector3.down, out hit, groundCheckDistance);
    }

    private void StopKnockback()
    {
        isKnockedBack = false;
        // Stop any residual movement
        characterController.Move(Vector3.zero);
    }
}
