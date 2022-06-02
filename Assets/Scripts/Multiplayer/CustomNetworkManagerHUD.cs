using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManagerHUD : MonoBehaviour
{
    private NetworkManager manager;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject pausePanel;
    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    public void StartHost()
    {
        if (!NetworkClient.active)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                manager.StartHost();
                startPanel.SetActive(false);
            }
        }
    }

    public void StartClient()
    {
        if (!NetworkClient.active)
        {
            manager.StartClient();
            startPanel.SetActive(false);
        }
    }

    public void Exit()
    {
        Destroy(manager.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
    }

    public void Continue()
    {
        pausePanel.SetActive(false);
    }

    public void StopAndExit()
    {
        Stop();
        Exit();
    }

    public void Stop()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            manager.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            manager.StopClient();
        }
    }

    public void Restart()
    {
        Stop();
        Destroy(manager.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
