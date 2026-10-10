using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject controlsPanel;

    private bool isPaused = false;

    void Update()
    {
        // Toggle the pause menu when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        // Turn the UI panel on or off
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(isPaused);
        }

        if (isPaused)
        {
            Time.timeScale = 0f; // Freeze all game movement/timers

        }
        else
        {
            Time.timeScale = 1f; // Resume time
            Cursor.lockState = CursorLockMode.Locked; // Lock the mouse back to the center
            Cursor.visible = false; // Hide the mouse for gameplay
        }
    }
}