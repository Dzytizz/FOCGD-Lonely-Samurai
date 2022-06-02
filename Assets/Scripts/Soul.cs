using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    [SerializeField] private int coins = 0;

    public void SetCoins(int coins)
    {
        this.coins = coins;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager levelManager = GameObject.FindWithTag("Level Manager").GetComponent<LevelManager>();
            levelManager.CreateSoulSave(false, coins, transform.position);
            other.GetComponent<Player>().AddCoins(coins);
            Destroy(gameObject);
        }
    }
}
