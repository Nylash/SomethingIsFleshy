using UnityEngine;
using UnityEngine.U2D;

public class LeverScript : MonoBehaviour
{
    [Header("PARAMETERS")]
#pragma warning disable 0649
    [Tooltip("Pipes associated to this lever.")]
    [SerializeField] GameObject[] pipes = new GameObject[2];
    [Tooltip("Levers associated to this lever's pipes. If one of the pipes is connected to a lever assign it here, on the good index (ex : if pipes[1] is connected to a lever, assign this lever to levers[1]).")]
    [SerializeField] GameObject[] levers = new GameObject[2];
    [Tooltip("Organs associated to this lever's pipes. Same details for the index as for the levers.")]
    [SerializeField] GameObject[] organs = new GameObject[2];
    [Tooltip("Check it if those pipes are at the start of the network.")]
    [SerializeField] bool initialPipes;
#pragma warning restore 0649
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public RessourcesType currentRessource;
    RessourcesType lastFrameRessource;
    LeverScript currentAssociatedLever;
    PrimarySystem currentAssociatedPrimarySystem;
    SecondarySystem currentAssociatedSecondarySystem;
    int currentPipe;
    //Only used for animation purpose
    bool isOpen;

    private void Start()
    {
        UpdatePipe();
    }

    private void Update()
    {
        if (lastFrameRessource != currentRessource)
            UpdatePipe();
        lastFrameRessource = currentRessource;
    }

    void UpdatePipe()
    {
        CleanPreviousAssociatedObjects();
        if (levers[currentPipe])
        {
            if (levers[currentPipe].GetComponent<LeverScript>())
            {
                currentAssociatedLever = levers[currentPipe].GetComponent<LeverScript>();
                currentAssociatedLever.currentRessource = currentRessource;
            }
            else
                Debug.LogError("The object associated to this pipe is not a lever (pipe : " + gameObject.name + " other lever : " + levers[currentPipe].name + ")");
        }
        else if (organs[currentPipe])
        {
            if (organs[currentPipe].GetComponent<PrimarySystem>())
            {
                if (initialPipes)
                {
                    if (organs[currentPipe].GetComponent<LungsManager>())
                        currentRessource = RessourcesType.oxygen;
                    else
                        currentRessource = RessourcesType.energy;
                    currentAssociatedPrimarySystem = organs[currentPipe].GetComponent<PrimarySystem>();
                    currentAssociatedPrimarySystem.filling = true;
                }
                else
                {
                    if (organs[currentPipe].GetComponent<LungsManager>())
                    {
                        if(currentRessource == RessourcesType.oxygen)
                        {
                            currentAssociatedPrimarySystem = organs[currentPipe].GetComponent<PrimarySystem>();
                            currentAssociatedPrimarySystem.filling = true;
                        }
                        else
                        {
                            //WRONG RESSOURCE
                        }
                    }
                    else
                    {
                        if (currentRessource == RessourcesType.energy)
                        {
                            currentAssociatedPrimarySystem = organs[currentPipe].GetComponent<PrimarySystem>();
                            currentAssociatedPrimarySystem.filling = true;
                        }
                        else
                        {
                            //WRONG RESSOURCE
                        }
                    }
                }
            }
            else if (organs[currentPipe].GetComponent<SecondarySystem>())
            {
                currentAssociatedSecondarySystem = organs[currentPipe].GetComponent<SecondarySystem>();
                if (currentAssociatedSecondarySystem.energyNeeded)
                {
                    if (currentRessource == RessourcesType.energy)
                        currentAssociatedSecondarySystem.filling = true;
                    else
                    {
                        //WRONG RESSOURCE
                    }
                }
                else if (currentAssociatedSecondarySystem.oxygenNeeded)
                {
                    if(currentRessource == RessourcesType.oxygen)
                        currentAssociatedSecondarySystem.filling = true;
                    else
                    {
                        //WRONG RESSOURCE
                    }
                }
            }
            else
                Debug.LogError("The object associated to this pipe is not an organ (pipe : " + gameObject.name + " organ : " + organs[currentPipe].name + ")");
        }
        else
            Debug.LogError("There is no object associated to this pipe " + pipes[currentPipe]);
        //COLORS
        pipes[currentPipe].GetComponent<SpriteShapeRenderer>().color = GetOpenColor(currentRessource);
        switch (currentPipe)
        {
            case 0:
                pipes[1].GetComponent<SpriteShapeRenderer>().color = GameManager.instance.pipeCloseColor;
                break;
            case 1:
                pipes[0].GetComponent<SpriteShapeRenderer>().color = GameManager.instance.pipeCloseColor;
                break;
        }
    }

    void CleanPreviousAssociatedObjects()
    {
        if (currentAssociatedLever)
        {
            currentAssociatedLever.currentRessource = RessourcesType.none;
            currentAssociatedLever = null;
        }
        if (currentAssociatedPrimarySystem)
        {
            currentAssociatedPrimarySystem.filling = false;
            currentAssociatedPrimarySystem = null;
        }
        if (currentAssociatedSecondarySystem)
        {
            currentAssociatedSecondarySystem.filling = false;
            currentAssociatedSecondarySystem = null;
        }
    }

    public void Switch()
    {
        switch (currentPipe)
        {
            case 0:
                currentPipe = 1;
                break;
            case 1:
                currentPipe = 0;
                break;
        }
        UpdatePipe();
        AnimHandler();
    }

    void AnimHandler()
    {
        isOpen = !isOpen;
        if (isOpen)
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, -85);
        else
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
    }

    Color GetOpenColor(RessourcesType currentRessource)
    {
        switch (currentRessource)
        {
            case RessourcesType.none:
                return GameManager.instance.emptyPipeOpenColor;
            case RessourcesType.energy:
                return GameManager.instance.energyPipeOpenColor;
            case RessourcesType.oxygen:
                return GameManager.instance.oxygenPipeOpenColor;
            default:
                return Color.red;
        }
    }

    public enum RessourcesType
    {
        none, energy, oxygen,
    }
}
