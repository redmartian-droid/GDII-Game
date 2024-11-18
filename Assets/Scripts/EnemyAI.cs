using UnityEngine;
using UnityEngine.UI; // For UI Text
using TMPro; // Optional, if using TextMeshPro
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3.0f;
    public float health = 100f;
    public GameObject endGameScreen;
    public float activationDistance = 10.0f;
    public float contactInterval = 5.0f;
    public GameObject[] incrementObjects;
    public GotHitEffect gotHitEffect;
    public float knockbackForce = 5.0f;
    public float knockbackDuration = 0.5f;
    public TMP_Text livesText; // Reference to the UI Text
    public int maxLives = 5; // Maximum number of lives
    public AudioClip hitSound; // The sound to play when the player is hit

    private Transform player;
    private int lives; // Current number of lives
    private int contactCount = 0;
    private bool canIncrement = true;
    private float lastContactTime;
    private AudioSource audioSource; // Reference to the AudioSource component

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastContactTime = -contactInterval;
        lives = maxLives; // Initialize lives
        UpdateLivesText(); // Initialize UI text

        foreach (GameObject obj in incrementObjects)
        {
            obj.SetActive(false);
        }

        // Initialize the AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // Ensure the sound doesn't play on start
    }

    void Update()
    {
        // Decrease counter if more than 5 seconds passed since the last contact and count is above 0
        if (Time.time - lastContactTime >= contactInterval && contactCount > 0)
        {
            contactCount--;
            lastContactTime = Time.time;
            UpdateIncrementObjects();
        }

        // Move towards the player if within activation distance
        if (player != null && Vector3.Distance(transform.position, player.position) <= activationDistance)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        Vector3 direction = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Trigger the "Got Hit" screen effect
            gotHitEffect?.TriggerHitEffect();

            if (canIncrement)
            {
                // Increment contact count
                contactCount++;
                lastContactTime = Time.time;
                UpdateIncrementObjects();
                StartCoroutine(KnockbackPlayer());

                LoseLife(); // Deduct a life when the player is hit

                // Play the hit sound effect
                if (hitSound != null)
                {
                    audioSource.PlayOneShot(hitSound);
                }
            }
        }
    }

    private IEnumerator KnockbackPlayer()
    {
        Vector3 knockbackDirection = transform.position - player.position;
        knockbackDirection.y = 0;
        knockbackDirection.Normalize();

        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            transform.position += knockbackDirection * knockbackForce * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Add a cooldown between knockbacks to allow the enemy to collide again
        yield return new WaitForSeconds(0.1f);
    }

    private void UpdateIncrementObjects()
    {
        for (int i = 0; i < incrementObjects.Length; i++)
        {
            incrementObjects[i].SetActive(i < contactCount);
        }
    }

    private void LoseLife()
    {
        lives--; // Decrement lives
        UpdateLivesText(); // Update the UI text

        if (lives <= 0)
        {
            EndGame();
        }
    }

    private void UpdateLivesText()
    {
        // Update the text to reflect the current number of lives
        livesText.text = "Lives: " + lives;
    }

    private void EndGame()
    {
        Time.timeScale = 0f;

        if (endGameScreen != null)
        {
            endGameScreen.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
