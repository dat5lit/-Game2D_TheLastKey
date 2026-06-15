using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MiniBossAnimator : MonoBehaviour
{
    private static readonly int HashIdle = Animator.StringToHash("Idle");
    private static readonly int HashWalk = Animator.StringToHash("Walk");
    private static readonly int HashAttack = Animator.StringToHash("Attack");
    private static readonly int HashHurt = Animator.StringToHash("Hurt");
    private static readonly int HashDie = Animator.StringToHash("Die");

    private Animator animator;
    private MiniBossController bossController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        bossController = GetComponentInParent<MiniBossController>();

        if (bossController == null)
        {
            Debug.LogError(
                "[MiniBossAnimator] Không tìm thấy MiniBossController ở object cha!"
            );
        }
    }

    public void OnStateChanged(MiniBossController.BossState newState)
    {
        Debug.Log("Animator State => " + newState);

        ResetAllTriggers();

        switch (newState)
        {
            case MiniBossController.BossState.Idle:
                animator.SetTrigger(HashIdle);
                break;

            case MiniBossController.BossState.Walk:
                animator.SetTrigger(HashWalk);
                break;

            case MiniBossController.BossState.Attack:
                animator.SetTrigger(HashAttack);
                break;

            case MiniBossController.BossState.Hurt:
                animator.SetTrigger(HashHurt);
                break;

            case MiniBossController.BossState.Die:
                animator.SetTrigger(HashDie);
                break;
        }
    }

    // Animation Event đặt tại frame gây damage
    public void AnimEvent_AttackHit()
    {
        Debug.Log("Attack Event Fired");
        bossController.OnAttackHit();
    }

    private void ResetAllTriggers()
    {
        animator.ResetTrigger(HashIdle);
        animator.ResetTrigger(HashWalk);
        animator.ResetTrigger(HashAttack);
        animator.ResetTrigger(HashHurt);
        animator.ResetTrigger(HashDie);
    }

    public bool IsPlayingState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0)
            .IsName(stateName);
    }
}