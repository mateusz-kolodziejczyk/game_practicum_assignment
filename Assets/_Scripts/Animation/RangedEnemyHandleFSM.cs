using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyHandleFSM : EnemyHandleFSM 
{
    public void SetAttacking()
    {
        anim.SetBool("isAttacking", true);
        anim.SetBool("isPatrolling", false);
    }
    public void SetPatrolling()
    {
        anim.SetBool("isAttacking", false);
        anim.SetBool("isPatrolling", true);
        if(WaypointTransforms.Count <= 1)
        {
            SetIdle();
        }
    }
    public void SetIdle()
    {
        anim.SetBool("isAttacking", false);
        anim.SetBool("isPatrolling", false);
    }
}
