using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothPan : MonoBehaviour
{
    [SerializeField] Vector3 targetOffset;
    [SerializeField] float moveDuration;

    Vector3 _targetPosition;
    Vector3 _velocity;

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition + targetOffset, ref _velocity, moveDuration);
    }

    public void UpdateTarget(Transform newTarget)
    {
        _targetPosition = newTarget.position;
        _velocity = Vector3.zero;
    }
}
