using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    public HotbarManager inventory;
    public Light headlampSpotlight;

    [Header("Settings")]
    [Tooltip("The exact Item ID of the flashlight in your ItemData script")]
    public string flashlightItemID = "Flashlight"; 
    
    private bool isLightOn = false;

    private void Start()
    {
        if (headlampSpotlight != null) headlampSpotlight.enabled = false;
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (inventory != null && inventory.HasItemInInventory(flashlightItemID))
            {
                isLightOn = !isLightOn;
                if (headlampSpotlight != null) headlampSpotlight.enabled = isLightOn;
            }
        }

        
        if (isLightOn && inventory != null && !inventory.HasItemInInventory(flashlightItemID))
        {
            isLightOn = false;
            if (headlampSpotlight != null) headlampSpotlight.enabled = false;
        }
    }
}