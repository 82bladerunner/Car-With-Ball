using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform carTransform;
	[Range(1, 10)]
	public float followSpeed = 2;
	[Range(1, 10)]
	public float lookSpeed = 5;
	
	[Header("Camera Positions")]
	private Vector3[] cameraOffsets = new Vector3[]
	{
		new Vector3(7, 7, -7),    // Isometric Right
		new Vector3(-7, 7, -7),   // Isometric Left
		new Vector3(0, 7, -7),    // Isometric Center
		new Vector3(0, 15, 0)     // Bird's Eye View
	};
	
	private float[] cameraAngles = new float[]
	{
		35f,    // Isometric Right angle
		35f,    // Isometric Left angle
		35f,    // Isometric Center angle
		90f     // Bird's Eye View angle
	};

	private int currentViewIndex = 0;

	void Start()
	{
		UpdateCameraPosition();
	}

	void Update()
	{
		// Switch camera view with V key
		if (Input.GetKeyDown(KeyCode.V))
		{
			currentViewIndex = (currentViewIndex + 1) % cameraOffsets.Length;
			UpdateCameraPosition();
		}
	}

	void UpdateCameraPosition()
	{
		// Set initial rotation for new view
		transform.rotation = Quaternion.Euler(cameraAngles[currentViewIndex], 
			currentViewIndex == 3 ? 0 : transform.rotation.eulerAngles.y, 0);
	}

	void FixedUpdate()
	{
		if (carTransform == null) return;

		// Calculate desired position based on current view
		Vector3 targetPos;
		if (currentViewIndex == 3) // Bird's Eye View
		{
			// Direct overhead position
			targetPos = carTransform.position + cameraOffsets[currentViewIndex];
		}
		else // Isometric views
		{
			// Rotate offset based on car's rotation for isometric views
			Vector3 rotatedOffset = Quaternion.Euler(0, carTransform.eulerAngles.y, 0) * cameraOffsets[currentViewIndex];
			targetPos = carTransform.position + rotatedOffset;
		}

		// Smoothly move camera
		transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

		// Handle rotation
		if (currentViewIndex != 3) // Not Bird's Eye View
		{
			Vector3 lookDirection = carTransform.position - transform.position;
			Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
			targetRotation = Quaternion.Euler(cameraAngles[currentViewIndex], targetRotation.eulerAngles.y, 0);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
		}
	}

}
