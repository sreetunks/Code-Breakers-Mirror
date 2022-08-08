using UnityEngine;

// TODO: Change to a Scriptable Object if possible
public class Room : MonoBehaviour
{
    public enum PivotDirection
    {
        PivotNone,
        PivotClockwise,
        PivotCounterClockwise
    };

    public enum DoorDirection
    {
        North,
        East,
        South,
        West
    }

    public DoorDirection EntryDirection
    {
        get => (DoorDirection)(((transform.rotation.eulerAngles.y / 90) + ((int)entryDirection)) % 4);
    }

    public DoorDirection ExitDirection
    {
        get => (DoorDirection)(((transform.rotation.eulerAngles.y / 90) + ((int)exitDirection)) % 4);
    }

    public PivotDirection roomPivot;
    [SerializeField] DoorDirection entryDirection;
    [SerializeField] DoorDirection exitDirection;
    [SerializeField] int roomGridWidth;
    [SerializeField] int roomGridHeight;
}
