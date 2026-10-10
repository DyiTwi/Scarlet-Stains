using UnityEngine;

public class TrashBin : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip disposeSound;
    [Range(0f, 1f)] public float volume = 1.0f;

    [Header("Cleaning Progress")]
    public int trashCleaned = 0;
    public int totalTrashInLevel = 5;

    public void DisposeTrash()
    {
        trashCleaned++;
        if (disposeSound != null) 
        {
            AudioSource.PlayClipAtPoint(disposeSound, transform.position, volume);
        }
        
        Debug.Log("Trash Cleaned: " + trashCleaned + " / " + totalTrashInLevel);
        
        if (trashCleaned >= totalTrashInLevel)
        {
            Debug.Log("Living room is clean!");
        }
    }
}