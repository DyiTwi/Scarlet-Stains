using UnityEngine;

public class ItemData : MonoBehaviour
{
    [Header("Inventory Information")]
    [Tooltip("The exact string ID the Hotbar will use (e.g., 'Sponge', 'Bleach', 'RedKey')")]
    public string itemID = "ItemName";

    public AudioClip pickupSound;
    [Range(0f, 1f)] public float pickupVolume = 1.0f; // Creates a slider from 0 to 1

    // NEW: Drop sound variables
    public AudioClip dropSound;
    [Range(0f, 1f)] public float dropVolume = 1.0f;
    
    [Tooltip("The 2D picture that will show up in the Hotbar slot")]
    public Sprite hotbarIcon;
    
    [Tooltip("The physical prefab from your Project window to spawn when dropped")]
    public GameObject dropPrefab;
}