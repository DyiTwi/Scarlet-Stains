using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("References")]
    public HotbarManager hotbarManager;
    
    [Header("Interaction Settings")]
    public float interactRange = 4f;
    
    [Tooltip("How thick the laser is. 0.15 is about the size of a grapefruit!")]
    public float selectionRadius = 0.15f; 
    
    [Tooltip("Tags that will trigger the white outline")]
    public List<string> allowedTags = new List<string> { "CleaningSupply", "KeyItem", "Interactable" };

    private GameObject currentTarget;

    private void Update()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        bool lookingAtValidTarget = false;

        if (Physics.SphereCast(ray.origin, selectionRadius, ray.direction, out RaycastHit hit, interactRange))
        {
            if (HasAllowedTag(hit.collider.tag))
            {
                lookingAtValidTarget = true;
                GameObject hitObj = hit.collider.gameObject;

                if (currentTarget != hitObj)
                {
                    ClearHighlight(); 
                    currentTarget = hitObj;
                    ApplyHighlight(currentTarget); 
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    TryPickup(currentTarget);
                }
            }
        }

        if (!lookingAtValidTarget && currentTarget != null)
        {
            ClearHighlight();
        }
    }

    private bool HasAllowedTag(string tagToCheck)
    {
        foreach (string tag in allowedTags)
        {
            if (tagToCheck == tag) return true;
        }
        return false;
    }

    private void ApplyHighlight(GameObject target)
    {
        Outline outline = target.GetComponentInChildren<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    private void ClearHighlight()
    {
        if (currentTarget != null)
        {
            Outline outline = currentTarget.GetComponentInChildren<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
        currentTarget = null;
    }

    private void TryPickup(GameObject targetObject)
    {

        LightSwitch lightSwitch = targetObject.GetComponentInParent<LightSwitch>();
        if (lightSwitch == null) lightSwitch = targetObject.GetComponent<LightSwitch>();

        if (lightSwitch != null)
        {
            lightSwitch.ToggleLights();
            return; // Stop the code right here so you don't try to put the switch in your pocket!
        }

        if (hotbarManager == null) return;

        ItemData itemInfo = targetObject.GetComponentInParent<ItemData>();
        if (itemInfo == null) itemInfo = targetObject.GetComponentInChildren<ItemData>();
        if (itemInfo == null) itemInfo = targetObject.GetComponent<ItemData>();
        
        if (itemInfo != null)
        {
            bool wasAdded = hotbarManager.AddItem(itemInfo.hotbarIcon, itemInfo.itemID, itemInfo.gameObject);
            
            if (wasAdded)
        {
            ClearHighlight(); 
            itemInfo.gameObject.SetActive(false); 
            hotbarManager.ShowMessage("Collected " + itemInfo.itemID.ToLower());

            // NEW: Play the pickup sound at the item's location before it vanishes!
            if (itemInfo.pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(itemInfo.pickupSound, targetObject.transform.position, itemInfo.pickupVolume);
            }
        }
        }
    }
}