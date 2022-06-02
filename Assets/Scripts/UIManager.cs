using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject deathScreen;

    public bool isPaused = false;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }

    private void Start()
    {
        coinText.text = "0";
    }


    public void ChangeCoinsAndText(int coins)
    {
        _StatsManager.Instance.coins = coins;
        coinText.text = coins.ToString();
    }

    public void PauseButton()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgainButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EnableDeathScreen()
    {
        deathScreen.SetActive(true);
    }
}
