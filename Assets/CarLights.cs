using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLights : MonoBehaviour
{
    [Header("Light References")]
    public Light[] headLights;  // Array of front lights
    public Light[] brakeLights; // Array of rear lights
    
    [Header("Light Settings")]
    public float normalBrakeIntensity = 2f;
    public float brakingIntensity = 6f;    // Brighter when braking
    public float headlightIntensity = 4f;
    public KeyCode headlightToggle = KeyCode.L;

    private PrometeoCarController carController;
    private bool headlightsOn = true;

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<PrometeoCarController>();
        
        // Initialize lights with proper settings
        foreach (Light light in headLights)
        {
            light.intensity = headlightIntensity;  // Start with headlights on
            light.range = 20f;    // Adjust light range
            light.spotAngle = 80f; // Adjust cone angle if using spot lights
            light.cullingMask = ~(1 << LayerMask.NameToLayer("Car")); // Don't light up car layer
        }
        
        foreach (Light light in brakeLights)
        {
            light.intensity = normalBrakeIntensity;
            light.range = 5f;     // Shorter range for brake lights
            light.spotAngle = 120f; // Wider angle for brake lights
            light.cullingMask = ~(1 << LayerMask.NameToLayer("Car")); // Don't light up car layer
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle headlights with L key
        if (Input.GetKeyDown(headlightToggle))
        {
            headlightsOn = !headlightsOn;
            foreach (Light light in headLights)
            {
                light.intensity = headlightsOn ? headlightIntensity : 0f;
            }
        }

        // Update brake lights based on car braking
        bool isBraking = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.S);
        foreach (Light light in brakeLights)
        {
            light.intensity = Mathf.Lerp(light.intensity, 
                isBraking ? brakingIntensity : normalBrakeIntensity, 
                Time.deltaTime * 10f);
        }
    }
}
