using UnityEngine;

public class BallDeath : MonoBehaviour
{
    [SerializeField] private float deathY = -10f; // Y position below which ball is considered dead

    void Update()
    {
        if (transform.position.y < deathY)
        {
            GameManager.Instance.GameOver();
        }
    }
} 