using UnityEngine;

public class Firearm : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public GameObject fpsCam;  // Reference to the camera used for raycasting
    public ParticleSystem GunFlash;
    public AudioSource gunSound;  // Reference to the AudioSource component

    private int currentAmmo = 10;  // Ammo count
    private float nextTimeToFire = 0f;  // Controls fire rate
    public float fireRate = 15f;  // Rate of fire
    internal int ammoCache;  // Ammo cache for reloading

    void Start()
    {
        // If the AudioSource component is not assigned via the Inspector, attempt to find it on the same GameObject
        if (gunSound == null)
        {
            gunSound = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Check if the fire button is pressed, enough time has passed since last shot, and ammo is available
        if (Input.GetButtonDown("Shoot") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            GunFlash.Play();  // Play the muzzle flash particle effect
            gunSound.Play();  // Play the shooting sound effect
        }
    }

    void Shoot()
    {
        currentAmmo--;  // Decrease ammo count when shooting

        RaycastHit hit;
        Vector3 shootDirection = fpsCam.transform.forward; // Get the forward direction of the camera
        Debug.DrawRay(fpsCam.transform.position, shootDirection * range, Color.red, 1f); // Draw the ray for debugging

        if (Physics.Raycast(fpsCam.transform.position, shootDirection, out hit, range))
        {
            // Check if the hit object has the EnemyAI component
            EnemyAI target = hit.transform.GetComponent<EnemyAI>();
            if (target != null)
            {
                // Apply damage to the enemy
                target.TakeDamage(damage);
            }
        }
    }
}
