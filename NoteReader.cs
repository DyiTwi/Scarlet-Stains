using UnityEngine;
using UnityEngine.UI;

public class NoteReader : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject noteUIContainer; 
    public Image noteImageComponent;   
    public Sprite thisNoteImage;       

    [Header("Subtitles")]
    [Tooltip("Drag the ThoughtTrigger GameObject here (or put ThoughtTrigger on this note)")]
    public ThoughtTrigger thoughtTrigger;

    [Header("Ending Sequence")]
    [Tooltip("Drag your GameEndingManager here. Only needed for BedroomPaper!")]
    public EndingCutscene finalCutscene;

    [Header("Audio")]
    public AudioClip paperRustleSound;
    [Range(0f, 1f)] public float soundVolume = 1.0f;

    [Header("Settings")]
    public float interactionDistance = 3f;

    private bool isReading = false;
    private Outline outlineComponent;

    private void Start()
    {
        outlineComponent = GetComponent<Outline>();
        if (outlineComponent != null)
        {
            outlineComponent.enabled = false;
        }

        if (thoughtTrigger == null)
        {
            thoughtTrigger = GetComponent<ThoughtTrigger>();
        }
    }

    private void Update()
    {

        if (isReading)
        {
            if (Input.GetKeyDown(KeyCode.E)) CloseNote();
            return; 
        }

        bool isLookingAtMe = false;
        Camera cam = Camera.main;

        if (cam != null)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                if (hit.collider.gameObject == this.gameObject && hit.collider.CompareTag("Note"))
                {
                    isLookingAtMe = true;
                }
            }
        }

        // 3. Turn outline on or off based on if looking at it
        if (outlineComponent != null)
        {
            outlineComponent.enabled = isLookingAtMe;
        }

        // 4. If looking at it and press E, open the note
        if (isLookingAtMe && Input.GetKeyDown(KeyCode.E))
        {
            OpenNote();
        }
    }

    private void OpenNote()
    {
        isReading = true;
        noteImageComponent.sprite = thisNoteImage;
        noteUIContainer.SetActive(true);
        
        if (outlineComponent != null) outlineComponent.enabled = false;

        if (paperRustleSound != null) 
        {
            AudioSource.PlayClipAtPoint(paperRustleSound, transform.position, soundVolume);
        }
    }

    private void CloseNote()
    {
        isReading = false;
        noteUIContainer.SetActive(false);

        // 1. ALWAYS trigger the subtitle, no matter what note this is
        if (thoughtTrigger != null)
        {
            thoughtTrigger.TriggerFromRaycast();
        }

        // 2. IF it is the final note, ALSO start the 5-second background timer
        if (gameObject.name == "BedroomPaper")
        {
            if (finalCutscene != null)
            {
                finalCutscene.StartEndingSequence();
            }
        }
    }
}