using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;
using Units;

public class Breakable : MonoBehaviour, IGridObject, IDamageable
{
    public delegate void OnObjectDeathEventHandler(Breakable unit);
    public OnObjectDeathEventHandler OnObjectDeath;

    public delegate void OnObjectDamagedEventHandler(int damageDealt);
    public OnObjectDamagedEventHandler OnObjectDamaged;

    [SerializeField] private int health;

    public GridCellState GridCellPreviousState { get; set; }
    public GridPosition Position { get; private set; }

    public int Health => health;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<UnitSelectedVisual>().UpdateVisual(false);
        Position = GridSystem.GetGridPosition(transform.position);
        GridCellPreviousState = GridCellState.Impassable;
        GridSystem.UpdateGridObjectPosition(this, Position);
        transform.position = GridSystem.GetWorldPosition(Position);
    }

    public void TakeDamage(int damageDealt)
    {
        health = Mathf.Max(0, health - damageDealt);
        OnObjectDamaged?.Invoke(damageDealt);
        if (health == 0)
        {
            OnObjectDeath?.Invoke(this); // Invoke is considered Expensive

            gameObject.SetActive(false);
            GridSystem.SetGridCellState(Position, GridCellPreviousState);
        }
    }
}
