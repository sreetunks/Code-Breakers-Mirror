using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private Animator unitAnimator;
    
    private Vector3 _targetPosition;
    private GridPosition _gridPosition;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking"); // Caching ID for Parameter

    private void Awake()
    {
        _targetPosition = transform.position; // Makes sure the unit does not walk to Vector3(0, 0, 0) upon load.
    }

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition((transform.position));
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);
    }

    private void Update()
    {
        var toTarget = _targetPosition - transform.position;
        var dist = toTarget.magnitude;
        
        if (dist > 0)
        {
            var move = moveSpeed * Time.deltaTime * toTarget.normalized; // Makes move speed the same for all frame-rate's
            
            if (move.magnitude > dist)
                move = toTarget;
            
            transform.position += move; // Sets target move location, EXACTLY (L: 15 - 26)
            unitAnimator.SetBool(IsWalking, true); // Starts "Walk" Animations
            
            var rotation = Quaternion.LookRotation(toTarget);
            var current = transform.localRotation;
            transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * rotateSpeed); // Sets Rotation to be more Accurate (L: 34 - 36) for 180 Degree's
        }
        else
            unitAnimator.SetBool(IsWalking, false); // Ends "Walk" Animations
        
        
        var newGridPosition = LevelGrid.Instance.GetGridPosition((transform.position));
        if (newGridPosition == _gridPosition) return;
        LevelGrid.Instance.UnitMovedGridPosition(this, _gridPosition, newGridPosition);
        _gridPosition = newGridPosition;
    }

    public void Move(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        HUDScript.HUD.ActionLogEvent("Unit Moved");
    }
}