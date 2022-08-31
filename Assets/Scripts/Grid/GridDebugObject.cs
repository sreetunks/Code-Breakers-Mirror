using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ReSharper disable once CheckNamespace
public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    private IGridObject _gridObject;
    
    public void SetGridObject(IGridObject gridObject)
    {
        _gridObject = gridObject;
    }

    private void Update()
    {
        textMeshPro.text = _gridObject.ToString();
    }
}
