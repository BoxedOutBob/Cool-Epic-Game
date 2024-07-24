using UnityEngine;

public class WeaponViewModel : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject weaponPrefab; // The weapon prefab to display
    public float bobAmount = 0.02f; // Amount of bobbing (very slight)
    public float bobSpeed = 1.0f; // Speed of bobbing (slow)
    public int ammo = 30; // Current ammo
    public int ammoReserve = 90; // Total ammo reserve
    public float fireRate = 0.1f; // Fire rate in seconds

    private GameObject weaponInstance; // Instance of the weapon
    private float nextFireTime = 0f; // Time when the weapon can fire next
    private Vector3 initialPosition; // Initial position for bobbing
    private Vector3 weaponBob; // Current bobbing position
    private Camera playerCamera; // Reference to the player's camera

    public Vector3 offset = new Vector3(0.2f, -0.2f, 0.5f); // Adjust this to set the viewmodel position

    private Vector3 currentVelocity; // For SmoothDamp
    private Quaternion currentRotationVelocity; // For smooth rotation

    void Start()
    {
        // Find and set the player camera
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Main camera not found. Please tag your camera as 'MainCamera'.");
            return;
        }

        // Instantiate and position the weapon
        if (weaponPrefab != null)
        {
            weaponInstance = Instantiate(weaponPrefab, transform);
            weaponInstance.transform.localPosition = Vector3.zero;
            initialPosition = weaponInstance.transform.localPosition;
        }
        else
        {
            Debug.LogError("Weapon prefab is not assigned.");
        }
    }

    void LateUpdate()
    {
        if (playerCamera == null || weaponInstance == null) return;

        // Continuously update the viewmodel's position and rotation
        UpdateViewModelPosition();

        HandleWeaponBobbing();
        HandleShooting();
    }

    private void UpdateViewModelPosition()
    {
        // Update the position of the viewmodel to stay in front of the camera with an offset
        Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * offset.z + playerCamera.transform.right * offset.x + playerCamera.transform.up * offset.y;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 0.05f); // Increased damping for smooth transition
        transform.rotation = Quaternion.Slerp(transform.rotation, playerCamera.transform.rotation, Time.deltaTime * 40f); // Faster smooth rotation
    }

    private void HandleWeaponBobbing()
    {
        // Calculate slight bobbing
        float bobbingY = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        weaponBob = new Vector3(0f, bobbingY, 0f);
        weaponInstance.transform.localPosition = initialPosition + weaponBob;
    }

    private void HandleShooting()
    {
        // Fire the weapon
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && ammo > 0)
        {
            // Print statement as placeholder for shooting logic
            Debug.Log("Weapon Fired!");

            // Reduce ammo and set next fire time
            ammo--;
            nextFireTime = Time.time + fireRate;

            // Optionally handle ammo reserve
            if (ammo <= 0 && ammoReserve > 0)
            {
                // Example: Refill ammo from reserve
                ammo = Mathf.Min(30, ammoReserve);
                ammoReserve -= ammo;
            }
        }
    }
}
