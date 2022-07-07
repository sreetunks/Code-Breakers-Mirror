using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseWorld : MonoBehaviour
{

    private static MouseWorld _instance;
    
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        _instance = this;
    }

    public static Vector3 GetPosition()
    {
        // ReSharper disable once PossibleNullReferenceException
        // ReSharper disable once Unity.PerformanceCriticalCodeCameraMain
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var raycastHit, float.MaxValue, _instance.mousePlaneLayerMask);
        return raycastHit.point;
    }
}
