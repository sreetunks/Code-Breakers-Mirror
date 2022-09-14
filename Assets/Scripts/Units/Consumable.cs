using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;

public class Consumable : MonoBehaviour, IGridObject
{
    public delegate void OnPickupEvent(Consumable consumable);
    public OnPickupEvent OnPickup;

    public GridCellState GridCellPreviousState { get; set; }
    public GridPosition Position { get; private set; }

    [SerializeField] private int overheal;
    [SerializeField] private int shield;
    [SerializeField] private int APBoost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickup?.Invoke(this);
        }
    }
}
