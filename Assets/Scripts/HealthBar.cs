using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private Damageable playerDamageable;

    void Start()
    {
        slider = GetComponent<Slider>();
        playerDamageable = PlayerController.Instance.GetComponent<Damageable>();
    }

    void Update()
    {
        float healthPercentage = playerDamageable.CurrentHealth / playerDamageable.maxHealth;
        slider.value = healthPercentage;
    }
}
