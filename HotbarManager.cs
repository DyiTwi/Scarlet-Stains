using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarManager : MonoBehaviour
{
    [System.Serializable]
    public class HotbarSlot
    {
        public Image iconImage;
        public Image highlightImage;
        
        [HideInInspector] public bool hasItem = false;
        [HideInInspector] public string itemID = ""; 
        [HideInInspector] public GameObject physicalItem; 
    }

    [Header("UI Slots")]
    public HotbarSlot[] slots = new HotbarSlot[9];

    [Header("Player References")]
    public Transform dropSpawnPoint; 

    [Header("UI Messages")]
    [Tooltip("Drag the new MessageText UI element here!")]
    public TextMeshProUGUI messageText; 
    private float messageTimer = 0f;

    private int selectedSlot = 0;
    
    private Outline highlightedBin;

    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].iconImage != null) slots[i].iconImage.gameObject.SetActive(false);
            if (slots[i].highlightImage != null) slots[i].highlightImage.gameObject.SetActive(false);
        }
        SelectSlot(0); 
    }

    private void Update()
    {
        // Handle UI Message Timer
        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0 && messageText != null) 
            {
                messageText.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);


        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SelectSlot(selectedSlot - 1);
        else if (scroll < 0f) SelectSlot(selectedSlot + 1);


        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("TrashBin"))
            {
                Outline binOutline = hit.collider.GetComponent<Outline>();
                if (binOutline != null && highlightedBin != binOutline)
                {
                    if (highlightedBin != null) highlightedBin.enabled = false;
                    highlightedBin = binOutline;
                    highlightedBin.enabled = true; // Turn outline on
                }
            }
            else if (highlightedBin != null)
            {
                highlightedBin.enabled = false; // Turn outline off if looking away
                highlightedBin = null;
            }
        }
        else if (highlightedBin != null)
        {
            highlightedBin.enabled = false;
            highlightedBin = null;
        }


        if (Input.GetKeyDown(KeyCode.G))
        {
            bool threwAway = false;


            if (highlightedBin != null && slots[selectedSlot].hasItem)
            {

                if (slots[selectedSlot].physicalItem.CompareTag("Trash"))
                {

                    TrashBin binScript = highlightedBin.GetComponent<TrashBin>();
                    if (binScript != null) binScript.DisposeTrash();

                    Destroy(slots[selectedSlot].physicalItem); 

  
                    slots[selectedSlot].hasItem = false;
                    slots[selectedSlot].itemID = "";
                    slots[selectedSlot].physicalItem = null;
                    slots[selectedSlot].iconImage.sprite = null;
                    slots[selectedSlot].iconImage.gameObject.SetActive(false);

                    threwAway = true; 
                }
            }


            if (!threwAway)
            {
                DropItem(selectedSlot); 
            }
        }
    }

    private void SelectSlot(int index)
    {
        if (index < 0) index = slots.Length - 1;
        if (index >= slots.Length) index = 0;

        selectedSlot = index;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].highlightImage != null)
                slots[i].highlightImage.gameObject.SetActive(i == selectedSlot);
        }
    }

    public string GetEquippedItemID()
    {
        if (slots[selectedSlot].hasItem) return slots[selectedSlot].itemID;
        return "None"; 
    }

    public bool AddItem(Sprite icon, string uniqueItemID, GameObject actualItem)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].hasItem)
            {
                slots[i].hasItem = true;
                slots[i].itemID = uniqueItemID;
                slots[i].physicalItem = actualItem;

                slots[i].iconImage.sprite = icon;
                slots[i].iconImage.gameObject.SetActive(true);

                if (i == selectedSlot) SelectSlot(selectedSlot); 
                return true; 
            }
        }
        return false;
    }

    public void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(true);
            messageTimer = 3f; 
        }
    }

    public bool HasItemInInventory(string searchItemID)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].hasItem && slots[i].itemID == searchItemID) return true;
        }
        return false; 
    }

    public void RemoveItem(string searchItemID)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].hasItem && slots[i].itemID == searchItemID)
            {
                if (slots[i].physicalItem != null) Destroy(slots[i].physicalItem);

                slots[i].hasItem = false;
                slots[i].itemID = "";
                slots[i].physicalItem = null;
                slots[i].iconImage.sprite = null;
                slots[i].iconImage.gameObject.SetActive(false);
                return; 
            }
        }
    }

    private void DropItem(int index)
    {
        if (!slots[index].hasItem) return; 

        if (slots[index].physicalItem != null && dropSpawnPoint != null)
        {
            Vector3 finalDropPosition = dropSpawnPoint.position;

            if (Physics.Raycast(dropSpawnPoint.position, Vector3.down, out RaycastHit hit, 10f))
            {
                finalDropPosition = hit.point;
                finalDropPosition.y += 0.1f; 
            }

            slots[index].physicalItem.transform.position = finalDropPosition;
            slots[index].physicalItem.SetActive(true);

            Rigidbody rb = slots[index].physicalItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero;
            }

            ItemData itemInfo = slots[index].physicalItem.GetComponent<ItemData>();
            if (itemInfo != null && itemInfo.dropSound != null)
            {
                AudioSource.PlayClipAtPoint(itemInfo.dropSound, finalDropPosition, itemInfo.dropVolume);
            }
        }

        slots[index].hasItem = false;
        slots[index].itemID = "";
        slots[index].physicalItem = null;
        slots[index].iconImage.sprite = null;
        slots[index].iconImage.gameObject.SetActive(false);
    }
}