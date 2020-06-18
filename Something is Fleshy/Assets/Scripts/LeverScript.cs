using UnityEngine;
using UnityEngine.U2D;

public class LeverScript : MonoBehaviour
{
    [Header("PARAMETERS")]
#pragma warning disable 0649
    [Tooltip("Pipes associated to this lever.")]
    [SerializeField] GameObject[] pipes = new GameObject[2];
    [Tooltip("Objects associated to this lever's pipes. Be sure to make the index coherent.")]
    [SerializeField] GameObject[] associatedObjects = new GameObject[2];
    [Tooltip("Check it if those pipes are at the start of the network. (do nothing on double entry)")]
    [SerializeField] bool initialPipes;
    [Tooltip("Check if the pipes are on double entry instead of classic double exit.")]
    [SerializeField] bool doubleEntry;
    [ConditionalHide("doubleEntry", true)]
    [Tooltip("Object at the end of the double pipes.")]
    [SerializeField] GameObject endObject;
#pragma warning restore 0649
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public RessourcesType currentRessource;
    RessourcesType lastFrameRessource;
    LeverScript currentAssociatedLever;
    PrimarySystem currentAssociatedPrimarySystem;
    SecondarySystem currentAssociatedSecondarySystem;
    int currentPipe;
    //Use when a double entry has a secondary system as end object
    bool checkNeedDoubleEntryDone;
    //Only used for animation purpose
    bool isOpen;

    private void Start()
    {
        if (doubleEntry)
        {
            if (endObject.GetComponent<LeverScript>())
                currentAssociatedLever = endObject.GetComponent<LeverScript>();
            else if (endObject.GetComponent<PrimarySystem>())
            {
                if(endObject.GetComponent<LungsManager>())
                    currentAssociatedPrimarySystem = endObject.GetComponent<LungsManager>();
                else
                    currentAssociatedPrimarySystem = endObject.GetComponent<StomachManager>();
            } 
            else if (endObject.GetComponent<SecondarySystem>())
                currentAssociatedSecondarySystem = endObject.GetComponent<SecondarySystem>();
            else
                Debug.LogError("There is no correct object associated at the end of this lever : " + gameObject.name + " current end object : " + endObject);
        }
        UpdatePipe();
    }

    private void Update()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused)
            {
                if (lastFrameRessource != currentRessource)
                    UpdatePipe();
                lastFrameRessource = currentRessource;
                if (doubleEntry && currentAssociatedSecondarySystem)
                {
                    if ((currentAssociatedSecondarySystem.energyNeeded || currentAssociatedSecondarySystem.oxygenNeeded) && !currentAssociatedSecondarySystem.filling)
                    {
                        if (!checkNeedDoubleEntryDone)
                        {
                            UpdatePipe();
                            checkNeedDoubleEntryDone = true;
                        }
                    }
                    else
                    {
                        if (checkNeedDoubleEntryDone)
                            checkNeedDoubleEntryDone = false;
                    }
                }
            }
        }
    }

    void UpdatePipe()
    {
        if (doubleEntry)
        {
            UpdateDoubleEntry();
            return;
        }
        CleanPreviousAssociatedObjects();
        if (associatedObjects[currentPipe].GetComponent<LeverScript>())
        {
            currentAssociatedLever = associatedObjects[currentPipe].GetComponent<LeverScript>();
            currentAssociatedLever.currentRessource = currentRessource;
        }
        else if (associatedObjects[currentPipe].GetComponent<PrimarySystem>())
        {
            if (initialPipes)
            {
                if (associatedObjects[currentPipe].GetComponent<LungsManager>())
                    currentRessource = RessourcesType.oxygen;
                else
                    currentRessource = RessourcesType.energy;
                currentAssociatedPrimarySystem = associatedObjects[currentPipe].GetComponent<PrimarySystem>();
                currentAssociatedPrimarySystem.filling = true;
            }
            else
            {
                if (associatedObjects[currentPipe].GetComponent<LungsManager>())
                {
                    if(currentRessource == RessourcesType.oxygen)
                    {
                        currentAssociatedPrimarySystem = associatedObjects[currentPipe].GetComponent<PrimarySystem>();
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
                        currentAssociatedPrimarySystem = associatedObjects[currentPipe].GetComponent<PrimarySystem>();
                        currentAssociatedPrimarySystem.filling = true;
                    }
                    else
                    {
                        //WRONG RESSOURCE
                    }
                }
            }
        }
        else if (associatedObjects[currentPipe].GetComponent<SecondarySystem>())
        {
            currentAssociatedSecondarySystem = associatedObjects[currentPipe].GetComponent<SecondarySystem>();
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
            Debug.LogError("There is no correct object associated to this pipe : "+pipes[currentPipe] +" actual object : "+associatedObjects[currentPipe]);
        UpdatePipesDisplay();
        lastFrameRessource = currentRessource;
    }

    void UpdateDoubleEntry()
    {
        if (associatedObjects[currentPipe].GetComponent<LeverScript>())
        {
            currentRessource = associatedObjects[currentPipe].GetComponent<LeverScript>().currentRessource;
            UpdateEndObjectDoubleEntry();
        }
        else if (associatedObjects[currentPipe].GetComponent<PrimarySystem>())
        {
            if (associatedObjects[currentPipe].GetComponent<LungsManager>())
                currentRessource = RessourcesType.oxygen;
            else
                currentRessource = RessourcesType.energy;
            UpdateEndObjectDoubleEntry();
        }
        else if (associatedObjects[currentPipe].GetComponent<SecondarySystem>())
        {
            Debug.LogError("This case is not suppose to happen. Normally a pipe doesn't leave a secondary system. If you want to keep these please discuss about it with game designers :)");
        }
        else
            Debug.LogError("There is no correct object associated to this pipe : " + pipes[currentPipe] + " actual object : " + associatedObjects[currentPipe]);
        UpdatePipesDisplay();
        lastFrameRessource = currentRessource;
    }

    void UpdateEndObjectDoubleEntry()
    {
        if (currentAssociatedLever)
        {
            currentAssociatedLever.currentRessource = currentRessource;
        }
        else if (currentAssociatedPrimarySystem)
        {
            if (currentAssociatedPrimarySystem is LungsManager)
            {
                if (currentRessource == RessourcesType.oxygen)
                    currentAssociatedPrimarySystem.filling = true;
                else
                {
                    currentAssociatedPrimarySystem.filling = false;
                    //WRONG RESSOURCE
                }
            }
            else
            {
                if (currentRessource == RessourcesType.energy)
                    currentAssociatedPrimarySystem.filling = true;
                else
                {
                    currentAssociatedPrimarySystem.filling = false;
                    //WRONG RESSOURCE
                }
            }
        }
        else if (currentAssociatedSecondarySystem)
        {
            if (currentAssociatedSecondarySystem.energyNeeded)
            {
                if (currentRessource == RessourcesType.energy)
                    currentAssociatedSecondarySystem.filling = true;
                else
                {
                    currentAssociatedSecondarySystem.filling = false;
                    //WRONG RESSOURCE
                }
            }
            else if (currentAssociatedSecondarySystem.oxygenNeeded)
            {
                if (currentRessource == RessourcesType.oxygen)
                    currentAssociatedSecondarySystem.filling = true;
                else
                {
                    currentAssociatedSecondarySystem.filling = true;
                    //WRONG RESSOURCE
                }
            }
        }
        else
            Debug.LogError("There is no correct object associated at the end of this lever : " + gameObject.name + " current end object : " + endObject);

    }

    void UpdatePipesDisplay()
    {
        pipes[currentPipe].GetComponent<SpriteShapeRenderer>().color = GetOpenColor(currentRessource);
        pipes[currentPipe].GetComponent<SpriteShapeRenderer>().sortingOrder = 0;
        pipes[currentPipe].GetComponent<SpriteShapeController>().spriteShape = GameManager.instance.pipeOpenShape;
        switch (currentPipe)
        {
            case 0:
                pipes[1].GetComponent<SpriteShapeRenderer>().color = GameManager.instance.pipeCloseColor;
                pipes[1].GetComponent<SpriteShapeRenderer>().sortingOrder = -1;
                pipes[1].GetComponent<SpriteShapeController>().spriteShape = GameManager.instance.pipeCloseShape;
                break;
            case 1:
                pipes[0].GetComponent<SpriteShapeRenderer>().color = GameManager.instance.pipeCloseColor;
                pipes[0].GetComponent<SpriteShapeRenderer>().sortingOrder = -1;
                pipes[0].GetComponent<SpriteShapeController>().spriteShape = GameManager.instance.pipeCloseShape;
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
