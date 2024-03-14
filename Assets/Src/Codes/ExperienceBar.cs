using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMPro.TextMeshProUGUI levelText;

    private int killCount = 0;

    public void UpdateExperienceSlider(int current, int target)
    {
        slider.maxValue = target;
        slider.value = current;
    }

    public void IncreaseKillCount()
    {
        killCount++;
    }

    public void SetLevelText(int level)
    {
        levelText.text = "Lv: " + level.ToString();
    }
}
