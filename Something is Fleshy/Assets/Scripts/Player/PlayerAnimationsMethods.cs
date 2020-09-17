using UnityEngine;

public class PlayerAnimationsMethods : MonoBehaviour
{
#pragma warning disable 0649
    [Header("REFERENCES")]
    [SerializeField] AudioSource walkSource;
#pragma warning restore 0649
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public Vector3 tpPosition;
    bool securityJumpSFX;

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

    void WalkSound()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Walk, walkSource);
    }
}
