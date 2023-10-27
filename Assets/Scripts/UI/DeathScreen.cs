using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField]
    private TextMeshProUGUI deathText;

    private bool foundPlayer = false;

    private void Update()
    {
        if (!foundPlayer && PlayerController.Instance != null)
        {
            EntityState playerState = PlayerController.Instance.GetComponent<EntityState>();
            if (playerState != null)
            {
                playerState.OnDeath += OnPlayerDeath;
            }
            foundPlayer = true;
        }
    }

    private void OnPlayerDeath(DeathContext deathContext)
    {
        Invoke(nameof(Display), delay);

        if (deathContext != null)
        {
            deathText.text = DetermineDeathText(deathContext);
        }
    }

    private void Display()
    {
        GameManager.Instance.EndGame();
        AudioManager.Instance.Play(deathSound);
        deathScreen.SetActive(true);
    }

    private string DetermineDeathText(DeathContext deathContext)
    {
        return "You were " + DeathActionText(deathContext.KillingAttack)
            + " by " + DeathSourceText(deathContext.KillingEntity) + ".";
    }

    private string DeathActionText(AttackDescription attackDescription)
    {
        string deathAction = "killed";
        switch (attackDescription)
        {
            case AttackDescription.Sword:
                deathAction = "sliced into ribbons";
                break;
            case AttackDescription.VineWhip:
                deathAction = "whipped to death";
                break;
            case AttackDescription.BodySlam:
                deathAction = "flattened";
                break;
            case AttackDescription.Arrow:
                deathAction = "shot with an arrow";
                break;
            case AttackDescription.AxeChop:
                deathAction = "cut in half";
                break;
            case AttackDescription.AxePoke:
                deathAction = "impaled";
                break;
            case AttackDescription.Charge:
                deathAction = "run over";
                break;
            case AttackDescription.Dash:
                deathAction = "run through";
                break;
        }
        return deathAction;
    }

    private string DeathSourceText(EntityDescription entityDescription)
    {
        string deathSource = "an evil creature";
        switch (entityDescription)
        {
            case EntityDescription.Player:
                deathSource = "a hero";
                break;
            case EntityDescription.Log:
                deathSource = "a walking tree";
                break;
            case EntityDescription.Twig:
                deathSource = "a forest spirit";
                break;
            case EntityDescription.Bandit:
                deathSource = "a bandit";
                break;
            case EntityDescription.Minotaur:
                deathSource = "a minotaur";
                break;
        }
        return deathSource;
    }
}
