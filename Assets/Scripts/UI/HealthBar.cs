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
    }

    void Update()
    {
        if (playerDamageable == null)
        {
            if (PlayerController.Instance != null)
            {
                playerDamageable = PlayerController.Instance.GetComponent<Damageable>();
            }
        }
        else
        {
            float healthPercentage = playerDamageable.CurrentHealth / playerDamageable.MaxHealth;
            slider.value = healthPercentage;
        }
    }
}
