using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectInteraction : MonoBehaviour
{
    public TMP_Text interactionText; // UI Text to display the interaction message
    public string displayMessage = "You interacted with the object!"; // Message to display
    public float interactionDistance = 3f; // Maximum distance to interact

    private bool isPlayerNearby = false; // Check if player is within range

    void Start()
    {
        // Ensure the interaction text is hidden at the start
        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    void Update()
    {
        // Check if the player presses E and is nearby
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithObject();
        }
    }

    private void InteractWithObject()
    {
        if (interactionText != null)
        {
            // Display the interaction message
            interactionText.text = displayMessage;

            // Optional: Hide the message after a few seconds
            Invoke("HideInteractionText", 3f);
        }
    }

    private void HideInteractionText()
    {
        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            if (interactionText != null)
            {
                // Show a prompt to interact
                interactionText.text = "Press E to interact.";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player leaves the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            if (interactionText != null)
            {
                // Hide the interaction prompt
                interactionText.text = "";
            }
        }
    }
}
