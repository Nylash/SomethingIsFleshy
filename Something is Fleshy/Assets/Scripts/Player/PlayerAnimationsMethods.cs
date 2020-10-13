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
    public AudioSource tpSource;

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
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.TeleportationOut, tpSource);
    }

    void EndShocked()
    {
        CharacterController2D.instance.animator.SetBool("Shocked", false);
    }

    void WalkSound()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Walk, walkSource);
    }

    void LandingSound()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Landing, CharacterController2D.instance.jumpLandingSource);
    }

    void ChocFace()
    {
        CharacterController2D.instance.animatorFace.SetTrigger("Choc");
    }

    void StressFace()
    {
        CharacterController2D.instance.animatorFace.SetTrigger("Stress");
    }

    void SighFace()
    {
        CharacterController2D.instance.animatorFace.SetTrigger("Sigh");
    }
}
