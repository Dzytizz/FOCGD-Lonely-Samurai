using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _SceneManager : MonoBehaviour
{
    public static _SceneManager Instance;

    [SerializeField] public LevelScenes[] levelScenes;
    
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void UnlockNext()
    {
        for (int i = 0; i < levelScenes.Length; i++)
        {
            if(levelScenes[i].sceneTitle.Equals(SceneManager.GetActiveScene().name))
            {
                if(i + 1 < levelScenes.Length)
                {
                    levelScenes[i + 1].isUnlocked = true;
                    break;
                }
                break;
            }
        }
    }
}

[System.Serializable]
public struct LevelScenes
{
    public bool isUnlocked; // TO BE CHANGED WHEN LOADING INFORMATION FROM OUTSIDE 
    public string sceneTitle;
}


