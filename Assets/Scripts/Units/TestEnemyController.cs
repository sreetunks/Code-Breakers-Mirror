using UnityEngine;
using System.Collections;

public class TestEnemyController : Controller
{
    public override FactionType Faction => FactionType.Enemy;

    public override void BeginTurn()
    {
        StartCoroutine(DelayedTurn());
    }

    IEnumerator DelayedTurn()
    {
        yield return new WaitForSeconds(2.0f);

        TurnOrderSystem.MoveNext();
    }
}
