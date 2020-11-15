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

    public abstract Material EnemyDamagedMaterial { get; set; }
    public abstract Material OriginalMaterial { get; set; }
    public abstract Renderer EnemyRenderer { get; set; }
    public abstract IEnumerator DamagedColorCoroutine { get; set; }

   

    public abstract IEnumerator Attack(Player player);
    public void TakeDamage(float damageAmount)
    {
        Health -= damageAmount;
        // Change enemy color when hit
        if (DamagedColorCoroutine != null)
        {
            StopCoroutine(DamagedColorCoroutine);
        }
        DamagedColorCoroutine = ChangeMaterial(EnemyDamagedMaterial, 0.15f);
        StartCoroutine(DamagedColorCoroutine);
        if (Health < 0.0f)
        {
            Die();
        }
    }
    public IEnumerator ChangeMaterial(Material material, float time)
    {
        EnemyRenderer.material = material;
        yield return new WaitForSeconds(time);
        EnemyRenderer.material = OriginalMaterial;
    }
    public virtual void Die()
    {
        GamesManager.AddToScore(Score);
        Destroy(gameObject);
    }
    public abstract void PlayAwareNoise();

}
