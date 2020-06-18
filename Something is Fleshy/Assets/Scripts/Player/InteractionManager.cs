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
            if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused)
            {
                if (canInteract)
                {
                    switch (currentInteractableType)
                    {
                        case InteractableType.lever:
                            interactableObject.GetComponent<LeverScript>().Switch();
                            break;
                        case InteractableType.button:
                            break;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Lever"))
        {
            canInteract = true;
            interactableObject = collision.gameObject;
            currentInteractableType = InteractableType.lever;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Lever"))
        {
            canInteract = false;
            interactableObject = null;
            currentInteractableType = InteractableType.none;
        }
    }

    public enum InteractableType
    {
        none, lever, button,
    }
}
