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
    CharacterController2D player;

    private void Start()
    {
        GetComponentInParent<InteractionManager>().animMethodsScript = this;
        player = GetComponentInParent<CharacterController2D>();
    }

    void EndInteraction()
    {
        player.animator.SetBool("Interacting", false);
    }

    void EndTeleporting()
    {
        player.animator.SetBool("Teleporting", false);
    }

    void DoTeleportation()
    {
        player.transform.position = tpPosition;
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.TeleportationOut, tpSource);
    }

    void EndShocked()
    {
        player.animator.SetBool("Shocked", false);
    }

    void WalkSound()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Walk, walkSource);
    }

    void LandingSound()
    {
        SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Landing, player.jumpLandingSource);
    }

    void ChocFace()
    {
        player.animatorFace.SetTrigger("Choc");
    }

    void StressFace()
    {
        player.animatorFace.SetTrigger("Stress");
    }

    void SighFace()
    {
        player.animatorFace.SetTrigger("Sigh");
    }
}
