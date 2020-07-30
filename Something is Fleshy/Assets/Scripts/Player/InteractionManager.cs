using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    [Header("Components")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    ActionsMap actionsMap;

    [Header("Variables")]
    bool canInteract;
    GameObject interactableObject;
    InteractableType currentInteractableType;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        actionsMap = new ActionsMap();

        actionsMap.Gameplay.Interact.started += ctx => Interaction();
    }

    void Interaction()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused && CharacterController2D.instance.AnimationNotCurrentlyBlocking())
            {
                if (canInteract)
                {
                    if (CharacterController2D.instance.AnimationNotCurrentlyBlocking())
                    {
                        CharacterController2D.instance.animator.SetTrigger("StartInteracting");
                        CharacterController2D.instance.animator.SetBool("Interacting", true);
                    }
                    CharacterController2D.instance.rb.velocity = Vector2.zero;
                    switch (currentInteractableType)
                    {
                        case InteractableType.lever:
                            interactableObject.GetComponent<LeverScript>().Switch();
                            break;
                        case InteractableType.electricLever:
                            interactableObject.GetComponent<ElectricSwitch>().Switch();
                            break;
                        case InteractableType.leak:
                            interactableObject.GetComponent<Leak>().PatchLeak();
                            break;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Lever":
                canInteract = true;
                interactableObject = collision.gameObject;
                currentInteractableType = InteractableType.lever;
                break;
            case "ElectricLever":
                canInteract = true;
                interactableObject = collision.gameObject;
                currentInteractableType = InteractableType.electricLever;
                break;
            case "Leak":
                canInteract = true;
                interactableObject = collision.gameObject;
                currentInteractableType = InteractableType.leak;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Lever":
                if (currentInteractableType == InteractableType.lever)
                    goto case "CLEAN CASE";
                else
                    break;
            case "ElectricLever":
                if (currentInteractableType == InteractableType.electricLever)
                    goto case "CLEAN CASE";
                else
                    break;
            case "Leak":
                if (currentInteractableType == InteractableType.leak)
                    goto case "CLEAN CASE";
                else
                    break;
            case "CLEAN CASE":
                canInteract = false;
                interactableObject = null;
                currentInteractableType = InteractableType.none;
                break;
        }
    }

    public enum InteractableType
    {
        none, lever, electricLever, leak,
    }
}
