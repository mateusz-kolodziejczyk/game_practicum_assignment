﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBoss : BasicEnemy
{
    GameManagement gameManagement;
    [SerializeField]
    GameObject droppableItem;
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
    }

    public override void Die()
    {
        gameManagement.OpenLevelExit();
        DropItem();
        Destroy(gameObject);
    }

    private void DropItem()
    {
        Instantiate(droppableItem, transform.position, Quaternion.identity);
    }
    
}
