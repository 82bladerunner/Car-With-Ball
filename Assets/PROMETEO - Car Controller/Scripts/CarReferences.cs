using System.Collections.Generic;
using UnityEngine;

public class CarReferences : MonoBehaviour
{
    [field:SerializeField] public Rigidbody rigidbody { get; private set; }
    [field:SerializeField] public Transform holderObject { get; private set; }
    [field:SerializeField] public List<WheelCollider> wheelColliders { get; private set; }
}
