using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class TurnOrderSystem : MonoBehaviour
{
    [SerializeField] private Controller playerController;
    private static TurnOrderSystem Instance { get; set; }

    private readonly List<Controller> _registeredControllers = new List<Controller>();
    private List<Controller> _controllerTurnOrderList = new List<Controller>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystem! " + transform + " - " + Instance);
            Destroy(this);
            return;
        }

        Instance = this;

        RegisterController(playerController);

        var levelEnemyManager = Grid.GridSystem.ActiveLevelGrid.GetComponent<LevelEnemyManager>();
        foreach (var enemyController in levelEnemyManager.EnemyControllers)
            RegisterController(enemyController);
    }

    private void Start()
    {
        MoveNext();
    }

    private static void RegisterController(Controller controller)
    {
        Instance._registeredControllers.Add(controller);
    }

    public static void DeregisterController(Controller controller)
    {
        Instance._registeredControllers.Remove(controller);
    }

    public static void MoveNext()
    {
        if (Instance._controllerTurnOrderList.Count > 0)
        {
            Instance._controllerTurnOrderList.RemoveAt(0);
        }

        // If there are controllers left still to take their turn, Begin their turn.
        if (Instance._controllerTurnOrderList.Count > 0)
        {
            Instance._controllerTurnOrderList[0].BeginTurn();
        }
        // If all controllers have taken their turn, begin the next round.
        else
        {
            Instance._controllerTurnOrderList = new List<Controller>(Instance._registeredControllers);
            Instance._controllerTurnOrderList[0].BeginTurn();
        }

        PlayerScript.Instance.UpdateTurnLabel(Instance._controllerTurnOrderList[0].Faction);
    }
}
