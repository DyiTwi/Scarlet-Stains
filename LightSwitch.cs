using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip switchSound; 
    [Range(0f, 1f)] public float switchVolume = 1.0f;

    [Header("1. Real Lights (Point/Spot)")]
    public GameObject[] lightFixtures;

    [Header("2. Glowing Bulbs (Emissive Meshes)")]
    public Renderer[] glowingMeshes;

    [Header("Switch State")]
    public bool isOn = true;

    private void Start()
    {
        bool startingState = isOn; 
        isOn = !startingState; 
        ToggleLights(); 
    }

    public void ToggleLights()
    {
        isOn = !isOn;

        AudioSource src = GetComponent<AudioSource>();
        if (src == null) src = gameObject.AddComponent<AudioSource>(); // Auto-adds the component if you forgot it

        if (switchSound != null) src.PlayOneShot(switchSound, switchVolume);

        foreach (GameObject fixture in lightFixtures)
        {
            if (fixture != null)
            {
                Light[] lightsInFixture = fixture.GetComponentsInChildren<Light>();
                foreach (Light light in lightsInFixture) light.enabled = isOn;
            }
        }

        foreach (Renderer rend in glowingMeshes)
        {
            if (rend != null)
            {
                foreach (Material mat in rend.materials)
                {
                    if (isOn) mat.EnableKeyword("_EMISSION");
                    else mat.DisableKeyword("_EMISSION");
                }
            }
        }
    }
}