using System.Collections.Generic;
using UnityEngine;
using Grid;
using UnityEngine.EventSystems;

public class PlayerScript : Controller
{
    public static PlayerScript Instance { get; private set; }
    public static Unit CurrentlySelectedUnit => Instance._selectedUnit;

    public override FactionType Faction => FactionType.Player;

    [SerializeField] private Unit playerCharacter;
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] private HUDScript playerHUD;

    Unit _selectedUnit;
    List<Unit> _controlledUnits = new List<Unit>();
    bool _isTurnActive = false;

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
        SelectUnit(playerCharacter);
    }

    private void Update()
    {
        if (!_isTurnActive) return;

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
            _selectedUnit.Move(targetPosition, forceMove: true);
            _selectedUnit.transform.position = targetPosition;
            GridSystem.UpdateGridObjectPosition(_selectedUnit, newGridPosition);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            _selectedUnit.TakeDamage(1);
        }
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
        _isTurnActive = true;
        foreach (var controlledUnit in _controlledUnits)
            controlledUnit.BeginTurn();
    }

    public void EndTurn()
    {
        if(_isTurnActive) TurnOrderSystem.MoveNext();
        _isTurnActive = false;
    }

    public void UpdateTurnLabel(FactionType factionType)
    {
        playerHUD.UpdateTurnLabel(factionType.ToString());
    }
}
