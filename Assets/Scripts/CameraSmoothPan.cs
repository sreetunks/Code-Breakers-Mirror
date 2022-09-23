using UnityEngine;

public class CameraSmoothPan : MonoBehaviour
{
    [SerializeField] Vector3 targetOffset;
    [SerializeField] float moveDuration;
    [SerializeField] float panSpeed = 5.0f;
    [SerializeField] Transform boundsMin;
    [SerializeField] Transform boundsMax;

    Transform _targetTransform;
    Vector3 _velocity;
    Vector3 _currentOffset = Vector3.zero;

    private void Update()
    {
        if (Application.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space)) _currentOffset = targetOffset;
            if (Input.mousePosition.x < (Screen.width * 0.1f)) _currentOffset.x -= panSpeed * Time.deltaTime;
            else if (Input.mousePosition.x > Screen.width - (Screen.width * 0.1f)) _currentOffset.x += panSpeed * Time.deltaTime;

            if (Input.mousePosition.y < (Screen.height * 0.1f)) _currentOffset.z -= panSpeed * Time.deltaTime;
            else if (Input.mousePosition.y > Screen.height - (Screen.height * 0.1f)) _currentOffset.z += panSpeed * Time.deltaTime;
        }

        _currentOffset.x = Mathf.Clamp(_currentOffset.x, boundsMin.position.x + targetOffset.x, boundsMax.position.x + targetOffset.x);
        _currentOffset.z = Mathf.Clamp(_currentOffset.z, boundsMin.position.z + targetOffset.z, boundsMax.position.z + targetOffset.z);

        transform.position = Vector3.SmoothDamp(transform.position, _targetTransform.position + _currentOffset, ref _velocity, moveDuration);
    }

    public void UpdateTarget(Transform newTarget)
    {
        _targetTransform = newTarget;
        _velocity = Vector3.zero;
        _currentOffset = targetOffset;
    }

    public void ResetOffset()
    {
        _currentOffset = targetOffset;
    }
}
