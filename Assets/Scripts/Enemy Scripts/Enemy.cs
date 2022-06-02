using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Core actions for all enemies (death, drops, taking damage, etc.)
public class Enemy : MonoBehaviour
{
    [SerializeField] protected HealthBar healthBar;

    protected GameObject model; // NOT IMPLEMENTED YET
    protected int health;
    protected float attackSpeed;
    protected float movementSpeed;
    protected int coins;

    public GameObject target;
    protected Player player;

    protected float colliderOffset;
    private int currentHealth;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        player = target.GetComponent<Player>();

        healthBar.IsHealthTextActive(false);

        colliderOffset = GetComponent<SphereCollider>().radius;
    }

    public void SetData(EnemySO data)
    {
        this.model = data.model;
        this.health = data.health;
        this.attackSpeed = data.attackSpeed;
        this.movementSpeed = data.movementSpeed;
        this.coins = data.coins;

        currentHealth = data.health;
        healthBar.SetMaxHealth(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        StartCoroutine(AddForce());


        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("blood");
            DestroyEnemy2();
            DestroyEnemy();

        }
        healthBar.SetHealth((int)currentHealth);
    }

    public IEnumerator AddForce()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 force = (target.transform.position - gameObject.transform.position) * -10;
        rb.AddForce(force, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector3.zero;
    }

    public bool WillDieFromDamage(int damage)
    {
        return currentHealth - damage <= 0;
    }

    public int GetCoins()
    {
        return coins;
    }

    public void DestroyEnemy()
    {
        //SpawnCoins();
        if(LevelManager.Instance != null)
        {
            LevelManager.Instance.enemies.Remove(this);
            LevelManager.Instance.ActionsIfCleared();
        }
        Destroy(gameObject);
    }
    public void DestroyEnemy2()
    {
        Destroy(gameObject);
    }
    public void SpawnCoins()
    {
        throw new NotImplementedException("SpawnCoins() not iplemented");
    }
}
