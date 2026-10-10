using UnityEngine;
using UnityEngine.UI;
using System.Collections; // NEW: Required for Coroutines

public class open_door : MonoBehaviour
{
    public Transform sideA;
    public Transform sideB;

    [Header("Audio Sounds")]
    public AudioClip unlockSound;
    [Range(0f, 1f)] public float unlockVolume = 1.0f; 

    public AudioClip openDoorSound; 
    [Range(0f, 1f)] public float openDoorVolume = 1.0f;

    [Header("Interaction Settings")]
    public float holdDuration = 2.0f;

    [Header("Cooldown Settings")]
    public float cooldownDuration = 1.0f; // How long the door ignores input after teleporting
    private bool canInteract = true;      // The lock that prevents spamming

    [Header("Lock Settings")]
    public bool isLocked = false;
    public string requiredKeyID = "";
    public HotbarManager inventory;

    [Header("Highlight Settings")]
    public Renderer doorRenderer;
    public Color highlightColor = Color.yellow;

    [Header("UI Progress Bar Settings")]
    public GameObject uiCanvas; 
    public Image progressBarImage;

    private float currentHoldTime = 0f;
    private Color originalColor;
    private bool isHighlighted = false;
    private bool isPlayerInZone = false;
    private GameObject playerObject;
    private AudioSource audioSource; 

    private void Start()
    {
        if (doorRenderer == null) doorRenderer = GetComponent<Renderer>();
        if (doorRenderer != null) originalColor = doorRenderer.material.color;

        if (uiCanvas != null) uiCanvas.SetActive(false);
        if (progressBarImage != null) progressBarImage.fillAmount = 0f;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            playerObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            playerObject = null;
            currentHoldTime = 0f;
            SetHighlight(false);

            if (uiCanvas != null) uiCanvas.SetActive(false);
            if (progressBarImage != null) progressBarImage.fillAmount = 0f;
        }
    }

    private void Update()
    {

        if (!isPlayerInZone || playerObject == null || !canInteract) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        bool isLookingAtDoor = false; 

        RaycastHit[] hits = Physics.RaycastAll(ray, 5f);
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform == this.transform || hit.collider.transform.parent == this.transform.parent)
            {
                isLookingAtDoor = true;
                break;
            }
        }

        SetHighlight(isLookingAtDoor);

        if (isLookingAtDoor)
        {
            if (Input.GetKeyDown(KeyCode.E) && isLocked)
            {
                if (inventory != null && inventory.HasItemInInventory(requiredKeyID))
                {
                    inventory.RemoveItem(requiredKeyID); 
                    inventory.ShowMessage("Door unlocked");
                    
                    if (unlockSound != null) audioSource.PlayOneShot(unlockSound, unlockVolume);
                    
                    isLocked = false; 
                }
                else
                {
                    if (inventory != null) inventory.ShowMessage("Door is locked.");
                }
            }

            if (!isLocked && Input.GetKey(KeyCode.E))
            {
                if (currentHoldTime == 0f && openDoorSound != null)
                {
                    audioSource.PlayOneShot(openDoorSound, openDoorVolume);
                }

                if (uiCanvas != null) uiCanvas.SetActive(true);

                currentHoldTime += Time.deltaTime;
                
                if (progressBarImage != null)
                {
                    progressBarImage.fillAmount = currentHoldTime / holdDuration;
                }
                
                if (currentHoldTime >= holdDuration)
                {
                    currentHoldTime = 0f;
                    SetHighlight(false);
                    
                    if (uiCanvas != null) uiCanvas.SetActive(false);
                    if (progressBarImage != null) progressBarImage.fillAmount = 0f;
                    
                    Teleport(playerObject);

                    StartCoroutine(DoorCooldown());
                }
            }
            else if (!Input.GetKey(KeyCode.E)) 
            {
                currentHoldTime = 0f;
                if (uiCanvas != null) uiCanvas.SetActive(false);
                if (progressBarImage != null) progressBarImage.fillAmount = 0f;
                
                if (audioSource.isPlaying) audioSource.Stop();
            }
        }
        else 
        {
            currentHoldTime = 0f;
            if (uiCanvas != null) uiCanvas.SetActive(false);
            if (progressBarImage != null) progressBarImage.fillAmount = 0f;
        }
    }

    private void SetHighlight(bool state)
    {
        if (doorRenderer == null || isHighlighted == state) return;
        doorRenderer.material.color = state ? highlightColor : originalColor;
        isHighlighted = state;
    }

    private void Teleport(GameObject player)
    {
        float distA = Vector3.Distance(player.transform.position, sideA.position);
        float distB = Vector3.Distance(player.transform.position, sideB.position);
        Transform target = (distA < distB) ? sideB : sideA;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = target.position;
        player.transform.rotation = target.rotation; 

        if (cc != null) cc.enabled = true;
    }

    private IEnumerator DoorCooldown()
    {
        canInteract = false;                                // Lock the door
        yield return new WaitForSeconds(cooldownDuration);  // Wait for 1 second
        canInteract = true;                                 // Unlock the door
    }
}