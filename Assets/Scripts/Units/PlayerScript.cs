using System.Collections.Generic;
using UnityEngine;
using Grid;
using UnityEngine.EventSystems;

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

    [SerializeField] private Unit playerCharacter;
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] private HUDScript playerHUD;
    [SerializeField] private CameraSmoothPan mainCamera;

    PositionTargetedAbility _positionTargetedAbility;
    UnitTargetedAbility _unitTargetedAbility;
    int _targetingRange;

    Unit _selectedUnit;
    List<Unit> _controlledUnits = new List<Unit>();
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
        playerCharacter.OnUnitReachedLevelExit += OnReachedLevelExit;
        SelectUnit(playerCharacter);
    }

    private void Update()
    {
        switch (_inputState)
        {
            case InputState.Inactive: return;
            case InputState.Active:
                {
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        var targetGridPosition = GridSystem.GetGridPosition(MouseWorld.GetPosition());
                        GridSystem.TryGetGridObject(targetGridPosition, out var targetObject);
                        var targetUnit = targetObject as Unit;
                        if (targetUnit && targetUnit != _selectedUnit)
                            SelectUnit(targetUnit);
                    }
                    else if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject() && _selectedUnit && _selectedUnit.IsOnDoorGridCell && !GridSystem.ActiveLevelGrid.AreDoorsLocked)
                    {
                        var newGridPosition = GridSystem.SwitchLevelGrid(_selectedUnit.Position, _selectedUnit.GridCellPreviousState);
                        var targetPosition = GridSystem.GetWorldPosition(newGridPosition);
                        _selectedUnit.ForceMove(targetPosition);
                        _selectedUnit.transform.position = targetPosition;
                        GridSystem.UpdateGridObjectPosition(_selectedUnit, newGridPosition);
                        mainCamera.UpdateTarget(GridSystem.ActiveLevelGrid.transform);
                    }
                    else if (Input.GetKeyDown(KeyCode.K))
                    {
                        _selectedUnit.TakeDamage(1);
                    }
                    break;
                }
            case InputState.TargetingPosition:
                {
                    if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && _positionTargetedAbility != null)
                    {
                        var targetGridPosition = GridSystem.GetGridPosition(MouseWorld.GetPosition());
                        var distance = Mathf.Abs(targetGridPosition.X - CurrentlySelectedUnit.Position.X) + Mathf.Abs(targetGridPosition.Z - CurrentlySelectedUnit.Position.Z);
                        if (distance > _targetingRange) return;
                        if (_positionTargetedAbility.Use(CurrentlySelectedUnit, targetGridPosition))
                        {
                            _positionTargetedAbility = null;

                            _inputState = InputState.Active;

                            GridSystem.HighlightPosition = GridPosition.Invalid;
                            GridSystem.HighlightRange = -1;
                        }
                    }
                    else if (Input.GetMouseButton(1))
                    {
                        _positionTargetedAbility = null;
                        _inputState = InputState.Active;

                        GridSystem.HighlightPosition = GridPosition.Invalid;
                        GridSystem.HighlightRange = -1;
                    }
                    break;
                }
            case InputState.TargetingUnit:
                {
                    if (_unitTargetedAbility != null && Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        var targetGridPosition = GridSystem.GetGridPosition(MouseWorld.GetPosition());
                        GridSystem.TryGetGridObject(targetGridPosition, out var targetObject);
                        var targetUnit = targetObject as Unit;
                        if (targetUnit)
                        {
                            _unitTargetedAbility.Use(CurrentlySelectedUnit, targetUnit);
                            _unitTargetedAbility = null;

                            _inputState = InputState.Active;
                        }
                    }
                    break;
                }
        }
    }

    void OnReachedLevelExit()
    {
        playerHUD.ShowLevelCompleteScreen();
    }

    void SelectUnit(Unit unit)
    {
        if (_selectedUnit)
        {
            _selectedUnit.OnUnitDamaged -= UpdateSelectedUnitHealth;
            _selectedUnit.OnUnitAPChanged -= UpdateSelectedUnitAP;
            _selectedUnit.OnUnitDeath -= OnSelectedUnitDeath;

            _selectedUnit.GetComponentInChildren<UnitSelectedVisual>().UpdateVisual(false);
        }

        _selectedUnit = unit;
        _selectedUnit.OnUnitDamaged += UpdateSelectedUnitHealth;
        _selectedUnit.OnUnitAPChanged += UpdateSelectedUnitAP;
        _selectedUnit.OnUnitDeath += OnSelectedUnitDeath;

        playerHUD.UpdateSelectedUnit(_selectedUnit);
        _selectedUnit.GetComponentInChildren<UnitSelectedVisual>().UpdateVisual(true);
    }

    void OnSelectedUnitDeath()
    {
        SelectUnit(playerCharacter);
    }

    void OnPlayerCharacterDeath()
    {
        _selectedUnit.OnUnitDeath -= OnSelectedUnitDeath;
        playerHUD.ShowDefeatedScreen();
    }

    void UpdateSelectedUnitHealth(int damage)
    {
        playerHUD.UpdateHealth();
    }

    void UpdateSelectedUnitAP()
    {
        playerHUD.UpdateActionPoints();
    }

    public override void BeginTurn()
    {
        _inputState = InputState.Active;
        foreach (var controlledUnit in _controlledUnits)
            controlledUnit.BeginTurn();
    }

    public void EndTurn()
    {
        if(_inputState == InputState.Active) TurnOrderSystem.MoveNext();
        _inputState = InputState.Inactive;
    }

    public void UpdateTurnLabel(FactionType factionType)
    {
        playerHUD.UpdateTurnLabel(factionType.ToString());
    }

    public override void TargetAbility(Unit owningUnit, PositionTargetedAbility ability, int range)
    {
        _positionTargetedAbility = ability;
        _inputState = InputState.TargetingPosition;
        _targetingRange = range;
        GridSystem.HighlightRange = range;
        GridSystem.HighlightPosition = owningUnit.Position;
    }

    public override void TargetAbility(Unit owningUnit, UnitTargetedAbility ability)
    {
        _unitTargetedAbility = ability;
        _inputState = InputState.TargetingUnit;
    }
}
