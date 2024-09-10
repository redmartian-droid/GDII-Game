using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Firearm : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15;

    public GameObject fpsCam;                   // the point of shooting
    public ParticleSystem muzzleFlash;          // particle effect for muzzle flash
    public GameObject impactEffect;             // bullet impact effect
    public GameObject bulletCasing;             // eject used casing
    public Transform casinglocation;            // where the casing gets ejected
    public AudioSource weaponSound;             // weapon sound effect
    public AudioSource noAmmoSound;             // empty gun sound 
    public AudioSource reloadSound;             // reload sound 

    public Animator anim;                       // animations for weapons
    public Vector3 reloading;                   // new position for reload
    public float reloadTime = 3;                // time it takes to reload
    public Vector3 upRecoil;                    // new position for recoil
    Vector3 originalRotation;                   // original position

    public float amount;                        // swaying min amount
    public float maxAmount;                     // swaying max amount
    public float smoothAmount;                  // smooth time for swaying

    private Vector3 initialPosition;            // original position before swaying

    public GameObject ammoText;                 // ammo text

    private int currentAmmo;                    // the current ammo in weapon
    public int magazineSize = 10;               // how much ammo is in each mag
    public int ammoCache = 20;                  // how much ammo is in your cache (storage)
    private int maxAmmo;                        // max ammo is private maxAmmo = mag size
    private int ammoNeeded;                     // ammo counter for how much is needed, you shoot 5 bullets, you need 5

    public bool semi;                           // is the weapon semi
    public bool auto;                           // is the weapon auto
    public bool melee;                          // is the weapon melee

    // there can be a bug where the casing goes reversed, these two bools will fix it:

    public bool casingForward;                  // get correct orientation of casings
    public bool casingBackwards;                // get correct orientation of casings

    private bool isreloading;                   // is the weapon reloading
    private bool canShoot;                      // is the weapon able to be shot

    private float nextTimeToFire = 0f;          // how much time must pass before shooting/meleeing again

    // start function to ensure theres no bugs:

    void Start()
    {
        currentAmmo = magazineSize;
        maxAmmo = magazineSize;

        isreloading = false;
        canShoot = true;

        originalRotation = transform.localEulerAngles;
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // for semi auto weapons:

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && semi && magazineSize > 0 && canShoot)
        {
            AddRecoil();
            nextTimeToFire = Time.time + 1f / fireRate;
            anim.SetBool("shoot", true);
            Invoke("setboolback", .5f);
            Shoot();
        }

        // for weapons with melee:

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && melee)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            anim.SetBool("melee", true);
            Invoke("setboolback", .5f);
            Melee();
        }

        // for auto weapons:

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && magazineSize > 0 && auto && canShoot)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            anim.SetBool("shoot", true);
            Invoke("setboolback", .5f);
            AddRecoilAuto();
            Shoot();
        }

        // checks for 0 ammo:

        if (Input.GetButton("Fire1") && magazineSize == 0)
        {
            noAmmoSound.Play();
        }
        else if (Input.GetButtonUp("Fire1") && canShoot)
        {
            StopRecoil();
        }

        // changing animation weapon ammo 0:

        if (magazineSize == 0)
        {
            ammoText.GetComponent<Text>().text = "Reload";
            anim.SetBool("empty", true);
        }

        if (magazineSize > 0)
        {
            anim.SetBool("empty", false);
        }

        // reloading:

        if (Input.GetButtonDown("reload") && magazineSize == 0 && ammoCache > 0)
        {
            canShoot = false;
            ammoCache -= ammoNeeded;
            magazineSize += ammoNeeded;
            ammoNeeded -= ammoNeeded;
            isreloading = true;
            StartCoroutine(ReloadTimer());
        }

        // stops bugs with pressing reload more than once:

        else if (isreloading)
        {
            return;
        }

        // doesn't reload if cache is 0:

        if (Input.GetButtonDown("reload") && ammoCache == 0)
        {
            return;
        }

        // tells our text object what to say:

        ammoText.GetComponent<Text>().text = magazineSize + " / " + ammoCache;

        // our swaying function being put to action:

        float movementX = -Input.GetAxis("Mouse X") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        // making sure the sway goes back to original position:

        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }

    // if our weapon is a gun:

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range));
        {
            magazineSize--;
            ammoNeeded++;

            muzzleFlash.Play();
            weaponSound.Play();

            GameObject casing = Instantiate(bulletCasing, casinglocation.position, casinglocation.rotation);

            Destroy(casing, 2f);

            EnemyAI target = hit.transform.GetComponent<EnemyAI>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            GameObject impactOB = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));

            Destroy(impactOB, 2f);

            if (casingForward)
            {
                casing.GetComponent<Rigidbody>().AddForce(transform.forward * 250);
            }

            if (casingBackwards)
            {
                casing.GetComponent<Rigidbody>().AddForce(transform.forward * -250);
            }
        }
    }

    // if our weapon is melee:

    void Melee()
    {
        weaponSound.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range));

        EnemyAI target = hit.transform.GetComponent<EnemyAI>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }

    // setting animation bools back to normal:

    public void setboolback()
    {
        anim.SetBool("shoot", false);
        anim.SetBool("melee", false);
    }

    // adding recoil position when shot semi weapons:

    public void AddRecoil()
    {
        if (canShoot)
        {
            transform.localEulerAngles += upRecoil;
            StartCoroutine(StopRecoilSemi());
        }
    }

    // adding recoil position when shot auto weapons:

    public void AddRecoilAuto()
    {
        if (canShoot)
        {
            transform.localEulerAngles += upRecoil;
            StartCoroutine(StopRecoilSemi());
        }
    }

    // stopping recoil:

    public void StopRecoil()
    {
        transform.localEulerAngles = originalRotation;
    }

    // stopping recoil (fixing bugs)

    IEnumerator StopRecoilSemi()
    {
        yield return new WaitForSeconds(.1f);
        transform.localEulerAngles = originalRotation;
    }

    // our reload timer:

    IEnumerator ReloadTimer()
    {
        reloadSound.Play();
        transform.localEulerAngles += reloading;
        yield return new WaitForSeconds(reloadTime);
        isreloading = false;
        canShoot = true;
        transform.localEulerAngles = originalRotation;
    }
}
