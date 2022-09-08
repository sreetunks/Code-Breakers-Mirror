using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnemyManager : MonoBehaviour
{
    [SerializeField] private List<Controller> enemyControllers;

    public List<Controller> EnemyControllers => enemyControllers;
}
