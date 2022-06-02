using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private Player player;
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private int levelNr;
    int index = 0;
    private GameObject room;
    public List<Animation> doorAnimations;
    public List<Enemy> enemies;
    private SoulData SoulData;
    [SerializeField] GameObject soulPrefab;
    private GameObject soul;
    [SerializeField] NavMeshSurface surface;
    [SerializeField] GameObject winScreen;

    private void Awake()
    {
        Instance = this;
        SoulData soulData = SerializationManager.Load("souldata") as SoulData;

        if (soulData == null)
        {
            soulData = new SoulData(false, 0, 0, 0, null);
            SerializationManager.Save("souldata", soulData);
        }
        if (soulData.soulExists) SoulData = soulData;
    }

    private void Start()
    {
        enemies = new List<Enemy>();
        SetRoomData();
        ActionsIfCleared();
    }

    public void ActionsIfCleared()
    {
        if (enemies.Count == 0)
        {
            foreach (Animation anim in doorAnimations)
            {
                if(anim != null)
                {
                    anim.Play();
                }
            }
        }
    }

    void SetRoomData()
    {
        if(rooms.Length != 0)
        {
            room = Instantiate(rooms[index], Vector3.zero, Quaternion.identity, transform);
        }
   
        GameObject[] go = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject g in go)
        {
            doorAnimations.Add(g.GetComponent<Animation>());
        }
        go = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject g in go)
        {
            enemies.Add(g.GetComponent<Enemy>());
        }
        go = GameObject.FindGameObjectsWithTag("Boss");
        foreach (GameObject g in go)
        {
            enemies.Add(g.GetComponent<Enemy>());
        }

        if (SoulData != null && SoulData.soulExists && SoulData.level == levelNr && SoulData.stage == index)
        {
            Vector3 position = new Vector3(SoulData.position[0], 0.75f, SoulData.position[2]);
            soul = Instantiate(soulPrefab, position, Quaternion.identity);
            soul.GetComponent<Soul>().SetCoins(SoulData.coinsCollected);
        }
        surface.BuildNavMesh();
        ActionsIfCleared();
    }

    public void CreateSoulSave(bool soulExists, int coins, Vector3 soulPosition)
    {
        SoulData soulData = new SoulData(soulExists, coins, levelNr, index, new float[3] { soulPosition.x, soulPosition.y, soulPosition.z });
        SerializationManager.Save("souldata", soulData);
    }

    private void OnTriggerEnter(Collider other)
    {
        index++;
        if(index > rooms.Length-1)
        {
            GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("fireworks");
            Debug.Log("GG YOU WON");
            _StatsManager tempStatsManager = _StatsManager.Instance;
            tempStatsManager.coins += player.GetComponent<Player>().coins;
            tempStatsManager.CreateStatsSave();

            _SceneManager.Instance.UnlockNext();
            winScreen.SetActive(true);
            StartCoroutine(MoveToMain());
        }
        else
        {
            Destroy(room);
            Destroy(soul);
            SetRoomData();
            player.GetComponent<PlayerController3>().isDragging = false;
            player.transform.position = new Vector3(0, 0, -14);
        }
    }

    IEnumerator MoveToMain()
    {
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("MainMenu");
    }
}
