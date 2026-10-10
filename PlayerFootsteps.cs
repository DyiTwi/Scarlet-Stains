using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource footstepAudio;

    private void Update()
    {
        // Check if the player is pressing WASD or Arrow Keys
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f;

        if (isMoving)
        {
            // If they are moving and the audio isn't playing yet, start it
            if (!footstepAudio.isPlaying)
            {
                footstepAudio.Play();
            }
        }
        else
        {
            // If they stop moving, stop the audio instantly
            if (footstepAudio.isPlaying)
            {
                footstepAudio.Pause(); 
            }
        }
    }
}