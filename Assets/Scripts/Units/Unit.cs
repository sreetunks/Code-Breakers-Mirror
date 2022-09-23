using System.Collections.Generic;
using System.Linq;
using Abilities;
using Grid;
using UnityEngine;

namespace Units
{
    public class Unit : MonoBehaviour, IGridObject, IDamageable
    {
        [System.Serializable]
        public class AbilityData
        {
            public AbilityBase ability;
            public int cooldownDuration;

            public AbilityData(AbilityBase ability)
            {
                this.ability = ability;
                this.cooldownDuration = 0;
            }
        }

        public delegate void OnUnitDeathEventHandler(Unit unit);
        public OnUnitDeathEventHandler OnUnitDeath;

        public delegate void OnUnitDamagedEventHandler(int damageDealt);
        public OnUnitDamagedEventHandler OnUnitDamaged;

        public delegate void OnUnitAPChangedEventHandler();
        public OnUnitAPChangedEventHandler OnUnitAPChanged;

        public delegate void OnUnitActionFinishedEventHandler();
        public OnUnitActionFinishedEventHandler OnUnitActionFinished;

        [SerializeField] private Animator unitAnimator;
        [SerializeField] private AudioSource unitAudioSource;
        [SerializeField] private Canvas worldSpaceCanvas;

        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private ParticleSystem shieldEffect;

        [SerializeField] private List<AbilityBase> unitAbilities;

        [SerializeField] private AudioClip footStepSFX;
        [SerializeField] private AudioClip healSFX;

        [SerializeField] private int maximumHealth = 4;
        [SerializeField] private int maximumAP = 6;
        [SerializeField] private int apGainPerRound = 3;

        private List<GridPosition> _path;

        private int _currentHealth;
        private int _currentAP;
        private int _currentShields;
        private List<AbilityData> _unitAbilityDataList;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking"); // Caching ID for Parameter

        public Controller Controller { get; set; }
        public AudioSource AudioSource => unitAudioSource;

        public List<AbilityData> AbilityList => _unitAbilityDataList;

        public GridCellState GridCellPreviousState { get; set; }
        public GridPosition Position { get; private set; }
        public bool IsOnDoorGridCell { get; private set; }
        public bool IsOnLevelExit { get; private set; }

        public int MaximumHealth => maximumHealth;
        public int CurrentHealth => _currentHealth;

        public int MaximumAP => maximumAP;
        public int CurrentAP => _currentAP;

        public int CurrentShields => _currentShields;

        private void Awake()
        {
            _currentHealth = maximumHealth;
            _path = new List<GridPosition>();

            _unitAbilityDataList = new List<AbilityData>();
            foreach (var ability in unitAbilities)
                _unitAbilityDataList.Add(new AbilityData(ability));
        }

        private void Update()
        {
            UpdateMove();
        }

        private void UpdateMove()
        {
            if (_path.Count <= 0) return;
            var toTarget = GridSystem.GetWorldPosition(_path[0]) - transform.position;
            var dist = toTarget.magnitude;

            if (dist > 0.01)
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
            {
                var newGridPosition = GridSystem.GetGridPosition(transform.position);
                if (newGridPosition == Position) return;
                GridSystem.UpdateGridObjectPosition(this, newGridPosition);
                Position = newGridPosition;
                IsOnDoorGridCell = CheckIsOnDoorGridCell(GridCellPreviousState);
                IsOnLevelExit = GridCellPreviousState == GridCellState.LevelExit;
                _path.RemoveAt(0);

                if (_path.Count == 0)
                {
                    OnUnitActionFinished?.Invoke();

                    unitAnimator.SetBool(IsWalking, false); // Ends "Walk" Animations
                    unitAudioSource.loop = false;
                    unitAudioSource.Stop();
                }

                if (IsOnDoorGridCell || IsOnLevelExit && Controller.Faction == Controller.FactionType.Player)
                {
                    worldSpaceCanvas.transform.rotation = Quaternion.Euler(60, 0, 0);
                    worldSpaceCanvas.enabled = true;
                }
            }
        }

        private static bool CheckIsOnDoorGridCell(GridCellState gridCellState)
        {
            return gridCellState is GridCellState.DoorNorth or GridCellState.DoorEast or GridCellState.DoorSouth or GridCellState.DoorWest;
        }

        public void Spawn()
        {
            GetComponentInChildren<UnitSelectedVisual>().UpdateVisual(false);
            Position = GridSystem.GetGridPosition(transform.position);
            GridCellPreviousState = GridCellState.Impassable;
            GridSystem.UpdateGridObjectPosition(this, Position);
            transform.position = GridSystem.GetWorldPosition(Position);
            IsOnDoorGridCell = CheckIsOnDoorGridCell(GridCellPreviousState);
        }

        public void ForceMove(Vector3 targetPosition)
        {
            var targetGridPosition = GridSystem.GetGridPosition(targetPosition);
            GridSystem.TryGetGridCellState(targetGridPosition, out var targetCellState);
            if (targetCellState is GridCellState.Impassable or GridCellState.Occupied or GridCellState.OccupiedEnemy) return;
            Position = targetGridPosition;
            IsOnDoorGridCell = CheckIsOnDoorGridCell(targetCellState);
        }

        public void Move(List<GridPosition> inPath)
        {
            _path = inPath;
            worldSpaceCanvas.enabled = false;
            unitAudioSource.clip = footStepSFX;
            unitAudioSource.loop = true;
            unitAudioSource.Play();
        }

        public void TakeDamage(int damageDealt)
        {
            if (_currentShields > 0)
            {
                var shieldsRemaining = Mathf.Max(0, _currentShields - damageDealt);
                damageDealt -= _currentShields;
                _currentShields = shieldsRemaining;
                if (!(damageDealt > 0))
                {
                    OnUnitDamaged?.Invoke(damageDealt);
                    shieldEffect.Play();
                    return;
                }
            }

            _currentHealth = Mathf.Max(0, _currentHealth - damageDealt);
            OnUnitDamaged?.Invoke(damageDealt);
            hitEffect.Play();
            if (_currentHealth == 0)
            {
                OnUnitDeath?.Invoke(this); // Invoke is considered Expensive

                gameObject.SetActive(false);
                GridSystem.UpdateGridObjectPosition(this, GridPosition.Invalid);
            }
        }

        public void Heal(int healthRestored)
        {
            _currentHealth = Mathf.Min(maximumHealth, _currentHealth + healthRestored);
            OnUnitDamaged?.Invoke(-healthRestored);
            unitAudioSource.PlayOneShot(healSFX);
        }

        public void GainShields(int shieldsGained)
        {
            _currentShields = shieldsGained;
            unitAudioSource.PlayOneShot(healSFX);
            OnUnitDamaged?.Invoke(0);
        }

        public void ConsumeAP(int apToConsume)
        {
            _currentAP -= apToConsume;
            OnUnitAPChanged?.Invoke();
        }

        public void OnAbilityUsed(AbilityBase ability)
        {
            int idx = _unitAbilityDataList.FindIndex(abilityData => abilityData.ability == ability);
            if (idx < 0) return;
            _unitAbilityDataList[idx].cooldownDuration = ability.CooldownDuration;

            OnUnitAPChanged?.Invoke();
            OnUnitActionFinished?.Invoke();
        }

        public int GetAbilityCooldown(AbilityBase ability)
        {
            int idx = _unitAbilityDataList.FindIndex(abilityData => abilityData.ability == ability);
            if (idx < 0) return -1;
            return _unitAbilityDataList[idx].cooldownDuration;
        }

        public void BeginTurn()
        {
            if (_currentShields > 0)
            {
                _currentShields = 0;
                OnUnitDamaged?.Invoke(0);
            }

            _currentAP = Mathf.Min(maximumAP, _currentAP + apGainPerRound);

            foreach (var abilityData in _unitAbilityDataList.Where(abilityData => abilityData.cooldownDuration > 0)) --abilityData.cooldownDuration;

            OnUnitAPChanged?.Invoke();
        }

        public void EndTurn()
        {
        }

        public void SetWorldSpaceCanvasActive(bool active)
        {
            worldSpaceCanvas.enabled = active;
        }
    }
}
