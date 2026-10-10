using UnityEngine;
using TMPro; 
using System.Collections;

public class ThoughtTrigger : MonoBehaviour
{
    [Header("UI Setup")]
    public TextMeshProUGUI subtitleUI; 

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;

    [Header("Monologue Settings")]
    [TextArea] public string thoughtText;
    public float displayTime = 3f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("Subtitle Triggered by walking into Collider!");
            StartCoroutine(ShowThought());
        }
    }

    public void TriggerFromRaycast()
    {
        Debug.Log("Subtitle Triggered by Script (Key Pickup)!");
        if (!hasTriggered)
        {
            StartCoroutine(ShowThought());
        }
        else 
        {
            Debug.LogWarning("The subtitle tried to play, but it was already triggered earlier!");
        }
    }

    private IEnumerator ShowThought()
    {
        hasTriggered = true;
        
        // 1. Freeze the player
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
        else
        {
            Debug.LogError("Player Movement Script is MISSING! Did you assign it in the Inspector?");
        }

        // 2. Show the text
        if (subtitleUI != null)
        {
            subtitleUI.text = thoughtText;
        }
        else
        {
            Debug.LogError("Subtitle UI is MISSING! The script crashed because it can't find the text box.");
        }
        
        // 3. Wait for the display time
        yield return new WaitForSeconds(displayTime);
        
        // 4. Clear the text
        if (subtitleUI != null && subtitleUI.text == thoughtText) 
        {
            subtitleUI.text = "";
        }

        // 5. Unfreeze the player
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
        
        Destroy(this); 
    }
}