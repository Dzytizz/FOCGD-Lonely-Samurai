using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MultiplayerPlayer : NetworkBehaviour
{
    [SyncVar(hook=nameof(RequestUpdateScore))] public int points;
    [SyncVar(hook = nameof(RequestUpdateHealth))] public int currentHealth;
    public int maxHealth;
    public int damage;
    public bool isDashing = false;
    [SerializeField] private HealthBar healthBar;
    private MultiplayerScoreUIManager scoreUIManager;

    void Start()
    {
        maxHealth = 350;
        damage = 30;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(350);
        scoreUIManager = GameObject.Find("Score UI Manager").GetComponent<MultiplayerScoreUIManager>();
    }

    [Command(requiresAuthority = false)]
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    private void PlayerDie()
    {
        GameObject gameManager = GameObject.Find("Game Manager");
        gameManager.GetComponent<MultiplayerGameManager>().SetGameIsOver();
    }

    [Command]
    private void AddPoints(int p)
    {
        points += p;
    }

    private void RequestUpdateScore(int oldValue, int newValue)
    {
        scoreUIManager.UpdateScore();
    }

    private void RequestUpdateHealth(int oldValue, int newValue)
    {
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            PlayerDie();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && isDashing)
        {
            MultiplayerEnemy enemy = other.GetComponent<MultiplayerEnemy>();
            if (enemy.WillDieFromDamage(damage))
            {
                AddPoints(1);
            }
            enemy.TakeDamage(damage);
        }
    }
}
