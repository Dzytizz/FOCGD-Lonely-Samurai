using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetCoolDownProgress(float progress)
    {
        if (progress > 1) progress = 1;
        slider.value = progress;
    }
}
