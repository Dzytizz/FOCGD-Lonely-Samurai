using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button[] footerButtons;
    [SerializeField] private GameObject[] footerButtonContent;
    [SerializeField] private Button activeButton;
    [SerializeField] private ActiveColors activeColors;

    [SerializeField] private Animation[] levelSelectAnimations;
    public int levelIndex = 0;

    [SerializeField] private Upgrades[] upgrades;

    [SerializeField] private TextMeshProUGUI coinsText;

    private void Start()
    {
        Time.timeScale = 1;
        foreach(Button btn in footerButtons)
        {
            btn.onClick.AddListener(delegate { SetActiveButton(btn); });
        }
        SetColors();
        SetContent();
        SetUpgradeIndexes();
        ChangeUpgradeText();
        SetCoinsText();
    }

    private void SetUpgradeIndexes()
    {
        _StatsManager tempStats = _StatsManager.Instance;
        for (int i = 0; i < upgrades.Length; i++)
        {
            for (int j = 0; j < upgrades[i].values.Length; j++)
            {
                if(i == 0)
                {
                    if (tempStats.maxHealth == upgrades[i].values[j])
                    {
                        upgrades[i].index = j;
                        break;
                    }
                }
                else if (i == 1)
                {
                    if (tempStats.damage == upgrades[i].values[j])
                    {
                        upgrades[i].index = j;
                        break;
                    }
                }
                else if(i == 2)
                {
                    if (tempStats.ultimateCoolDown == upgrades[i].values[j])
                    {
                        upgrades[i].index = j;
                        break;
                    }
                }
                else if(i == 3)
                {
                    if (tempStats.ultimateDuration == upgrades[i].values[j])
                    {
                        upgrades[i].index = j;
                        break;
                    }
                }
                else
                {
                    Debug.Log("Current upgrade not implemented!");
                    break;
                }
            }
        }
    }

    private void SetActiveButton(Button btn)
    {
        activeButton = btn;
        SetColors();
        SetContent();
    }

    private void SetCoinsText()
    {
        coinsText.text = _StatsManager.Instance.coins.ToString();
    }

    private void SetContent()
    {
        for (int i = 0; i < footerButtons.Length; i++)
        {
            if (footerButtons[i] == activeButton)
            {
                footerButtonContent[i].SetActive(true);
            }
            else
            {
                footerButtonContent[i].SetActive(false);
            }
        }     
    }

    private void SetColors()
    {
        foreach (Button btn in footerButtons)
        {
            if (btn == activeButton)
            {
                btn.image.color = activeColors.inactive;
            }
            else
            {
                btn.image.color = activeColors.active;
            }
        }
    }

    public void MoveLevelLeft()
    {
        if(levelIndex == 0)
        {
            Debug.Log("Can't move left!");
        }
        else
        {
            levelSelectAnimations[levelIndex].Play("OutRight");
            levelIndex--;
            levelSelectAnimations[levelIndex].Play("InLeft");
        }
    }

    public void MoveLevelRight()
    {
        if (levelIndex == levelSelectAnimations.Length-1)
        {
            Debug.Log("Can't move right!");
        }
        else
        {
            levelSelectAnimations[levelIndex].Play("OutLeft");
            levelIndex++;
            levelSelectAnimations[levelIndex].Play("InRight");
        }
    }

    public void StartLevel()
    {
        LevelScenes[] levelScenes = _SceneManager.Instance.levelScenes;
        if(levelIndex < levelScenes.Length)
        {
            if (levelScenes[levelIndex].isUnlocked)
            {
                SceneManager.LoadScene(levelScenes[levelIndex].sceneTitle);
            }
            else
            {
                Debug.Log("Level is locked!");
            }
        }
        else
        {
            Debug.Log("Level is not created yet!");
        }
    }

    public void BuyButton(int index)
    {
        _StatsManager tempStats = _StatsManager.Instance;
        if (upgrades.Length <= index || upgrades[index].prices.Length <= upgrades[index].index)
        {
            return;
        }

        Upgrades currUp = upgrades[index];
        if(tempStats.coins >= currUp.prices[currUp.index])
        {
            tempStats.coins -= (int)currUp.prices[currUp.index];
            currUp.index++;
            switch (index)
            {
                case 0:
                    tempStats.maxHealth = (int)currUp.values[currUp.index];
                    break;
                case 1:
                    tempStats.damage = (int)currUp.values[currUp.index];
                    break;
                case 2:
                    tempStats.ultimateCoolDown = currUp.values[currUp.index];
                    break;
                case 3:
                    tempStats.ultimateDuration = currUp.values[currUp.index];
                    break;
                default:
                    Debug.Log("Current upgrade not implemented!");
                    break;
            }
            tempStats.CreateStatsSave();

            upgrades[index].index++;
            ChangeUpgradeText();
            SetCoinsText();
        }
        else
        {
            Debug.Log("Not enough coins!");
        }

    }

    public void ChangeUpgradeText()
    {
        foreach(Upgrades up in upgrades)
        {
            if (up.prices.Length <= up.index)
            {
                up.priceText.text = "max";
                continue;
            }
            up.priceText.text = up.prices[up.index].ToString();

            StringBuilder changeTxt = new StringBuilder();
            changeTxt.Append(up.values[up.index]);
            changeTxt.Append(" > ");
            changeTxt.Append(up.values[up.index+1]);
            up.changeText.text = changeTxt.ToString();
        }
    }
}

[System.Serializable]
public struct Upgrades
{
    public int index;
    public float[] values;
    public float[] prices;
    public Text priceText;
    public Text changeText;
}

[System.Serializable]
public struct ActiveColors
{
    public Color inactive;
    public Color active;
}

