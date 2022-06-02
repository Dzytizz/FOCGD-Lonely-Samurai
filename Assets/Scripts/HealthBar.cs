using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;
    [SerializeField] private GameObject damagePopUpPrefab;


    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        healthText.text = health.ToString();
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        ShowDamagePopUp((int)slider.value - health);
        if (health < 0) health = 0;
        slider.value = health;
        healthText.text = health.ToString();
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void IsHealthTextActive(bool isActive)
    {
        healthText.enabled = isActive;
    }

    private void ShowDamagePopUp(int damage)
    {
        GameObject damagePopUp = Instantiate(damagePopUpPrefab, gameObject.transform);
        damagePopUp.transform.localPosition = new Vector3(0, 27.5f, 0);
        damagePopUp.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(damage.ToString());
    }
}
