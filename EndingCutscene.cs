using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using UnityEngine.SceneManagement; // Critical for loading the menu

public class EndingCutscene : MonoBehaviour
{
    [Header("Ending UI")]
    public Image blackoutScreen;
    public TextMeshProUGUI endingText; 
    [TextArea] public string finalMessage;

    [Header("Audio")]
    public AudioSource gunshotSound;

    [Header("Game State")]
    public MonoBehaviour playerMovementScript;
    public GameObject[] gameplayCanvases; // Drag your crosshair/hotbar here
    public string mainMenuSceneName = "MainMenu"; // Exact name of your menu scene

    private bool canExitToMenu = false;

    private void Update()
    {
        // Once the cutscene is done, wait for the player to press E
        if (canExitToMenu && Input.GetKeyDown(KeyCode.E))
        {
            Cursor.lockState = CursorLockMode.None; // Give the mouse back for the menu
            Cursor.visible = true;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    public void StartEndingSequence()
    {
        StartCoroutine(PlayEnding());
    }

    private IEnumerator PlayEnding()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        // Hide crosshair and hotbar so they don't show on the black screen
        foreach (GameObject canvas in gameplayCanvases)
        {
            if (canvas != null) canvas.SetActive(false);
        }

        yield return new WaitForSeconds(5f);

        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (blackoutScreen != null) blackoutScreen.color = new Color(0, 0, 0, 1);
        if (gunshotSound != null) gunshotSound.Play();
        if (endingText != null) endingText.text = finalMessage;

        // Allow the player to press E now that the text is on screen
        canExitToMenu = true; 
    }
}