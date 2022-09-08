using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void UpdateVisual(bool value)
    {
        _meshRenderer.enabled = value;
    }
}
