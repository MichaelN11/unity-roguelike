using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private bool isPlayer;

    [SerializeField]
    private bool isBoss;

    [SerializeField]
    private TextMeshProUGUI labelText;

    private Slider slider;
    private Damageable damageable;

    private void OnEnable()
    {
        if (labelText != null)
        {
            labelText.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (damageable == null)
        {
            if (isPlayer && PlayerController.Instance != null)
            {
                damageable = PlayerController.Instance.GetComponent<Damageable>();
            } else if (isBoss && GameManager.Instance.CurrentBoss != null)
            {
                Debug.Log("Found boss: " + GameManager.Instance.CurrentBoss.Entity.Description.ToString());
                damageable = GameManager.Instance.CurrentBoss.GetComponent<Damageable>();
                labelText.text = GameManager.Instance.CurrentBoss.Entity.Description.ToString();
            } else if (isBoss)
            {
                Debug.Log("Hiding boss health bar");
                labelText.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
        else
        {
            float healthPercentage = damageable.CurrentHealth / damageable.MaxHealth;
            slider.value = healthPercentage;
        }
    }
}
