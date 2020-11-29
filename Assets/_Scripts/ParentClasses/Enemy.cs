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
    public abstract float MaxHealth { get; set; }
    public abstract float Damage { get; set; }
    public abstract float TimeBetweenAttacks { get; set; }

    public abstract Material EnemyDamagedMaterial { get; set; }
    public abstract Material OriginalMaterial { get; set; }
    public abstract Renderer EnemyRenderer { get; set; }
    public abstract IEnumerator DamagedColorCoroutine { get; set; }
    public abstract GameObject HealthBar {get; set;}
    public abstract Transform Target { get; set; }

    public abstract IEnumerator Attack(Player player);
    public void TakeDamage(float damageAmount)
    {
        // set the entir healthbar(background and active health to active)
        if (!HealthBar.activeInHierarchy)
        {
            HealthBar.transform.parent.gameObject.SetActive(true);
            foreach (Transform child in HealthBar.transform.parent)
            {
                child.gameObject.SetActive(true);
            }
        }

        Health -= damageAmount;


        var HealthSize = HealthBar.transform.localScale;
        HealthSize.x = Health / MaxHealth;

        HealthBar.transform.localScale = HealthSize;
        // Change enemy color when hit
        if (DamagedColorCoroutine != null)
        {
            StopCoroutine(DamagedColorCoroutine);
        }
        DamagedColorCoroutine = ChangeMaterial(EnemyDamagedMaterial, 0.15f);
        StartCoroutine(DamagedColorCoroutine);
        if (Health <= 0.1f)
        {
            Die();
        }
    }
    public virtual void Update()
    {
        // Only rotate if its active
        if(HealthBar != null && HealthBar.activeInHierarchy && Target != null)
        {
            Vector3 v3 = Target.position - HealthBar.transform.parent.position;
            v3.y = 0.0f;
            HealthBar.transform.parent.rotation = Quaternion.LookRotation(-v3);
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
