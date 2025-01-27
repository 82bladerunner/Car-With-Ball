using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform carTransform;
	[Range(1, 20)]
	public float followSpeed = 8;
	[Range(1, 20)]
	public float lookSpeed = 12;
	
	[Header("Isometric Settings")]
	public float cameraDistance = 20f;
	public float cameraHeight = 15f;
	
	[Header("FOV Settings")]
	public float baseFOV = 60f;
	public float maxFOVIncrease = 15f;
	public float maxSpeed = 100f; // Speed at which FOV reaches maximum
	
	private Camera cam;
	private int currentAngleIndex = 0;
	private float[] viewAngles = new float[] { 45f, 135f, 225f, 315f }; // Four isometric views
	
	private Vector3 currentOffset;
	private bool cinematicView = false;
	private float cinematicTimer = 0f;
	private int currentViewIndex = 0;
	
	private enum CameraViews
	{
		FarRear,
		CloseRear,
		BirdView,
		Front,
		Cinematic
	}

	public Rigidbody carRigidbody; // Assign the car's rigidbody in inspector
	private float driftOffset = 3.0f; // How far the camera shifts during drifts

	void Start(){
		cam = GetComponent<Camera>();
		currentViewIndex = 0;
		
		// Automatically get the Rigidbody from the car
		carRigidbody = carTransform.GetComponent<Rigidbody>();
		if (carRigidbody == null) {
			Debug.LogError("No Rigidbody found on the car! Please add a Rigidbody component to your car.");
		}
		
		// Set initial rotation
		transform.rotation = Quaternion.Euler(45f, viewAngles[currentAngleIndex], 0f);
	}

	void Update()
	{
		// Change view angle when V is pressed
		if (Input.GetKeyDown(KeyCode.V))
		{
			currentAngleIndex = (currentAngleIndex + 1) % viewAngles.Length;
		}
	}

	void FixedUpdate()
	{
		UpdateIsometricCamera();
		UpdateFOV();
	}

	void UpdateIsometricCamera() {
		float currentAngle = viewAngles[currentAngleIndex];
		Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
		
		// Calculate base position
		Vector3 targetPosition = carTransform.position;
		targetPosition -= direction * cameraDistance;
		targetPosition += Vector3.up * cameraHeight;
		
		// Add subtle drift compensation
		float speed = carRigidbody.velocity.magnitude;
		Vector3 velocityDirection = speed > 1f ? carRigidbody.velocity.normalized : transform.forward;
		Vector3 driftComp = Vector3.Cross(Vector3.up, velocityDirection) * (driftOffset * Mathf.Min(speed / 30f, 1f));
		targetPosition += driftComp;
		
		// Smooth movement
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
		Quaternion targetRotation = Quaternion.Euler(45f, currentAngle, 0f);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
	}

	void UpdateFOV() {
		float speed = carRigidbody.velocity.magnitude;
		float speedRatio = Mathf.Clamp01(speed / maxSpeed);
		float targetFOV = baseFOV + (maxFOVIncrease * speedRatio);
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 5f);
	}
}
