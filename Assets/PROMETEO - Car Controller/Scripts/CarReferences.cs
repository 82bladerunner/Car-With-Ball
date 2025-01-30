using System.Collections.Generic;
using UnityEngine;

public class CarReferences : MonoBehaviour
{
    [field:SerializeField] public Rigidbody carRigidbody { get; private set; }
    [field:SerializeField] public Transform holderObject { get; private set; }
    [field:SerializeField] public List<WheelCollider> wheelColliders { get; private set; }
    
    // Changed to regular SerializeField for better inspector visibility
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private MeshRenderer[] wheelMeshRenderers;
    
    public MeshRenderer[] AllMeshRenderers { get; private set; }
    public Vector3 respawnPosition { get; private set; }

    private void Start()
    {
        // Combine body and wheel renderers into one array
        AllMeshRenderers = new MeshRenderer[1 + wheelMeshRenderers.Length];
        AllMeshRenderers[0] = bodyMeshRenderer;
        for (int i = 0; i < wheelMeshRenderers.Length; i++)
        {
            AllMeshRenderers[i + 1] = wheelMeshRenderers[i];
        }
        
        // Store initial position as respawn position
        respawnPosition = transform.position;
    }
}
