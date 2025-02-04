using System;
using System.Collections;
using UnityEngine;

public class EscapeButtonListener : MonoBehaviour
{
    public static event Action OnEscapeButtonPressed;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(AAq());
        }
    }

    private IEnumerator AAq()
    {
        yield return new WaitForSeconds(0.1f);
        OnEscapeButtonPressed?.Invoke();
    }
}
