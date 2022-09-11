using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class LevelEnemyManager : MonoBehaviour
    {
        [SerializeField] private List<Controller> enemyControllers;

        public List<Controller> EnemyControllers => enemyControllers;
    }
}
