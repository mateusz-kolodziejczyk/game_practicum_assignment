using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBoss : BasicEnemy
{
    GameManagement gameManagement;
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
    }

    public override void Die()
    {
        gameManagement.OpenLevelExit();
        Destroy(gameObject);
    }
    
}
