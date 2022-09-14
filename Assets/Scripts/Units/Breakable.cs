using Grid;
using UnityEngine;

namespace Units
{
    public class Breakable : MonoBehaviour, IGridObject, IDamageable
    {
        public delegate void OnObjectDamagedEventHandler(int damageDealt);
        public delegate void OnObjectDeathEventHandler(Breakable unit);

        private readonly OnObjectDeathEventHandler _onObjectDeath;
        private readonly OnObjectDamagedEventHandler _onObjectDamaged;

        [SerializeField] private int maximumHealth;

        private int _currentHealth;

        public GridCellState GridCellPreviousState { get; set; }
        public GridPosition Position { get; private set; }

        private void Start()
        {
            _currentHealth = maximumHealth;

            Position = GridSystem.GetGridPosition(transform.position);
            GridCellPreviousState = GridCellState.Impassable;
            GridSystem.UpdateGridObjectPosition(this, Position);
            transform.position = GridSystem.GetWorldPosition(Position);
        }

        public int MaximumHealth => maximumHealth;
        public int CurrentHealth => _currentHealth;

        public void TakeDamage(int damageDealt)
        {
            _currentHealth = Mathf.Max(0, _currentHealth - damageDealt);
            _onObjectDamaged?.Invoke(damageDealt);
            if (_currentHealth != 0) return;
            _onObjectDeath?.Invoke(this); // Invoke is considered Expensive

            gameObject.SetActive(false);
            GridSystem.SetGridCellState(Position, GridCellPreviousState);
        }
    }
}
