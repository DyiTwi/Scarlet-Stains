using UnityEngine;
public class ScaleImage : MonoBehaviour
{
   public RectTransform imageRect;
   public float scaleFactor = 0.5f; // Example: Scale down by 50%
   void Start()
   {
       if (imageRect != null)
       {
           imageRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
       }
   }
}