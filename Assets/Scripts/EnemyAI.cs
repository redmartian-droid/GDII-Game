using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3.0f;
    public float health = 100f;
    public GameObject endGameScreen;
    public float activationDistance = 10.0f; // Distance within which the enemy activates
    public float contactInterval = 5.0f; // Time limit for each contact to remain valid
    public GameObject[] incrementObjects; // Array of objects to enable/disable based on contact count

    private Transform player;
    private int contactCount = 0; // Counter for contacts with the player
    private bool canIncrement = true; // Check if the counter can be incremented
    private float lastContactTime; // Time of the last contact

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastContactTime = -contactInterval; // Initialize to ensure no initial increment
        Debug.Log("Enemy AI initialized. Contact count starts at: " + contactCount);

        // Ensure all increment objects start disabled
        foreach (GameObject obj in incrementObjects)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        // Decrease counter if more than 5 seconds passed since the last contact and count is above 0 (this is meant to provide the player more breathing room)
        if (Time.time - lastContactTime >= contactInterval && contactCount > 0)
        {
            contactCount--;
            lastContactTime = Time.time;
            Debug.Log("Contact count decreased due to timeout. Current count: " + contactCount);
            UpdateIncrementObjects();
        }

        // Check if the player is within the activation distance
        if (player != null && Vector3.Distance(transform.position, player.position) <= activationDistance)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        Vector3 direction = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
        direction.Normalize(); // Normalize the direction to prevent faster movement when further away
        transform.position += direction * speed * Time.deltaTime; // Move enemy towards player
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Damage taken");

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy has died.");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player contact detected.");
            if (canIncrement)
            {
                // Increment contact count if within the interval (if enemy makes contact with player again within 5 seconds this count will increase until a maximum value of 4, in which case the endGame function will be called)
                contactCount++;
                lastContactTime = Time.time; // Reset the contact timer
                Debug.Log("Player contact made. Contact count increased to: " + contactCount);

                UpdateIncrementObjects();
                StartCoroutine(PauseMovement());

                if (contactCount >= 4)
                {
                    Debug.Log("Contact count reached 4. Triggering end game.");
                    EndGame();
                }
            }
            else
            {
                Debug.Log("Contact detected but canIncrement is false. Contact not added.");
            }
        }
    }

    private IEnumerator PauseMovement()
    {
        canIncrement = false; // Disable further increments
        Debug.Log("Pausing movement for 2 seconds.");
        yield return new WaitForSeconds(2f); // Pause movement for 2 seconds
        canIncrement = true; // Re-enable increments
        Debug.Log("Resuming movement. canIncrement set to true.");
    }

    private void UpdateIncrementObjects()
    {
        // Enable or disable objects based on the current contact count
        for (int i = 0; i < incrementObjects.Length; i++)
        {
            if (i < contactCount)
            {
                incrementObjects[i].SetActive(true);
                Debug.Log("Enabling object " + i);
            }
            else
            {
                incrementObjects[i].SetActive(false);
                Debug.Log("Disabling object " + i);
            }
        }
    }

    private void EndGame()
    {
        Time.timeScale = 0f;

        if (endGameScreen != null)
        {
            endGameScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("End Game Screen is not assigned!");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Game Over!");
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting level...");

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
