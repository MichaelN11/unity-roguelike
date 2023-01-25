using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Component used for using a combo attack.
/// </summary>
public class MeleeAttack : MonoBehaviour, IAttack
{
    [SerializeField]
    private List<AttackType> attackTypes;
    public AttackType AttackType => attackTypes[comboStage];

    public event Action<IAttack> OnAttackUsed; 

    [SerializeField]
    private string attackObjectResourceName = "AttackObject";

    private GameObject attackPrefab;
    private Vector2 direction;
    private float distance;
    private bool interrupted = false;
    private EntityType entityType;

    private int numComboStages;
    private int comboStage = 0;
    private float comboTimer = 0;

    private void Awake()
    {
        attackPrefab = (GameObject) Resources.Load(attackObjectResourceName);
        numComboStages = attackTypes.Count();
    }

    private void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    /// <summary>
    /// Use the attack. Waits for the startup time before starting the attack.
    /// </summary>
    /// <param name="direction">The direction of the attack</param>
    /// <param name="distance">The distance away the attack is used</param>
    /// <param name="entityType">The user's EntityType</param>
    public void Use(Vector2 direction, float distance, EntityType entityType)
    {
        interrupted = false;
        this.direction = direction;
        this.distance = distance;
        this.entityType = entityType;
        Invoke(nameof(StartAttack), AttackType.StartupTime);
    }

    /// <summary>
    /// Interrupts the attack, preventing it from starting if it hasn't already started.
    /// </summary>
    public void Interrupt()
    {
        interrupted = true;
        ResetCombo();
    }

    /// <summary>
    /// Resets the current combo stage.
    /// </summary>
    private void ResetCombo()
    {
        comboStage = 0;
        comboTimer = 0;
    }

    /// <summary>
    /// Starts the attack. Attacks in the passed direction, at the passed distance.
    /// An AttackDamage object is created with the AttackData set.
    /// </summary>
    private void StartAttack()
    {
        if (!interrupted)
        {
            OnAttackUsed?.Invoke(this);

            float angle = Vector2.SignedAngle(Vector2.right, direction) - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            distance += AttackType.Range;
            Vector3 position = transform.position + (Vector3)direction.normalized * distance;
            GameObject instance = Instantiate(attackPrefab, position, rotation);

            instance.transform.parent = gameObject.transform;

            DestroyTimer destroyTimer = instance.GetComponent<DestroyTimer>();
            destroyTimer.Duration = AttackType.HitboxDuration;

            AttackOnCollision attackObject = instance.GetComponent<AttackOnCollision>();
            AttackData attackData = new();
            attackData.AttackType = AttackType;
            attackData.User = UnityUtil.GetParentIfExists(gameObject);
            attackData.Direction = direction;
            attackData.SetDirectionOnHit = false;
            attackData.EntityType = entityType;
            attackObject.attackData = attackData;

            AudioManager.Instance.Play(AttackType.SoundOnUse);

            if (comboStage + 1 < numComboStages)
            {
                comboTimer = AttackType.ComboContinueWindow;
                ++comboStage;
            }
            else
            {
                ResetCombo();
            }
        }
    }
}
