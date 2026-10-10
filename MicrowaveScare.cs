using UnityEngine;

public class MicrowaveScare : MonoBehaviour
{
    [Header("Scare Elements")]
    public AudioSource buzzingAudio;
    public Light interiorLight; 
    public Light mainKitchenLight;

    [Header("Audio")]
    public AudioClip switchClickSound;
    [Range(0f, 1f)] public float switchVolume = 1.0f;
    
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    
    private bool hasTriggered = false;
    private bool isBuzzing = false;

    private void Start()
    {
        if (interiorLight != null) interiorLight.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; 
            isBuzzing = true;
            
            if (buzzingAudio != null) buzzingAudio.Play();
            if (interiorLight != null) interiorLight.enabled = true;
            
            // Turn off the ceiling light and play the click
            if (mainKitchenLight != null)
            {
                mainKitchenLight.enabled = false;

                if (switchClickSound != null)
                {
                    AudioSource.PlayClipAtPoint(switchClickSound, mainKitchenLight.transform.position, switchVolume);
                }
            }
            else if (switchClickSound != null)
            {
                // Fallback to microwave's position if no light is assigned
                AudioSource.PlayClipAtPoint(switchClickSound, transform.position, switchVolume);
            }
        }
    }

    private void Update()
    {
        if (isBuzzing && Input.GetKeyDown(KeyCode.E))
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    TurnOffMicrowave();
                }
            }
        }
    }

    private void TurnOffMicrowave()
    {
        isBuzzing = false;
        
        if (buzzingAudio != null) buzzingAudio.Stop();
        if (interiorLight != null) interiorLight.enabled = false;
    }
}