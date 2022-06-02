using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private GameObject deathExplosion;

    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private HealthBar healthBar;

    [SerializeField, Min(1)]
    private int health;

    private float timer = 0f;


    private void Start()
    {
        healthBar.IsHealthTextActive(false);
        healthBar.SetMaxHealth((int)health);
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 1.0f / attackSpeed)
        {
            timer = 0f;
            ProjectileManager.Instance.SpawnProjectiles(projectilePrefab, transform.position + offset);
        }
    }

    public void GiveDamage(int amount)
    {
        health -= amount;
        healthBar.SetHealth(health);

        if (health <= 0)
        {
            GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("fireworks");
            ParticleSystem[] particleSystems = deathExplosion.GetComponents<ParticleSystem>();
            float delay = 0;
            foreach (ParticleSystem system in particleSystems)
            {
                if (system.main.duration > delay)
                {
                    delay = system.main.duration;
                }
            }

            for (int i = 0; i < 15; i++)
            {
                var obj = Instantiate(deathExplosion, transform.position + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10) + 10, Random.Range(-10, 10)), Quaternion.identity);
                Destroy(obj, delay);
            }

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.enemies.Remove(GetComponent<Enemy>());
                LevelManager.Instance.ActionsIfCleared();
            }
            Destroy(gameObject);
        }
    }

    public int GetHealth()
    {
        return this.health;
    }
}
