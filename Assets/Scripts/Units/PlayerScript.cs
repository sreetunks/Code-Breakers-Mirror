using System;
using System.Collections.Generic;
using Abilities;
using Grid;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Units
{
    public class PlayerScript : Controller
    {
        enum InputState
        {
            Inactive,
            Active,
            TargetingPosition,
            TargetingUnit
        }

        public static PlayerScript Instance { get; private set; }
        public static Unit PlayerCharacter => Instance.playerCharacter;
        public static Unit CurrentlySelectedUnit => Instance._selectedUnit;

        public override FactionType Faction => FactionType.Player;

        public override bool IsActive => true;

        [SerializeField] private Unit playerCharacter;
        [SerializeField] private LayerMask unitLayerMask;
        [SerializeField] private HUDScript playerHUD;
        [SerializeField] private CameraSmoothPan mainCamera;

        PositionTargetedAbility _positionTargetedAbility;
        UnitTargetedAbility _unitTargetedAbility;
        int _targetingRange;

        Unit _selectedUnit;
        readonly List<Unit> _controlledUnits = new List<Unit>();
        private List<Unit> _perTurnUnitList;
        InputState _inputState;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogError("Scene contains multiple PlayerScript components");
#if UNITY_EDITOR
                UnityEditor.EditorGUIUtility.PingObject(gameObject);
#endif
                return;
            }

            Instance = this;
            _controlledUnits.Add(playerCharacter);
            playerCharacter.Controller = this;
        }

        private void Start()
        {
            playerCharacter.OnUnitDeath += OnPlayerCharacterDeath;
            TurnOrderSystem.MoveNext();
            SelectUnit(playerCharacter);
        }

        private void Update()
        {
            switch (_inputState)
            {
                case InputState.Inactive: return;
                case InputState.Active:
                    {
                        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && MouseWorld.GetPosition(out var targetWorldPosition))
                        {
                            var targetGridPosition = GridSystem.GetGridPosition(targetWorldPosition); // Get Position is considered Expensive
                            GridSystem.TryGetGridObject(targetGridPosition, out var targetObject);
                            var targetUnit = targetObject as Unit;
                            if (targetUnit && targetUnit != _selectedUnit)
                                SelectUnit(targetUnit); // Select unit is considered Expensive
                        }
                        else if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject() && !GridSystem.ActiveLevelGrid.AreDoorsLocked)
                        {
                            if (playerCharacter.IsOnDoorGridCell)
                            {
                                var newGridPosition = GridSystem.SwitchLevelGrid(_selectedUnit.Position, _selectedUnit.GridCellPreviousState); // Switch Level Grid is considered Expensive
                                var targetPosition = GridSystem.GetWorldPosition(newGridPosition);
                                _selectedUnit.ForceMove(targetPosition);
                                _selectedUnit.transform.position = targetPosition;
                                GridSystem.UpdateGridObjectPosition(_selectedUnit, newGridPosition);
                                TurnOrderSystem.MoveNext();
                            }
                            else if (playerCharacter.IsOnLevelExit)
                            {
                                playerHUD.ShowLevelCompleteScreen();
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.K))
                        {
                            _selectedUnit.TakeDamage(1); // Take Damage is considered Expensive
                        }
                        break;
                    }
                case InputState.TargetingPosition:
                    {
                        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && _positionTargetedAbility != null && MouseWorld.GetPosition(out var targetPosition)) // Comparing to Null is considered Expensive
                        {
                            var targetGridPosition = GridSystem.GetGridPosition(targetPosition); // Get Position is considered Expensive
                            if (_positionTargetedAbility.Use(CurrentlySelectedUnit, targetGridPosition))
                            {
                                CancelAbility();
                                playerHUD.SetHUDButtonsActive(false);
                                _inputState = InputState.Inactive;
                                CurrentlySelectedUnit.OnUnitActionFinished += OnSelectedUnitActionFinished;
                            }
                        }
                        else if (Input.GetMouseButton(1))
                        {
                            CancelAbility();
                        }
                        break;
                    }
                case InputState.TargetingUnit:
                    {
                        if (_unitTargetedAbility != null && Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && MouseWorld.GetPosition(out var targetPosition))
                        {
                            var targetGridPosition = GridSystem.GetGridPosition(targetPosition); // Get Position is considered Expensive
                            GridSystem.TryGetGridObject(targetGridPosition, out var targetObject);
                            var targetUnit = targetObject as Unit;
                            if (targetUnit && _unitTargetedAbility.Use(CurrentlySelectedUnit, targetUnit))
                            {
                                CancelAbility();
                            }
                        }
                        else if (Input.GetMouseButton(1))
                        {
                            CancelAbility();
                        }
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                playerHUD.ShowPauseScreen();
            }
        }

        private void SelectUnit(Unit unit)
        {
            if (_selectedUnit)
            {
                _selectedUnit.OnUnitDamaged -= UpdateSelectedUnitHealth;
                _selectedUnit.OnUnitAPChanged -= UpdateSelectedUnitAP;
                _selectedUnit.OnUnitDeath -= OnSelectedUnitDeath;

                _selectedUnit.GetComponentInChildren<UnitSelectedVisual>().UpdateVisual(false); // Get Component In Childern is considered Expensive
            }

            _selectedUnit = unit;
            _selectedUnit.OnUnitDamaged += UpdateSelectedUnitHealth;
            _selectedUnit.OnUnitAPChanged += UpdateSelectedUnitAP;
            _selectedUnit.OnUnitDeath += OnSelectedUnitDeath;
            mainCamera.UpdateTarget(_selectedUnit.transform);

            playerHUD.UpdateSelectedUnit(_selectedUnit);
            _selectedUnit.GetComponentInChildren<UnitSelectedVisual>().UpdateVisual(true); // Get Component In Childern is considered Expensive
        }

        private void OnSelectedUnitDeath(Unit unit)
        {
            SelectUnit(playerCharacter); // Select Unit is considered Expensive
        }

        private void OnSelectedUnitActionFinished()
        {
            if (CurrentlySelectedUnit != null) CurrentlySelectedUnit.OnUnitActionFinished -= OnSelectedUnitActionFinished; // Comparison to Null is Expensive
            _inputState = InputState.Active;
            playerHUD.SetHUDButtonsActive(true);
        }

        private void OnPlayerCharacterDeath(Unit unit)
        {
            _selectedUnit.OnUnitDeath -= OnSelectedUnitDeath;
            playerHUD.ShowDefeatedScreen();
        }

        private void UpdateSelectedUnitHealth(int damage)
        {
            playerHUD.UpdateHealth();
        }

        private void UpdateSelectedUnitAP()
        {
            playerHUD.UpdateActionPoints();
        }

        public override void BeginTurn()
        {
            _inputState = InputState.Active;

            foreach (var controlledUnit in _controlledUnits)
                controlledUnit.BeginTurn();

            _perTurnUnitList = new List<Unit>(_controlledUnits);
            SelectUnit(_perTurnUnitList[0]);
            playerHUD.SetHUDButtonsActive(true);
        }

        public void EndTurn()
        {
            if (_inputState != InputState.Active)
                return;

            _selectedUnit.EndTurn();
            _perTurnUnitList.Remove(_selectedUnit);

            if (_perTurnUnitList.Count > 0)
                SelectUnit(_perTurnUnitList[0]);
            else
            {
                _inputState = InputState.Inactive;
                playerHUD.SetHUDButtonsActive(false);
                TurnOrderSystem.MoveNext();
                playerHUD.UpdateSelectedUnit(_selectedUnit);
            }
        }

        public void UpdateTurnLabel(FactionType factionType)
        {
            playerHUD.UpdateTurnLabel(factionType.ToString());
        }

        public void CancelAbility()
        {
            if (_inputState == InputState.TargetingPosition)
                _positionTargetedAbility = null;
            else if (_inputState == InputState.TargetingUnit)
                _unitTargetedAbility = null;

            _inputState = InputState.Active;

            GridSystem.ResetGridRangeInfo();
            GridSystem.HighlightPosition = GridPosition.Invalid;
            GridSystem.HighlightRange = -1;

            playerHUD.SetCancelActionButtonEnabled(false);
            playerHUD.SetHUDButtonsActive(true);
        }

        public override void TargetAbility(Unit owningUnit, PositionTargetedAbility ability, int range)
        {
            _positionTargetedAbility = ability;

            _inputState = InputState.TargetingPosition;
            playerHUD.SetHUDButtonsActive(false);

            _targetingRange = range;
            GridSystem.HighlightRange = range;
            GridSystem.HighlightPosition = owningUnit.Position;

            playerHUD.SetCancelActionButtonEnabled(true);
        }

        public override void TargetAbility(Unit owningUnit, UnitTargetedAbility ability, int range)
        {
            _unitTargetedAbility = ability;

            _inputState = InputState.TargetingUnit;
            playerHUD.SetHUDButtonsActive(false);

            GridSystem.HighlightRange = range;
            GridSystem.HighlightPosition = owningUnit.Position;

            playerHUD.SetCancelActionButtonEnabled(true);
        }

        public override void Initialize()
        {
            foreach (var controlledUnit in _controlledUnits)
                controlledUnit.Spawn();
        }
    }
}
