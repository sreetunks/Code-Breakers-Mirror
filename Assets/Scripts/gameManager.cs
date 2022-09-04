using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public static PlayerScript player;
    public bool isPaused = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = new PlayerScript();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
