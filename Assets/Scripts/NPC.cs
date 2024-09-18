using System.Collections;
using UnityEngine;
using TMPro; // For using TextMeshPro UI

public class NPC : MonoBehaviour
{
    public string[] dialogue; // Array to hold multiple lines of dialogue
    public float dialogueSpeed = 0.05f; // Speed at which the dialogue will appear
    public GameObject dialogueUI; // The UI panel that shows the dialogue
    public TMP_Text dialogueText; // The TextMeshPro text where dialogue is shown

    private int dialogueIndex; // To keep track of the current line in the dialogue
    private bool isPlayerInRange; // Checks if player is in range for interaction
    private bool isInteracting; // Checks if interaction is happening

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isInteracting)
            {
                StartDialogue();
            }
            else
            {
                DisplayNextLine();
            }
        }
    }

    // Function to start the dialogue
    void StartDialogue()
    {
        dialogueIndex = 0;
        isInteracting = true;
        dialogueUI.SetActive(true);
        StartCoroutine(TypeDialogue());
    }

    // Function to display the next line of dialogue
    void DisplayNextLine()
    {
        dialogueIndex++;
        if (dialogueIndex < dialogue.Length)
        {
            StartCoroutine(TypeDialogue());
        }
        else
        {
            EndDialogue();
        }
    }

    // Function to type the dialogue letter by letter
    IEnumerator TypeDialogue()
    {
        dialogueText.text = ""; // Clear the text
        foreach (char letter in dialogue[dialogueIndex].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueSpeed);
        }
    }

    // Function to end the dialogue interaction
    void EndDialogue()
    {
        isInteracting = false;
        dialogueUI.SetActive(false);
    }

    // Detect when the player enters the NPC's interaction zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // You can show a prompt like "Press E to interact" here
        }
    }

    // Detect when the player exits the NPC's interaction zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (isInteracting)
            {
                EndDialogue(); // End dialogue if the player walks away
            }
        }
    }
}
