using UnityEngine;

public class CarAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AudioSource tireScreechSound;

    [Header("Engine Sound Settings")]
    [SerializeField] private float minEnginePitch = 0.6f;
    [SerializeField] private float maxEnginePitch = 1.8f;
    [SerializeField] private float airborneMaxPitch = 2.2f;  // Higher pitch limit when airborne
    [SerializeField] private float airborneAcceleration = 2.5f;  // Faster pitch increase when airborne
    [SerializeField] private float minEngineVolume = 0.1f;
    [SerializeField] private float maxEngineVolume = 0.8f;
    [SerializeField] private float enginePitchChangeSpeed = 0.12f;  // Slow normal changes
    [SerializeField] private float suddenDecelerationSpeed = 3f;    // Fast deceleration response
    [SerializeField] private float stationaryRevSpeed = 4f;         // Fast revving when not moving
    [SerializeField] private float engineVolumeChangeSpeed = 1.5f;
    [SerializeField] private float idleEnginePitch = 0.7f;
    [SerializeField] private float idleEngineVolume = 0.2f;
    [SerializeField] private float velocityThreshold = 0.5f;  // Threshold for detecting stuck state
    [SerializeField] private float stationaryThreshold = 0.1f; // Speed threshold to consider car stationary

    [Header("Tire Screech Settings")]
    [SerializeField] private float minTireScreechVolume = 0.1f;
    [SerializeField] private float maxTireScreechVolume = 0.6f;
    [SerializeField] private float tireScreechPitchRange = 0.15f;
    [SerializeField] private float volumeChangeSpeed = 8f;

    private PrometeoCarController carController;
    private bool isInitialized = false;
    private float targetEnginePitch;
    private float targetEngineVolume;
    private float lastThrottleValue;
    private float currentThrottleChangeRate;
    private WheelCollider[] wheelColliders;
    private Vector3 lastPosition;
    private float actualVelocityMagnitude;
    private bool isGrounded;
    private float timeSinceLastGrounded;

    private void Start()
    {
        carController = GetComponent<PrometeoCarController>();
        
        // Get all wheel colliders
        wheelColliders = new WheelCollider[] {
            carController.frontLeftCollider,
            carController.frontRightCollider,
            carController.rearLeftCollider,
            carController.rearRightCollider
        };
        
        lastPosition = transform.position;
        
        if (engineSound != null)
        {
            engineSound.loop = true;
            engineSound.playOnAwake = false;
            engineSound.volume = 0f;
            engineSound.pitch = minEnginePitch;
            engineSound.Play();
        }

        if (tireScreechSound != null)
        {
            tireScreechSound.loop = true;
            tireScreechSound.playOnAwake = false;
            tireScreechSound.volume = 0f;
            tireScreechSound.Play();
        }

        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || !carController.useSounds) return;

        UpdateVehicleState();
        UpdateEngineSounds();
        UpdateTireScreech();
    }

    private void UpdateVehicleState()
    {
        // Calculate actual velocity using position change
        actualVelocityMagnitude = ((transform.position - lastPosition) / Time.deltaTime).magnitude;
        lastPosition = transform.position;

        // Check ground contact
        isGrounded = false;
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.isGrounded)
            {
                isGrounded = true;
                timeSinceLastGrounded = 0f;
                break;
            }
        }

        if (!isGrounded)
        {
            timeSinceLastGrounded += Time.deltaTime;
        }
    }

    private void UpdateEngineSounds()
    {
        if (engineSound == null || carController.maxSpeed <= 0) return;

        float throttleValue = Mathf.Clamp01(Mathf.Abs(carController.ThrottleInput));
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carController.carSpeed) / carController.maxSpeed);
        bool isStationary = actualVelocityMagnitude < stationaryThreshold;
        
        // Calculate throttle effectiveness based on actual movement
        float intendedVelocity = normalizedSpeed * carController.maxSpeed * (throttleValue > 0.1f ? 1f : 0f);
        float velocityDifference = Mathf.Abs(intendedVelocity - actualVelocityMagnitude);
        bool isStuck = velocityDifference > velocityThreshold && throttleValue > 0.1f;
        
        // Modify throttle value if stuck
        if (isStuck && isGrounded)
        {
            throttleValue *= 0.3f; // Reduce engine sound when stuck
        }
        
        float throttleChange = Mathf.Clamp(throttleValue - lastThrottleValue, -1f, 1f);
        currentThrottleChangeRate = Mathf.Clamp(
            Mathf.Lerp(currentThrottleChangeRate, throttleChange / Mathf.Max(Time.deltaTime, 0.0001f), Time.deltaTime * 3f),
            -10f, 10f
        );
        lastThrottleValue = throttleValue;

        // Calculate base engine pitch
        float speedPitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, Mathf.Pow(normalizedSpeed, 0.9f));
        float throttlePitch = Mathf.Lerp(idleEnginePitch, maxEnginePitch * 1.1f, Mathf.Pow(throttleValue, 1.2f));
        
        // Adjust pitch based on airborne state
        float currentMaxPitch = isGrounded ? maxEnginePitch : airborneMaxPitch;
        
        // Determine pitch change speed based on state
        float pitchChangeSpeed;
        if (!isGrounded)
        {
            // Very fast pitch changes when airborne
            pitchChangeSpeed = airborneAcceleration * 5f; // Much faster when airborne
        }
        else if (isStationary)
        {
            // Fast pitch changes when stationary (revving in place)
            pitchChangeSpeed = stationaryRevSpeed;
        }
        else
        {
            // Normal driving pitch changes
            pitchChangeSpeed = enginePitchChangeSpeed;
        }
        
        targetEnginePitch = Mathf.Clamp(Mathf.Max(speedPitch, throttlePitch), minEnginePitch, currentMaxPitch);

        // Add acceleration influence
        float accelerationInfluence = Mathf.Clamp(Mathf.Abs(currentThrottleChangeRate) * 0.08f, 0f, 0.2f);
        if (!isGrounded)
        {
            // Increase acceleration influence when airborne
            accelerationInfluence *= 2f;
        }
        targetEnginePitch = Mathf.Clamp(targetEnginePitch * (1f + accelerationInfluence), minEnginePitch, currentMaxPitch);

        // Calculate engine volume
        float baseVolume = Mathf.Lerp(minEngineVolume, maxEngineVolume, Mathf.Pow(normalizedSpeed, 1.2f));
        float throttleVolume = Mathf.Lerp(idleEngineVolume, maxEngineVolume, Mathf.Pow(throttleValue, 1.5f));
        targetEngineVolume = Mathf.Clamp01(Mathf.Max(baseVolume, throttleVolume));

        // Reduce volume when stuck
        if (isStuck && isGrounded)
        {
            targetEngineVolume *= 0.7f;
        }

        // Add subtle random variation (reduced when airborne for more consistent sound)
        float randomVariation = !isGrounded ? 1f : Mathf.Clamp(
            1f + (Mathf.PerlinNoise(Time.time * 1.5f, 0f) - 0.5f) * 0.02f,
            0.98f,
            1.02f
        );
        targetEnginePitch = Mathf.Clamp(targetEnginePitch * randomVariation, minEnginePitch, currentMaxPitch);

        // Apply final values with adaptive interpolation speed
        float currentPitch = engineSound.pitch;
        float pitchDifference = targetEnginePitch - currentPitch;
        
        // Special case for immediate pitch changes
        if (!isGrounded || (isStationary && throttleValue > 0.1f))
        {
            // Much faster pitch changes in special cases
            engineSound.pitch = Mathf.Lerp(currentPitch, targetEnginePitch, Time.deltaTime * pitchChangeSpeed);
        }
        else
        {
            // Normal driving behavior
            float adaptivePitchSpeed = (pitchDifference < 0) ? 
                Mathf.Lerp(pitchChangeSpeed, suddenDecelerationSpeed, Mathf.Abs(pitchDifference)) : 
                pitchChangeSpeed;
                
            float lerpTime = Mathf.Clamp01(Time.deltaTime * adaptivePitchSpeed);
            engineSound.pitch = Mathf.Lerp(currentPitch, targetEnginePitch, lerpTime);
        }
        
        engineSound.volume = Mathf.Lerp(engineSound.volume, targetEngineVolume, Time.deltaTime * engineVolumeChangeSpeed);
    }

    private void UpdateTireScreech()
    {
        if (tireScreechSound == null) return;

        float driftIntensity = 0f;
        
        if (carController.isDrifting || carController.isTractionLocked)
        {
            float speedFactor = Mathf.Clamp01(Mathf.Abs(carController.carSpeed) / 30f);
            driftIntensity = Mathf.Clamp01(speedFactor * (carController.isTractionLocked ? 1f : 0.7f));
        }

        float targetVolume = Mathf.Lerp(minTireScreechVolume, maxTireScreechVolume, driftIntensity);
        
        if (driftIntensity > 0f)
        {
            if (!tireScreechSound.isPlaying)
            {
                tireScreechSound.Play();
            }
            
            float pitchVariation = 1f + (Mathf.PerlinNoise(Time.time * 3f, 0f) - 0.5f) * tireScreechPitchRange;
            tireScreechSound.pitch = Mathf.Lerp(0.8f, 1.2f, driftIntensity) * pitchVariation;
            
            tireScreechSound.volume = Mathf.Lerp(tireScreechSound.volume, targetVolume, Time.deltaTime * volumeChangeSpeed);
        }
        else
        {
            tireScreechSound.volume = Mathf.Lerp(tireScreechSound.volume, 0f, Time.deltaTime * volumeChangeSpeed);
            if (tireScreechSound.volume < 0.01f)
            {
                tireScreechSound.Stop();
            }
        }
    }

    public void EnableSounds(bool enable)
    {
        if (!isInitialized) return;

        if (!enable)
        {
            if (engineSound != null)
            {
                engineSound.volume = 0f;
            }
            if (tireScreechSound != null)
            {
                tireScreechSound.volume = 0f;
                tireScreechSound.Stop();
            }
        }
    }

    private void OnDisable()
    {
        if (engineSound != null)
        {
            engineSound.Stop();
        }
        if (tireScreechSound != null)
        {
            tireScreechSound.Stop();
        }
    }
} 