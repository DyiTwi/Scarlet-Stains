using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour
{
    [Header("Light Settings")]
    public Light bathroomLight;
    public float minFlickerSpeed = 0.1f;
    public float maxFlickerSpeed = 0.6f;
    
    private float originalIntensity;
    private bool isShattered = false;

    private void Start()
    {
        if (bathroomLight != null)
        {
            // Memorize how bright the light is supposed to be
            originalIntensity = bathroomLight.intensity;
            StartCoroutine(FlickerRoutine());
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (!isShattered)
        {
            // Wait for a random fraction of a second
            yield return new WaitForSeconds(Random.Range(minFlickerSpeed, maxFlickerSpeed));
            
            // Only flicker if the LightSwitch has the power turned ON!
            if (bathroomLight != null && bathroomLight.enabled)
            {
                // Toggle the brightness between 0 and its normal level
                if (bathroomLight.intensity > 0f)
                {
                    bathroomLight.intensity = 0f;
                }
                else
                {
                    bathroomLight.intensity = originalIntensity;
                }
            }
        }
    }

    public void Shatter()
    {
        isShattered = true;
        if (bathroomLight != null) 
        {
            bathroomLight.intensity = 0f;
            bathroomLight.enabled = false;
        }
    }
}