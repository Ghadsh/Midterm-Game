using UnityEngine;
using UnityEngine.InputSystem;
using PlayerForShop;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private readonly int SpeedHash = Animator.StringToHash("Speed");

    public void SetSpeed(float speed)
    {
        animator.SetFloat(SpeedHash, speed, 0.1f, Time.deltaTime);
    }

    public void Trigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    public bool GetBool(string name)
    {
        return animator.GetBool(name);
    }
}
