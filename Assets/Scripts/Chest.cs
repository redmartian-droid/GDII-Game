using UnityEngine;
using UnityEngine.SceneManagement;

public class Chest : MonoBehaviour
{
    public GameObject endGameScreen;
    private PlayerMovement playerMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        // Stop time
        Time.timeScale = 0f;

        // Show end game screen
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
        // Reset time scale
        Time.timeScale = 1f;

        PickUpCoin.ResetCoinCount();

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}