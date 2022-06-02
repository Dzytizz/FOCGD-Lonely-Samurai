using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _StatsManager : MonoBehaviour
{
    public static _StatsManager Instance;

    public int coins;

    [SerializeField] public int maxHealth; //300
    [SerializeField] public int damage; //10
    [SerializeField] public float ultimateDuration; //5f
    [SerializeField] public float ultimateCoolDown; //20f

    private void Awake()
    {
        Instance = this;
        PlayerData playerData = SerializationManager.Load("playerdata") as PlayerData;
        if(playerData == null)
        {
            playerData = new PlayerData(0, 300, 10, 5, 20);
            SerializationManager.Save("playerdata", playerData);
        }
        coins = playerData.coins;
        maxHealth = playerData.maxHealth;
        damage = playerData.damage;
        ultimateDuration = playerData.ultimateDuration;
        ultimateCoolDown = playerData.ultimateCoolDown;
    }

    public void CreateStatsSave()
    {
        PlayerData playerData = new PlayerData(coins, maxHealth, damage, ultimateDuration, ultimateCoolDown);
        SerializationManager.Save("playerdata", playerData);
    }

    public void DebugValuesToConsole()
    {
        Debug.Log("Max health: " + maxHealth);
        Debug.Log("Damage: " + damage);
        Debug.Log("Ultimate duration: " + ultimateDuration);
        Debug.Log("Ultimate cooldown: " + ultimateCoolDown);
    }
}
