using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component managing the UI menu that appears after the player dies.
/// </summary>
public class DeathScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject deathScreen;

    [SerializeField]
    private float delay;

    [SerializeField]
    private Sound deathSound;

    private void Start()
    {
        if (PlayerController.Instance != null)
        {
            EntityState playerState = PlayerController.Instance.GetComponent<EntityState>();
            if (playerState != null)
            {
                playerState.OnDeath += OnPlayerDeath;
            }
        }
    }

    private void OnPlayerDeath()
    {
        Invoke(nameof(Display), delay);
    }

    private void Display()
    {
        AudioManager.Instance.Play(deathSound);
        deathScreen.SetActive(true);
    }
}
