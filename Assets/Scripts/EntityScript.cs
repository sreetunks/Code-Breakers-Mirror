using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityScript : Unit
{
    [SerializeField] public int HPMax = 1;
    [SerializeField] public int HP;
    [SerializeField] public int APMax;
    [SerializeField] public int AP;
    [SerializeField] public int APRefresh;
    // Start is called before the first frame update
    void Start()
    {
        HP = HPMax;
        AP = APMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Damage(int dmg)
    {
        HP -= dmg;
        if (HP <= 0)
        {
            //do entity death
        }        
    }
    public void Heal(int hp)
    {
        HP += hp;
        if (HP > HPMax)
        {
            HP = HPMax;
        }
        
    }
    public void UseAP(int ap)
    {
        AP -= ap;
        if (AP < 0)
        {
            AP = 0;
        }
        
    }
    public void SetStatus()
    {

    }
    public void StartTurn()
    {
        AP += APRefresh;
        if (AP > APMax)
        {
            AP = APMax;
        }
    }
}
