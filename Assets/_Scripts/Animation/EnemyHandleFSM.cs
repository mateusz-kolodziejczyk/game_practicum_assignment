using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHandleFSM : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    AnimatorStateInfo info;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void CanSeePlayer(bool canSeePlayer)
    {
        anim.SetBool("canSeePlayer", canSeePlayer);
    }
}
