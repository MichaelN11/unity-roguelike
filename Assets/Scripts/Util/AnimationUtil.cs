using System;
/// <summary>
/// Utility class for animation methods.
/// </summary>
public class AnimationUtil
{
    /// <summary>
	/// Gets the animation speed required to run a 1 second animation for the full length
	/// of the passed animation time.
	/// </summary>
	/// <param name="animationTime">The time the animation is going to play for</param>
	/// <returns>The speed at which the animation will play for a full loop</returns>
    public static float GetAnimationSpeedFromTime(float animationTime)
    {
        float animationSpeed = -1;
        if (animationTime > 0)
        {
            animationSpeed = 1 / animationTime;
        }
        return animationSpeed;
    }
}
