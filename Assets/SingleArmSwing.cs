using UnityEngine;

public class SingleArmSwing : MonoBehaviour
{
    public Transform arm;            // Reference to the arm
    public Transform gun;            // Reference to the gun
    public float swingSpeed = 5f;    // Speed of arm swing while walking
    public float swingAngle = 30f;   // Maximum swing angle for walking
    public float shootRecoilAmount = 0.5f; // How much the arm and gun move up when shooting
    public float recoilSpeed = 5f;   // Speed at which the arm and gun return after shooting

    private float armInitialY;       // Arm's initial Y position
    private float gunInitialY;       // Gun's initial Y position
    private bool isWalking = false;  // Check if the player is walking
    private bool isRecoiling = false;

    void Start()
    {
        // Store the initial Y positions of the arm and gun
        armInitialY = arm.localPosition.y;
        gunInitialY = gun.localPosition.y;
    }

    void Update()
    {
        HandleWalkingMotion();
        HandleShooting();
    }

    void HandleWalkingMotion()
    {
        // Check if the player is moving
        isWalking = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (isWalking && !isRecoiling) // Only apply walking motion if not recoiling
        {
            float swingOffset = Mathf.Sin(Time.time * swingSpeed) * swingAngle / 100f;
            arm.localPosition = new Vector3(arm.localPosition.x, armInitialY + swingOffset, arm.localPosition.z);
        }
        else if (!isRecoiling) // Reset to initial position if not walking
        {
            arm.localPosition = new Vector3(arm.localPosition.x, armInitialY, arm.localPosition.z);
        }
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isRecoiling)
        {
            StartCoroutine(ArmAndGunRecoil());
        }
    }

    System.Collections.IEnumerator ArmAndGunRecoil()
    {
        isRecoiling = true;

        // Move arm and gun upward (recoil)
        float elapsedTime = 0f;
        while (elapsedTime < recoilSpeed)
        {
            arm.localPosition = new Vector3(
                arm.localPosition.x,
                arm.localPosition.y + shootRecoilAmount * Time.deltaTime,
                arm.localPosition.z
            );
            gun.localPosition = new Vector3(
                gun.localPosition.x,
                gun.localPosition.y + shootRecoilAmount * Time.deltaTime,
                gun.localPosition.z
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset arm and gun to original positions
        elapsedTime = 0f;
        while (elapsedTime < recoilSpeed)
        {
            arm.localPosition = new Vector3(
                arm.localPosition.x,
                Mathf.Lerp(arm.localPosition.y, armInitialY, elapsedTime / recoilSpeed),
                arm.localPosition.z
            );
            gun.localPosition = new Vector3(
                gun.localPosition.x,
                Mathf.Lerp(gun.localPosition.y, gunInitialY, elapsedTime / recoilSpeed),
                gun.localPosition.z
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        arm.localPosition = new Vector3(arm.localPosition.x, armInitialY, arm.localPosition.z);
        gun.localPosition = new Vector3(gun.localPosition.x, gunInitialY, gun.localPosition.z);
        isRecoiling = false;
    }
}