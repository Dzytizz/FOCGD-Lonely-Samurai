using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGameManager : NetworkBehaviour
{
    private bool isGameStarted = false;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject waitingForPlayersText;
    [SyncVar(hook = nameof(HandleGameOver))] bool gameIsOver = false;
    [SerializeField] private GameObject deathPanel;
    private float spawnSpeed = 5.0f;
    private float changeRate = 5.0f;
    private float change = 0.5f;
    // Update is called once per frame

    private void Start()
    {
        if (!isServer) return;
        StartCoroutine(SpawnEnemies());
        StartCoroutine(IncreaseSpawnRate());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (isGameStarted && !gameIsOver)
            {
                float x = Random.Range(-8.0f, 8.0f);
                float z = Random.Range(-18.0f, 18.0f);
                GameObject enemy = Instantiate(enemyPrefab, new Vector3(x, 1, z), Quaternion.identity);
                NetworkServer.Spawn(enemy);
            }
            yield return new WaitForSeconds(spawnSpeed);
        }
    }

    IEnumerator IncreaseSpawnRate()
    {
        while (true)
        {
            if (isGameStarted && !gameIsOver)
            {
                spawnSpeed -= change;
                if(spawnSpeed <= 1)
                {
                    spawnSpeed = 1;
                }
            }
            yield return new WaitForSeconds(changeRate);
        }
    }

    [Command(requiresAuthority = false)]
    public void StartGame()
    {
        isGameStarted = true;
        DisableWaitingText();
    }

    [ClientRpc]
    private void DisableWaitingText()
    {
        waitingForPlayersText.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    public void SetGameIsOver()
    {
        gameIsOver = true;
    }

    public void HandleGameOver(bool oldValue, bool newValue)
    {
        deathPanel.SetActive(true);
    }
}
