using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public bool isPaused = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
