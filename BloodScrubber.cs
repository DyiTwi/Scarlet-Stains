using UnityEngine;
using UnityEngine.UI;

public class BloodScrubber : MonoBehaviour
{
    [Header("Dependencies")]
    public HotbarManager hotbar;
    public string spongeItemID = "sponge";
    
    [Header("UI Settings")]
    public GameObject scrubbingCanvas; 
    public Image progressBar;          

    [Header("Audio")]
    public AudioSource scrubAudioSource; 
    public AudioClip finishSound;

    [Header("Scrubbing Settings")]
    public float scrubDistance = 4f;
    public float timeToClean = 3.0f; 

    private float currentScrubTime = 0f;
    private GameObject currentBloodTarget;

    private void Start()
    {
        if (scrubbingCanvas != null) scrubbingCanvas.SetActive(false);
        if (progressBar != null) progressBar.fillAmount = 0f;
    }

    private void Update()
    {
        
        if (hotbar == null || hotbar.GetEquippedItemID() != spongeItemID)
        {
            StopScrubbing();
            return;
        }

        // 2. Shoot a raycast to look for blood
        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, scrubDistance))
        {
            if (hit.collider.CompareTag("Blood"))
            {
                
                if (Input.GetMouseButton(0)) 
                {
                    
                    if (currentBloodTarget != hit.collider.gameObject)
                    {
                        currentBloodTarget = hit.collider.gameObject;
                        currentScrubTime = 0f;
                    }

                    PerformScrubbing();
                }
                else
                {
                    StopScrubbing(); // Looked at blood, but let go of the mouse
                }
            }
            else
            {
                StopScrubbing(); // Looked away from the blood
            }
        }
        else
        {
            StopScrubbing(); // Looking at nothing
        }
    }

    private void PerformScrubbing()
    {
        if (scrubbingCanvas != null) scrubbingCanvas.SetActive(true);
        
        if (scrubAudioSource != null && !scrubAudioSource.isPlaying)
        {
            scrubAudioSource.Play();
        }

        currentScrubTime += Time.deltaTime;

        if (progressBar != null)
        {
            progressBar.fillAmount = currentScrubTime / timeToClean;
        }

        
        if (currentScrubTime >= timeToClean)
        {
            if (finishSound != null)
            {
                AudioSource.PlayClipAtPoint(finishSound, currentBloodTarget.transform.position);
            }

            Destroy(currentBloodTarget);
            StopScrubbing();
        }
    }

    private void StopScrubbing()
    {
        currentScrubTime = 0f;
        currentBloodTarget = null;
        
        if (scrubbingCanvas != null) scrubbingCanvas.SetActive(false);
        if (progressBar != null) progressBar.fillAmount = 0f;
        
        if (scrubAudioSource != null && scrubAudioSource.isPlaying)
        {
            scrubAudioSource.Stop();
        }
    }
}