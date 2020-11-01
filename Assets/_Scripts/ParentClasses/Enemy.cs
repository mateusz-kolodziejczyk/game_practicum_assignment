using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    public abstract float Health{ get; }
    public abstract float Damage { get; }
    public abstract float TimeBetweenAttacks { get; }
    public abstract int Score { get; set; }
    public abstract AudioClip AwareNoise { get; set; }
    public abstract AudioSource EnemyAudioSource { get; set; }
    public abstract GameManagement GamesManager { get; set; }
   

    public abstract IEnumerator Attack(Player player);
    public abstract void TakeDamage(float damageAmount);
    public void Die()
    {
        GamesManager.addToScore(Score);
        Destroy(gameObject);
    }
    public abstract void playAwareNoise();

}
