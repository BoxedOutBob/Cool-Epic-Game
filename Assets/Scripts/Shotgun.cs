using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public ParticleSystem muzzleFlash; // Particle system for muzzle flash
    public GameObject pelletPrefab; // Prefab for the shotgun pellets
    public Transform pelletSpawnPoint; // Point from which the pellets are spawned
    public int pelletCount = 10; // Number of pellets to spawn
    public float pelletSpread = 5f; // Spread angle of the pellets
    public float pelletForce = 20f; // Force applied to the pellets
    public AudioSource shotgunSound; // Audio source for shotgun sound

    [SerializeField]
    private Knockback knockbackScript; // Reference to the knockback script

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button to shoot
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Check if the pelletPrefab is assigned
        if (pelletPrefab == null)
        {
            Debug.LogError("Pellet Prefab is not assigned!");
            return;
        }

        // Play muzzle flash particles
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Play shotgun sound
        if (shotgunSound != null)
        {
            shotgunSound.Play();
        }

        // Create and shoot pellets
        for (int i = 0; i < pelletCount; i++)
        {
            GameObject pelletClone = Instantiate(pelletPrefab, pelletSpawnPoint.position, pelletSpawnPoint.rotation);
            if (pelletClone != null)
            {
                // Attach the Bullet script to the instantiated clone
                pelletClone.AddComponent<Bullet>();

                Rigidbody rb = pelletClone.GetComponent<Rigidbody>();

                // Calculate random spread for each pellet
                Vector3 spread = new Vector3(
                    Random.Range(-pelletSpread, pelletSpread),
                    Random.Range(-pelletSpread, pelletSpread),
                    Random.Range(-pelletSpread, pelletSpread)
                );

                // Apply force to the pellet
                Vector3 direction = pelletSpawnPoint.forward + spread;
                rb.AddForce(direction * pelletForce, ForceMode.Impulse);
            }
        }

        // Apply knockback effect
        if (knockbackScript != null)
        {
            knockbackScript.ApplyKnockback();
        }
    }
}
