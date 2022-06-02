using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coins;
    public int maxHealth;
    public int damage;
    public float ultimateDuration;
    public float ultimateCoolDown;

    public PlayerData(int coins, int maxHealth, int damage, float ultimateDuration, float ultimateCoolDown)
    {
        this.coins = coins;
        this.maxHealth = maxHealth;
        this.damage = damage;
        this.ultimateDuration = ultimateDuration;
        this.ultimateCoolDown = ultimateCoolDown;
    }
}
