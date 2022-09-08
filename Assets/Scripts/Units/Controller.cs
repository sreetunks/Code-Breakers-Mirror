using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public enum FactionType
    {
        Neutral,
        Player,
        Enemy
    }

    public abstract FactionType Faction { get; }

    public abstract void BeginTurn();
}
