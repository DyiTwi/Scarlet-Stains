using UnityEngine;

public class KeyPickupScare : MonoBehaviour
{
    [Header("Inventory Check")]
    public HotbarManager hotbar;
    public string targetKeyID = "Bedroom Key"; 

    [Header("Scare Elements")]
    public AudioSource doorBangingAudio;
    public AudioSource glassShatterAudio;     
    public FlickeringLight lightToShatter;    

    [Header("Subtitle Integration")]
    public ThoughtTrigger panicSubtitle; // NEW: Slot for your subtitle trigger

    private bool scareTriggered = false;

    private void Update()
    {
        if (!scareTriggered && hotbar != null)
        {
            if (hotbar.HasItemInInventory(targetKeyID))
            {
                TriggerNightmare();
            }
        }
    }

    private void TriggerNightmare()
    {
        scareTriggered = true; 

        // 1. Shatter the light
        if (lightToShatter != null) lightToShatter.Shatter();
        if (glassShatterAudio != null) glassShatterAudio.Play();

        // 2. Start the aggressive door banging
        if (doorBangingAudio != null) doorBangingAudio.Play();

        // 3. Trigger the panic subtitle (and freeze the player!)
        if (panicSubtitle != null)
        {
            panicSubtitle.TriggerFromRaycast();
        }
    }
}