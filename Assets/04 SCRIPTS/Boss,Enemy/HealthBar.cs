using UnityEngine;
using UnityEngine.UI;

public class HealthBarWorld : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetMaxHealth(float maxHP)
    {
        slider.maxValue = maxHP;
        slider.value = maxHP;
    }

    public void SetHealth(float currentHP)
    {
        slider.value = currentHP;
    }
}