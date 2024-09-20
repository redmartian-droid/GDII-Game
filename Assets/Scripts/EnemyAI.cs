using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3.0f;
    public float health = 100f;
    public GameObject endGameScreen;
    public float activationDistance = 10.0f; // Distance within which the enemy activates

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= activationDistance)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        Vector3 direction = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
        direction.Normalize(); // Normalise the direction to prevent faster movement when further away
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
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndGame();
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
    }

    public void RestartLevel()
    {
        Debug.Log("Button pressed!");

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
