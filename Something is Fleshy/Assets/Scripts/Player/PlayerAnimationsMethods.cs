using UnityEngine;

public class PlayerAnimationsMethods : MonoBehaviour
{
    public void EndInteraction()
    {
        CharacterController2D.instance.animator.SetBool("Interacting", false);
    }
}
