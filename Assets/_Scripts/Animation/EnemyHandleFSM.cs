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
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
    }

    public void IsMoving(bool isMoving)
    {
        anim.SetBool("isMoving", isMoving);
    }
}
