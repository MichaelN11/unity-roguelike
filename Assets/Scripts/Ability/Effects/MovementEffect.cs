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

    [SerializeField]
    private PrefabEffectData trailEffectData;
    public PrefabEffectData TrailEffectData => trailEffectData;

    [SerializeField]
    private float trailEffectDistance;
    public float TrailEffectDistance => trailEffectDistance;

    public override void Trigger(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        if (abilityUseData.Movement != null)
        {
            abilityUseData.Movement.SetMovement(abilityUseData.Direction.normalized,
                moveSpeed,
                moveAcceleration);
            
            if (accelerationDelay > 0)
            {
                abilityUseData.Movement.SetDelayedAcceleration(delayedAcceleration, accelerationDelay);
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
