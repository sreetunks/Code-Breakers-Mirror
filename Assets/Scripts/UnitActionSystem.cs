using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedUnitChanged; 
    
    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Update()
    {
        // ReSharper disable once InvertIf
        if (Input.GetMouseButtonDown(0))
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            if (TryHandleUnitSelection()) return;
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            selectedUnit.Move(MouseWorld.GetPosition()); // Move's Unit based on Left Mouse Button Down
        }
    }

    private bool TryHandleUnitSelection()
    {
        // ReSharper disable once PossibleNullReferenceException
        // ReSharper disable once Unity.PerformanceCriticalCodeCameraMain
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var raycastHit, float.MaxValue, unitLayerMask)) return false;
        if (!raycastHit.transform.TryGetComponent<Unit>(out selectedUnit)) return false;
        SetSelectedUnit(selectedUnit);
        return true;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}
