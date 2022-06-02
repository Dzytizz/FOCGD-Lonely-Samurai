using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MultiplayerEnemy : NetworkBehaviour
{
    [SerializeField] protected HealthBar healthBar;
    protected int maxHealth;
    protected float attackSpeed;
    protected float movementSpeed;
    public GameObject[] targets;
    protected float colliderOffset;
    [SyncVar(hook = nameof(RequestUpdateHealth))] int currentHealth;

    private void Awake()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        attackSpeed = 0.5f;
        movementSpeed = 20;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.IsHealthTextActive(false);
        colliderOffset = GetComponent<SphereCollider>().radius;
        targets = GameObject.FindGameObjectsWithTag("Player");
    }

    [Command(requiresAuthority = false)]
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public bool WillDieFromDamage(int damage)
    {
        return currentHealth - damage <= 0;
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void RequestUpdateHealth(int oldValue, int newValue)
    {
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            DestroyEnemy();
        }
    }
}
