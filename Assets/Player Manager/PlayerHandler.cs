using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 8f;
    public float runSpeed = 12f;
    public float defaultFOV = 70f;
    public float runFOV = 72f;
    public float bobbingSpeed = 1f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float mouseSensitivity = 100f;
    public float stopDamping = 20f; // Added damping to stop sliding
    public float airControlMultiplier = 0.5f; // Control over air movement
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public KeyCode dashKey = KeyCode.LeftShift;

    private Camera playerCamera;
    private CharacterController characterController;
    private Vector3 velocity;
    private float speed;
    private float currentFOV;
    private Vector3 originalCameraPosition;
    private float timer = 0f;
    private bool isGrounded;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private float dashTime;

    private AudioSource audioSource;
    public AudioClip walkClip;

    private void Start()
    {
        playerCamera = Camera.main;
        characterController = GetComponent<CharacterController>();
        originalCameraPosition = playerCamera.transform.localPosition;
        currentFOV = defaultFOV;
        playerCamera.fieldOfView = currentFOV;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;
        HandleMovement();
        HandleCameraBob();
        HandleFOV();
        ApplyGravity();
        characterController.Move(velocity * Time.deltaTime);
        HandleMouseLook();
        PlayMovementSound();
    }

    private void HandleMovement()
    {
        if (isDashing)
        {
            Dash();
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = horizontal != 0 || vertical != 0;

        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        if (move.magnitude > 1)
        {
            move.Normalize(); // Normalize the movement vector to maintain consistent speed
        }

        speed = (Input.GetKey(KeyCode.LeftShift) && isMoving && isGrounded) ? runSpeed : walkSpeed;

        if (isGrounded)
        {
            if (isMoving)
            {
                velocity.x = move.x * speed;
                velocity.z = move.z * speed;
            }
            else
            {
                // Apply damping to stop the player instantly
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * stopDamping);
                velocity.z = Mathf.Lerp(velocity.z, 0, Time.deltaTime * stopDamping);
            }

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // Limit movement in the air
            velocity.x = Mathf.Lerp(velocity.x, move.x * speed * airControlMultiplier, Time.deltaTime * stopDamping);
            velocity.z = Mathf.Lerp(velocity.z, move.z * speed * airControlMultiplier, Time.deltaTime * stopDamping);
        }

        if (Input.GetKeyDown(dashKey))
        {
            dashDirection = move.normalized;
            if (dashDirection != Vector3.zero)
            {
                isDashing = true;
                dashTime = dashDuration;
                velocity.y = 0; // Reset y velocity to ensure a smooth dash
            }
        }
    }

    private void Dash()
    {
        if (dashTime > 0)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTime -= Time.deltaTime;
        }
        else
        {
            isDashing = false;
        }
    }

    private void HandleCameraBob()
    {
        if (characterController.velocity.magnitude > 0.1f)
        {
            timer += Time.deltaTime * bobbingSpeed;
            float bobAmount = Mathf.Sin(timer) * 0.1f;
            playerCamera.transform.localPosition = new Vector3(originalCameraPosition.x, originalCameraPosition.y + bobAmount, originalCameraPosition.z);
        }
        else
        {
            timer = 0f;
            playerCamera.transform.localPosition = originalCameraPosition;
        }
    }

    private void HandleFOV()
    {
        if (characterController.velocity.magnitude > walkSpeed)
        {
            currentFOV = Mathf.Lerp(currentFOV, runFOV, Time.deltaTime * 5f);
        }
        else
        {
            currentFOV = Mathf.Lerp(currentFOV, defaultFOV, Time.deltaTime * 5f);
        }
        playerCamera.fieldOfView = currentFOV;
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void PlayMovementSound()
    {
        // Check if the player is grounded
        if (isGrounded)
        {
            // Check if the player is moving
            bool isMoving = characterController.velocity.magnitude > 0.1f;

            if (isMoving)
            {
                if (!audioSource.isPlaying || audioSource.clip != walkClip)
                {
                    // Play walking sound
                    audioSource.clip = walkClip;
                    audioSource.Play();
                }

                // Adjust the pitch based on speed
                if (characterController.velocity.magnitude > walkSpeed) // Running
                {
                    audioSource.pitch = 1.25f; // Increase pitch for running
                }
                else // Walking
                {
                    audioSource.pitch = 1.0f; // Normal pitch for walking
                }
            }
            else
            {
                // Stop the sound if the player is not moving
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
        else
        {
            // Stop the sound if the player is in the air
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
