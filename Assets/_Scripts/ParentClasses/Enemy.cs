using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    public abstract int Score { get; set; }
    public abstract AudioSource EnemyAudioSource { get; set; }
    public abstract GameManagement GamesManager { get; set; }
    public abstract float Health { get; set; }
    public abstract float Damage { get; set; }
    public abstract float TimeBetweenAttacks { get; set; }
   

    public abstract IEnumerator Attack(Player player);
    public abstract void TakeDamage(float damageAmount);
    public virtual void Die()
    {
        GamesManager.AddToScore(Score);
        Destroy(gameObject);
    }
    public abstract void PlayAwareNoise();

}
