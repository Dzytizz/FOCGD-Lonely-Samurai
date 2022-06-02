using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMinionSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> phaseEnemyPrefans = new List<GameObject>();
    [SerializeField]
    private List<int> phaseEnemyCount = new List<int>();

    [SerializeField]
    private GameObject weakspotObject;

    [SerializeField]
    private int offsetX;

    [SerializeField]
    private int offsetY;

    [SerializeField]
    private GameObject weakPoint;

    private int currentPhase = 0;

    private List<GameObject> enemies = new List<GameObject>();

    int bossHealth;

    void Start()
    {
        spawnPhase();
    }


    private void spawnPhase()
    {
        var enemyObject = phaseEnemyPrefans[currentPhase];
        for (int i = 0; i < phaseEnemyCount[currentPhase]; i++)
        {
            var pos = new Vector3(Random.Range(-offsetX, offsetX), 1, Random.Range(-offsetY, offsetY));
            enemies.Add(Instantiate(enemyObject, pos + gameObject.transform.position, Quaternion.identity));
        }
    }


    private void updateEnemyList()
    {
        for(int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];
            // remove the enemy from the list if it has died
            if (enemy == null)
            {
                enemies.RemoveAt(i); 
            }
        }

        if (enemies.Count == 0 && GameObject.FindGameObjectsWithTag("BossWeakPoint").Length == 0)
        {
            spawnWeakPointIfPhaseIsOver();
        }
    }

    private void spawnWeakPointIfPhaseIsOver()
    {
        // new Vector3(-19.78, -13.72, 1.11)
        Instantiate(weakPoint, new Vector3(-19.78f, -13.72f, 1.11f), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        var boss = GameObject.FindGameObjectsWithTag("Boss");
        if (boss.Length == 0)
        {
            return;
        }
        this.bossHealth = GameObject.FindGameObjectsWithTag("Boss")[0].GetComponent<BossController>().GetHealth();
        updateEnemyList();
    }

    public void ActivateNextPhase()
    {
        currentPhase++;
        if (currentPhase > phaseEnemyCount.Count - 1)
        {
            GameObject.FindGameObjectsWithTag("Boss")[0].GetComponent<BossController>().GiveDamage(100000000);
            return;
        }
        spawnPhase();
    }

    public int GetMaxWeakPointDamage()
    {
        int leftPhases = phaseEnemyCount.Count - currentPhase;
        if (leftPhases == 0)
        {
            return 0;
        }
        return this.bossHealth / leftPhases;
    }
}
