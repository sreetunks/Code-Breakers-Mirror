using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;

namespace Units
{
    public class Breakable : MonoBehaviour, IGridObject, IDamageable
    {
        private OnObjectDeathEventHandler _onObjectDeath;
        private OnObjectDamagedEventHandler _onObjectDamaged;
        
        private delegate void OnObjectDeathEventHandler(Breakable unit);
        private delegate void OnObjectDamagedEventHandler(int damageDealt);

        [SerializeField] private int health;

        public GridCellState GridCellPreviousState { get; set; }
        public GridPosition Position { get; private set; }

        public int Health => health;
        
        // Start is called before the first frame update
        private void Start()
        {
            GetComponentInChildren<UnitSelectedVisual>().UpdateVisual(false);
            Position = GridSystem.GetGridPosition(transform.position);
            GridCellPreviousState = GridCellState.Impassable;
            GridSystem.UpdateGridObjectPosition(this, Position);
            transform.position = GridSystem.GetWorldPosition(Position);
        }

        public int MaximumHealth { get; }
        public int CurrentHealth { get; }

        public void TakeDamage(int damageDealt)
        {
            health = Mathf.Max(0, health - damageDealt);
            _onObjectDamaged?.Invoke(damageDealt);
            if (health != 0) return;
            _onObjectDeath?.Invoke(this); // Invoke is considered Expensive

            gameObject.SetActive(false);
            GridSystem.SetGridCellState(Position, GridCellPreviousState);
        }
    }
}