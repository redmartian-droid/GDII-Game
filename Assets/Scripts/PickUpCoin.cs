using UnityEngine;
using TMPro;

public class PickUpCoin : MonoBehaviour
{
    public TextMeshProUGUI coinCountText;

    private static int coinCount = 0;
    private static TextMeshProUGUI staticCoinCountText;

    void Start()
    {
        // Assign the static reference to the instance reference
        staticCoinCountText = coinCountText;
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
        Destroy(gameObject);
    }

    void UpdateCoinCountText()
    {
        if (staticCoinCountText != null)
        {
            staticCoinCountText.text = "Coins: " + coinCount;
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
