using System.Collections.Generic;
using Units;
using UnityEngine;
using Grid;

public class TurnOrderSystem : MonoBehaviour
{
    [SerializeField] private Controller playerController;

    private static TurnOrderSystem Instance { get; set; }
    public static Controller ActiveController => Instance._activeController;

    private readonly List<Controller> _registeredControllers = new List<Controller>();
    private List<Controller> _perTurnControllerList;
    private Controller _activeController;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TurnOrderSystem! " + transform + " - " + Instance);
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RegisterController(playerController);
        Instance._perTurnControllerList = new List<Controller>(Instance._registeredControllers);
    }

    public static void RegisterLevelGrid()
    {
        var levelEnemyManager = GridSystem.ActiveLevelGrid.GetComponent<LevelEnemyManager>();
        bool enemyControllersActive = false;
        foreach (var enemyController in levelEnemyManager.EnemyControllers)
        {
            if (enemyController.IsActive)
            {
                RegisterController(enemyController);
                enemyControllersActive = true;
            }
        }
        GridSystem.ActiveLevelGrid.SetDoorLock(enemyControllersActive);
    }

    private static void RegisterController(Controller controller)
    {
        Instance._registeredControllers.Add(controller);
        controller.Initialize();
    }

    public static void DeregisterController(Controller controller)
    {
        Instance._perTurnControllerList.Remove(controller);
        Instance._registeredControllers.Remove(controller);

        if (Instance._registeredControllers.Count == 1)
            GridSystem.ActiveLevelGrid.SetDoorLock(false);
    }

    public static void MoveNext()
    {
        if (Instance._perTurnControllerList.Count > 1)
            Instance._perTurnControllerList.RemoveAt(0);
        else if(Instance._registeredControllers.Count > 1)
            Instance._perTurnControllerList = new List<Controller>(Instance._registeredControllers);

        Instance._activeController = Instance._perTurnControllerList[0];
        ActiveController.BeginTurn();
        PlayerScript.Instance.UpdateTurnLabel(ActiveController.Faction);
    }
}
