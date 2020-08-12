using UnityEngine;

public class PlayerAnimationsMethods : MonoBehaviour
{
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public Vector3 tpPosition;

    private void Start()
    {
        InteractionManager.instance.animMethodsScript = this;
    }

    void EndInteraction()
    {
        CharacterController2D.instance.animator.SetBool("Interacting", false);
    }

    void EndTeleporting()
    {
        CharacterController2D.instance.animator.SetBool("Teleporting", false);
    }

    void DoTeleportation()
    {
        CharacterController2D.instance.transform.position = tpPosition;
    }

    void EndShocked()
    {
        CharacterController2D.instance.animator.SetBool("Shocked", false);
    }
}
