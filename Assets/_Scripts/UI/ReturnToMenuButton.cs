using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMenuButton : MonoBehaviour
{
    private GameManagement gameManagement;
    // Start is called before the first frame update
    void Awake()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
