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

    public static bool GetPosition(out Vector3 position)
    {
        // ReSharper disable once PossibleNullReferenceException
        // ReSharper disable once Unity.PerformanceCriticalCodeCameraMain
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isValidHit = Physics.Raycast(ray, out var raycastHit, float.MaxValue, _instance.mousePlaneLayerMask);
        position =  raycastHit.point;
        return isValidHit;
    }
}
