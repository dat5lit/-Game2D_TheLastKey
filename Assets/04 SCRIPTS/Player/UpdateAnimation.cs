using UnityEngine;

public class UpdateAnimation : MonoBehaviour
{
    private Animator _anim;

    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed"); // MỚI: Nhận vận tốc rơi
    private readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");       // MỚI: Nhận trạng thái chạm đất
    private readonly int JumpHash = Animator.StringToHash("Jump");
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int HitHash = Animator.StringToHash("Hit");
    private readonly int DeadHash = Animator.StringToHash("Dead");
    private readonly int ComboHash = Animator.StringToHash("Combo");
    private readonly int DashHash = Animator.StringToHash("Dash");

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        _anim.SetFloat(SpeedHash, speed);
    }

    // 🔥 CẬP NHẬT: Đồng bộ trạng thái chạm đất
    public void SetGrounded(bool isGrounded)
    {
        _anim.SetBool(IsGroundedHash, isGrounded);
    }

   

    public void Jump()
    {
        // Sử dụng Trigger cho hành động nhảy sẽ mượt mà hơn
        ResetJumpTrigger();
        _anim.SetTrigger(JumpHash);
    }

    public void ResetJumpTrigger()
    {
        _anim.ResetTrigger(JumpHash);
    }

    public void Attack()
    {
        _anim.ResetTrigger(AttackHash);
        _anim.SetTrigger(AttackHash);
    }

    public void SetCombo(int combo)
    {
        _anim.SetInteger(ComboHash, combo);
    }

    public void Dash()
    {
        _anim.SetTrigger(DashHash);
    }

    public void Hit()
    {
        _anim.SetTrigger(HitHash);
    }

    public void Dead()
    {
        _anim.SetBool(DeadHash, true);
    }
    public void UnDead()
    {
        _anim.SetBool(DeadHash, false);
    }
}