using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int damage;
    public bool isDashing = false;

    public float ultimateDuration;
    public float ultimateCoolDown;
    public bool isUltimateReady = false;
    private float ultimateCoolDownTimer;
    public bool inUltimate = false;

    public int maxHealth;
    public int currentHealth;

    public int coins = 0;

    [SerializeField] private HealthBar healthBar;
    [SerializeField] private CoolDownBar ultimateCoolDownBar;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private AudioClip sliceSound, bloodSound;

    private void Start()
    {
        _StatsManager tempStatsManager = _StatsManager.Instance;
        damage = tempStatsManager.damage;
        maxHealth = tempStatsManager.maxHealth;
        ultimateDuration = tempStatsManager.ultimateDuration;
        ultimateCoolDown = tempStatsManager.ultimateCoolDown;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        ultimateCoolDownTimer = ultimateCoolDown;
    }

    private void Update()
    {
        //UpdateDashCoolDown();
        UpdateUltimateCoolDown();

        if (gameObject.GetComponent<TrailRenderer>() != null)
        {
            gameObject.GetComponent<TrailRenderer>().time = isDashing ? 5 : 0;
        }
    }

    //public void PutDashOnCooldown()
    //{
    //    isDashReady = false;
    //    dashCoolDownTimer = dashCoolDown;
    //}

    //public void UpdateDashCoolDown()
    //{
    //    if (!isDashReady)
    //    {
    //        dashCoolDownTimer -= Time.deltaTime;
    //        if (dashCoolDownTimer < 0.0f)
    //        {
    //            isDashReady = true;
    //            dashCoolDownBar.SetCoolDownProgress(1f);
    //        }
    //        else
    //        {
    //            float progress = (dashCoolDown - dashCoolDownTimer) / dashCoolDown;
    //            dashCoolDownBar.SetCoolDownProgress(progress);
    //        }
    //    }
    //}

    //public void ResetDashCoolDown()
    //{
    //    isDashReady = true;
    //    dashCoolDownBar.SetCoolDownProgress(1);
    //}

    public void PutUltimateOnCoolDown()
    {
        isUltimateReady = false;
        ultimateCoolDownTimer = ultimateCoolDown;
        ultimateCoolDownBar.SetCoolDownProgress(0);
    }

    public void UpdateUltimateCoolDown()
    {
        if (!isUltimateReady && !inUltimate)
        {
            ultimateCoolDownTimer -= Time.deltaTime;
            if (ultimateCoolDownTimer < 0.0f)
            {
                isUltimateReady = true;
                ultimateCoolDownBar.SetCoolDownProgress(1f);
            }
            else
            {
                float progress = (ultimateCoolDown - ultimateCoolDownTimer) / ultimateCoolDown;
                ultimateCoolDownBar.SetCoolDownProgress(progress);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        float precDownHealth = (float)currentHealth / (float)maxHealth; 
        
        GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("hurt", 1.0f*precDownHealth);
        StartCoroutine(GameObject.Find("Main Camera")?.GetComponent<CameraShake>()?.Shake(0.2f, 0.5f));
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        LevelManager levelManager = GameObject.FindWithTag("Level Manager").GetComponent<LevelManager>();
        levelManager.CreateSoulSave(true, coins, transform.position);
        UIManager uiManager = UIManager.Instance;
        uiManager.EnableDeathScreen();
        gameObject.GetComponent<PlayerController3>().enabled = false;
    }

    public void AddCoins(int coins)
    {
        this.coins += coins;
        if (coinsText != null)
        {
            coinsText.text = this.coins.ToString();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && isDashing)
        {
            GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("slice");
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.WillDieFromDamage(damage)){
                AddCoins(enemy.GetCoins());
            }
            enemy.TakeDamage(damage);
        }

        if (other.CompareTag("BossWeakPoint"))
        {
            GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("slice");
            other.GetComponent<BossWeakPoint>().OnCollisionWithPlayer();
        }
    }
}
