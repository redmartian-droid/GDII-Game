using UnityEngine;
using TMPro;

public class PickUpCoin : MonoBehaviour
{
    public TextMeshProUGUI coinCountText;
    public AudioClip coinPickupSound; // Reference to the coin pickup sound
    private static AudioSource audioSource; // Shared audio source for playing sounds

    private static int coinCount = 0;
    private static TextMeshProUGUI staticCoinCountText;

    void Start()
    {
        // Assign the static reference to the instance reference
        staticCoinCountText = coinCountText;

        // Ensure there is a single AudioSource for playing sound effects
        if (audioSource == null)
        {
            GameObject audioObject = new GameObject("CoinPickupAudioSource");
            audioSource = audioObject.AddComponent<AudioSource>();
        }

        UpdateCoinCountText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CollectCoin();
            Debug.Log("Coin collected!");
        }
    }

    void CollectCoin()
    {
        coinCount++;
        UpdateCoinCountText();
        PlayCoinPickupSound(); // Play the sound when a coin is picked up
        Destroy(gameObject);
    }

    void UpdateCoinCountText()
    {
        if (staticCoinCountText != null)
        {
            staticCoinCountText.text = "-  " + coinCount;
        }
    }

    void PlayCoinPickupSound()
    {
        if (coinPickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(coinPickupSound); // Play the sound effect
        }
    }

    // Method to reset the coin count to 0 and update the text
    public static void ResetCoinCount()
    {
        coinCount = 0;
        if (staticCoinCountText != null)
        {
            staticCoinCountText.text = "Coins: " + coinCount;
        }
    }
}
