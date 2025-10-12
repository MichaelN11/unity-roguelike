using UnityEngine;

/// <summary>
/// An AbilityEffect that causes the entity to move.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Movement")]
public class MovementEffect : AbilityEffect
{
    [SerializeField]
    private float moveSpeed = 0;
    public float MoveSpeed => moveSpeed;

    [SerializeField]
    private float moveAcceleration = 0;
    public float MoveAcceleration => moveAcceleration;

    [SerializeField]
    private float delayedAcceleration = 0;
    public float DelayedAcceleration => delayedAcceleration;

    [SerializeField]
    private float accelerationDelay = 0;
    public float AccelerationDelay => accelerationDelay;

    /// <summary>
    /// Scales the acceleration delay based on the active time of the effect.
    /// </summary>
    [SerializeField]
    private bool scaleToActiveTime = false;
    public bool ScaleToActiveTime => scaleToActiveTime;

    [SerializeField]
    private PrefabEffectData trailEffectData;
    public PrefabEffectData TrailEffectData => trailEffectData;

    [SerializeField]
    private float trailEffectDistance;
    public float TrailEffectDistance => trailEffectDistance;

    private float? _decelerationTime;
    private float DecelerationTime
    {
        get
        {
            if (!_decelerationTime.HasValue)
            {
                if (delayedAcceleration < 0)
                {
                    float fixedUpdatesPerSecond = 1 / Time.fixedDeltaTime;
                    float decelerationTimeInUpdates = (moveSpeed / delayedAcceleration) * -1;
                    _decelerationTime = decelerationTimeInUpdates / fixedUpdatesPerSecond;
                }
                else
                {
                    _decelerationTime = 0;
                }
            }
            return _decelerationTime.Value;
        }
    }

    public override void Trigger(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        if (abilityUseData.Movement != null)
        {
            abilityUseData.Movement.SetMovement(abilityUseData.Direction.normalized,
                moveSpeed,
                moveAcceleration);

            float delay = accelerationDelay;
            if (scaleToActiveTime && delayedAcceleration < 0)
            {
                delay = Mathf.Max(abilityUseData.ActiveTime - DecelerationTime, 0);
                Debug.Log($"Setting deceleration delay to {delay}");
            }
            if (delay > 0)
            {
                abilityUseData.Movement.SetDelayedAcceleration(delayedAcceleration, delay);
            }

            if (trailEffectData.Prefab != null)
            {
                Vector2 distance = -1 * TrailEffectDistance * abilityUseData.Direction.normalized;
                Vector3 position = abilityUseData.Position + distance;
                Quaternion rotation = (trailEffectData.RotatePrefab) ? UnityUtil.RotateTowardsVector(abilityUseData.Direction.normalized) : Quaternion.identity;
                GameObject instance = Instantiate(trailEffectData.Prefab, position, rotation);

                DestroyTimer destroyTimer = instance.GetComponent<DestroyTimer>();
                destroyTimer.Duration = trailEffectData.PrefabDuration;

                instance.transform.parent = abilityUseData.Entity.transform;
            }
        }
    }
}
