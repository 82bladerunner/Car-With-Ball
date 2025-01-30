using UnityEngine;

[System.Serializable]
public class CarSnapshot
{
    private Vector3 _position;
    private Quaternion _rotation;
    private float _timestamp;

    public Vector3 position { get { return _position; } }
    public Quaternion rotation { get { return _rotation; } }
    public float timestamp { get { return _timestamp; } }

    public CarSnapshot(Vector3 pos, Quaternion rot, float time)
    {
        _position = pos;
        _rotation = rot;
        _timestamp = time;
    }
} 