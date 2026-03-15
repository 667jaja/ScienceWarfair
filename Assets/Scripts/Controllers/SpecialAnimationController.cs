using UnityEngine;

public class SpecialAnimationController : MonoBehaviour
{
    //animators
    public Animator animator;
    public Transform unitAnimation;
    public Transform laneAnimation;
    public string enemyBool;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SpecialAnimation(string AnimationPlayed, Vector2 unitPos, Vector2 LanePos, bool isEnemy, float destructionTimer)
    {
        unitAnimation.position = unitPos;
        laneAnimation.position = LanePos;

        animator.SetBool(enemyBool, isEnemy);
        animator.SetTrigger(AnimationPlayed);

        Destroy(this, destructionTimer*1.1f);
    }
}
