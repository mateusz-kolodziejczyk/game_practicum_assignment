using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyHandleFSM : EnemyHandleFSM 
{
    public override void Start()
    {
        base.Start();
        SetIdle();
    }
    public override void Update()
    {
    }
    public void SetAttacking()
    {
        anim.SetBool("isAttacking", true);
        anim.SetBool("isPatrolling", false);
    }
    public void SetPatrolling()
    {
        anim.SetBool("isAttacking", false);
        anim.SetBool("isPatrolling", true);
        if(WaypointTransforms.Count < 1)
        {
            SetIdle();
        }
    }
    public void SetIdle()
    {
        anim.SetBool("isAttacking", false);
        anim.SetBool("isPatrolling", false);
    }
    public override void IsPatrolling(bool isPatrolling)
    {
        if (isPatrolling)
        {
            SetPatrolling();
        }
        else
        {
            SetIdle();
        }
    }
    public override void IsAttacking(bool isAttacking)
    {
        if (isAttacking)
        {
            SetAttacking();
        }
        else
        {
            SetIdle();
        }
    }
}
