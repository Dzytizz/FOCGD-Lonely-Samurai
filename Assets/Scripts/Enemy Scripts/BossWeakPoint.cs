using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeakPoint : MonoBehaviour
{

    private int health = 100;
    private int maxHealth = 100;


    [SerializeField]
    private int hitsToDestroy;

    [SerializeField]
    private HealthBar healthBar;

    private GameObject bossObject;
    private GameObject bossMinionSpawner;

    // Start is called before the first frame update
    void Start()
    {
        this.bossObject = GameObject.FindGameObjectsWithTag("Boss")[0];
        this.bossMinionSpawner = GameObject.FindGameObjectsWithTag("BossSpawner")[0];

        healthBar.IsHealthTextActive(false);
        healthBar.SetMaxHealth((int)health);
    }

    private void TakeDamage(int amount)
    {
        health -= amount;
        healthBar.SetHealth(health);

        if (health <= 0)
        {
            this.bossMinionSpawner.GetComponent<BossMinionSpawner>().ActivateNextPhase();
            Destroy(gameObject);
        }
    }

    public void OnCollisionWithPlayer()
    {
        var bossController = this.bossObject.GetComponent<BossController>();
        var minnionController = this.bossMinionSpawner.GetComponent<BossMinionSpawner>();

        int weakPointDamageTake = maxHealth / hitsToDestroy;

        // give damage to the weak point itself
        this.TakeDamage(weakPointDamageTake);

        // give damage to the boss
        bossController.GiveDamage(minnionController.GetMaxWeakPointDamage() / hitsToDestroy);
    }


}
