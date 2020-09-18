using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;

public class LeverScript : MonoBehaviour
{
    [System.Serializable]
    public struct Pipe
    {
        public GameObject pipe;
        public SpriteShapeRenderer shapeRenderer;
        public SpriteShapeController shapeController;
    }

    [Header("PARAMETERS")]
#pragma warning disable 0649
    [Tooltip("Pipes associated to this lever.")]
    [SerializeField] Pipe[] pipes = new Pipe[2];
    [Tooltip("Objects associated to this lever's pipes. Be sure to make the index coherent.")]
    [SerializeField] GameObject[] associatedObjects = new GameObject[2];
    [Tooltip("Check it if those pipes are at the start of the network. (do nothing on double entry)")]
    [SerializeField] bool initialPipes;
    [Tooltip("Check if the pipes are on double entry instead of classic double exit.")]
    [SerializeField] bool doubleEntry;
    [ConditionalHide("doubleEntry", true)]
    [Tooltip("Object at the end of the double pipes.")]
    [SerializeField] GameObject endObject;

    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    [SerializeField] List<LeakZone> pipe0LeaksZones = new List<LeakZone>();
    [SerializeField] List<LeakZone> pipe1LeaksZones = new List<LeakZone>();
#pragma warning restore 0649
    public RessourcesType currentRessource;
    public List<LeakZone>[] allLeaksZones = new List<LeakZone>[2];
    public int currentPipe;
    public LeverScript previousLever;
    RessourcesType lastFrameRessource;
    LeverScript currentAssociatedLever;
    PrimarySystem currentAssociatedPrimarySystem;
    SecondarySystem currentAssociatedSecondarySystem;
    Animator anim;

    //Lerp pipe height variable
    public bool isFillingSS;
    //Use when a double entry has a secondary system as end object
    bool checkNeedDoubleEntryDone;
    //Only used for animation purpose
    bool isSwitched;

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
        for (int i = 0; i < pipes.Length; i++)
        {
            pipes[i].shapeRenderer = pipes[i].pipe.GetComponent<SpriteShapeRenderer>();
            pipes[i].shapeController = pipes[i].pipe.GetComponent<SpriteShapeController>();
        }

        UpdatePipe();

        foreach (LeakZone item in pipes[0].pipe.GetComponentsInChildren<LeakZone>())
        {
            pipe0LeaksZones.Add(item);
        }
        foreach (LeakZone item in pipes[1].pipe.GetComponentsInChildren<LeakZone>())
        {
            pipe1LeaksZones.Add(item);
        }

        if (pipe0LeaksZones.Count != 0)
            allLeaksZones[0] = pipe0LeaksZones;
        if (pipe1LeaksZones.Count != 0)
            allLeaksZones[1] = pipe1LeaksZones;
        if (allLeaksZones[0] != null || allLeaksZones[1] != null)
            LeaksManager.instance.leversWithLeaksZones.Add(this);

        foreach (GameObject item in associatedObjects)
        {
            if (item.GetComponent<LeverScript>())
                item.GetComponent<LeverScript>().previousLever = this;
            if (item.GetComponent<SecondarySystem>())
                item.GetComponent<SecondarySystem>().associatedLever = this;
        }
        if (endObject)
        {
            if (endObject.GetComponent<LeverScript>())
                endObject.GetComponent<LeverScript>().previousLever = this;
            if (endObject.GetComponent<SecondarySystem>())
                endObject.GetComponent<SecondarySystem>().associatedLever = this;
        }
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                if (lastFrameRessource != currentRessource)
                    UpdatePipe();
                lastFrameRessource = currentRessource;
                if (currentAssociatedSecondarySystem)
                {
                    if (currentAssociatedSecondarySystem.oxygenNeeded && currentRessource == RessourcesType.oxygen && !currentAssociatedSecondarySystem.filling)
                        UpdatePipe();
                    if (currentAssociatedSecondarySystem.energyNeeded && currentRessource == RessourcesType.energy && !currentAssociatedSecondarySystem.filling)
                        UpdatePipe();
                }

                if (isFillingSS)
                {
                    if (pipes[currentPipe].shapeRenderer.color != GameManager.instance.emptyPipeOpenColor)
                        LerpPipeHeight();
                }

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
                {
                    currentAssociatedSecondarySystem.filling = true;
                    SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.StartFillingSecondarySystem, currentAssociatedSecondarySystem.audioSource);
                    IsSecondarySystemFilling(true);
                }
                else
                {
                    //WRONG RESSOURCE
                }
            }
            else if (currentAssociatedSecondarySystem.oxygenNeeded)
            {
                if (currentRessource == RessourcesType.oxygen)
                {
                    currentAssociatedSecondarySystem.filling = true;
                    SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.StartFillingSecondarySystem, currentAssociatedSecondarySystem.audioSource);
                    IsSecondarySystemFilling(true);
                }
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
                {
                    currentAssociatedSecondarySystem.filling = true;
                    SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.StartFillingSecondarySystem, currentAssociatedSecondarySystem.audioSource);
                    IsSecondarySystemFilling(true);
                }
                else
                {
                    currentAssociatedSecondarySystem.filling = false;
                    IsSecondarySystemFilling(false);
                    //WRONG RESSOURCE
                }
            }
            else if (currentAssociatedSecondarySystem.oxygenNeeded)
            {
                if (currentRessource == RessourcesType.oxygen)
                {
                    
                }
                else
                {
                    currentAssociatedSecondarySystem.filling = false;
                    IsSecondarySystemFilling(false);
                    //WRONG RESSOURCE
                }
            }
        }
        else
            Debug.LogError("There is no correct object associated at the end of this lever : " + gameObject.name + " current end object : " + endObject);

    }

    void UpdatePipesDisplay()
    {
        SecondarySystemsManager.instance.timerLerp = 0.5f;
        pipes[currentPipe].shapeRenderer.color = GetOpenColor(currentRessource);
        pipes[currentPipe].shapeRenderer.sortingOrder = 0;
        for (int i = 0; i < pipes[currentPipe].shapeController.spline.GetPointCount(); i++)
            pipes[currentPipe].shapeController.spline.SetHeight(i, 1);
        if (pipes[currentPipe].shapeRenderer.color == GameManager.instance.emptyPipeOpenColor)
            pipes[currentPipe].shapeController.spriteShape = GameManager.instance.pipeCloseShape;
        else
            pipes[currentPipe].shapeController.spriteShape = GameManager.instance.pipeOpenShape;
        switch (currentPipe)
        {
            case 0:
                pipes[1].shapeRenderer.color = GameManager.instance.pipeCloseColor;
                pipes[1].shapeRenderer.sortingOrder = -1;
                pipes[1].shapeController.spriteShape = GameManager.instance.pipeCloseShape;
                for (int i = 0; i < pipes[1].shapeController.spline.GetPointCount(); i++)
                    pipes[1].shapeController.spline.SetHeight(i, 1);
                break;
            case 1:
                pipes[0].shapeRenderer.color = GameManager.instance.pipeCloseColor;
                pipes[0].shapeRenderer.sortingOrder = -1;
                pipes[0].shapeController.spriteShape = GameManager.instance.pipeCloseShape;
                for (int i = 0; i < pipes[0].shapeController.spline.GetPointCount(); i++)
                    pipes[0].shapeController.spline.SetHeight(i, 1);
                break;
        }
    }

    void LerpPipeHeight()
    {
        for (int i = 0; i < pipes[currentPipe].shapeController.spline.GetPointCount(); i++)
            pipes[currentPipe].shapeController.spline.SetHeight(i, Mathf.Lerp(.9f, 1.1f, SecondarySystemsManager.instance.timerLerp));
    }

    public void IsSecondarySystemFilling(bool isFilling)
    {
        isFillingSS = isFilling;
        if (previousLever)
            previousLever.IsSecondarySystemFilling(isFilling);
        UpdatePipesDisplay();
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
            IsSecondarySystemFilling(false);
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
        isSwitched = !isSwitched;
        if (isSwitched)
            anim.SetTrigger("ToRight");
        else
            anim.SetTrigger("ToLeft");
    }

    Color GetOpenColor(RessourcesType currentRessource)
    {
        switch (currentRessource)
        {
            case RessourcesType.none:
                return GameManager.instance.emptyPipeOpenColor;
            case RessourcesType.energy:
                return GameManager.instance.energyColor;
            case RessourcesType.oxygen:
                return GameManager.instance.oxygenColor;
            default:
                return Color.red;
        }
    }

    public enum RessourcesType
    {
        none, energy, oxygen,
    }
}
