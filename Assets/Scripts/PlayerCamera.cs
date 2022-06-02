using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x = 0;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.unscaledDeltaTime);
        transform.position = smoothedPosition;
    }
}
