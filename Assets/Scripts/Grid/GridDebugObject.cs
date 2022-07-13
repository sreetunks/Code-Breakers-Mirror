using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ReSharper disable once CheckNamespace
public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    private GridObject _gridObject;
    
    public void SetGridObject(GridObject gridObject)
    {
        _gridObject = gridObject;
    }

    private void Update()
    {
        textMeshPro.text = _gridObject.ToString();
    }
}
