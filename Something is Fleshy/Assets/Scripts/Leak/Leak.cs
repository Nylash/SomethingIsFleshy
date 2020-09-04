using System.Collections.Generic;
using UnityEngine;

public class Leak : MonoBehaviour
{
    [Header("PARAMETERS")]
#pragma warning disable 0649
    [SerializeField] GameObject FXprefab;
    [SerializeField] GameObject bandagePrefab;
    [SerializeField] List<Sprite> crackSprites = new List<Sprite>();
#pragma warning restore 0649
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public List<LeakZone> associatedPipeLeaksZones;
    public LeverScript associatedLever;
    public GameObject associatedHint;
    public int associatedPipe;
    GameObject FX;
    ParticleSystem.MainModule fxMainModule;
    ParticleSystem.MainModule fxMainModuleChild;
    LeverScript.RessourcesType currentRessource;

    private void Start()
    {
        int random = Random.Range(0, crackSprites.Count);
        GetComponent<SpriteRenderer>().sprite = crackSprites[random];
        FX = Instantiate(FXprefab, transform.position, FXprefab.transform.rotation);
        fxMainModule = FX.GetComponent<ParticleSystem>().main;
        fxMainModuleChild = FX.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
        HintLeakManager.instance.activesLeaks.Add(this);
    }

    private void Update()
    {
        if (associatedLever.currentPipe == associatedPipe)
        {
            switch (associatedLever.currentRessource)
            {
                case LeverScript.RessourcesType.energy:
                    EmptyingStomach();
                    if (fxMainModule.startColor.color != GameManager.instance.energyColor)
                    {
                        fxMainModule.startColor = GameManager.instance.energyColor;
                        fxMainModuleChild.startColor = GameManager.instance.energyColor;
                        currentRessource = LeverScript.RessourcesType.energy;
                    }
                    break;
                case LeverScript.RessourcesType.oxygen:
                    EmptyingLungs();
                    if (fxMainModule.startColor.color != GameManager.instance.oxygenColor)
                    {
                        fxMainModule.startColor = GameManager.instance.oxygenColor;
                        fxMainModuleChild.startColor = GameManager.instance.oxygenColor;
                        currentRessource = LeverScript.RessourcesType.oxygen;
                    }
                    break;
                case LeverScript.RessourcesType.none:
                    switch (currentRessource)
                    {
                        case LeverScript.RessourcesType.none:
                            int rand = Random.Range(0, 2);
                            switch (rand)
                            {
                                case 0:
                                    EmptyingStomach();
                                    if (fxMainModule.startColor.color != GameManager.instance.energyColor)
                                    {
                                        fxMainModule.startColor = GameManager.instance.energyColor;
                                        fxMainModuleChild.startColor = GameManager.instance.energyColor;
                                        currentRessource = LeverScript.RessourcesType.energy;
                                    }
                                    break;
                                case 1:
                                    EmptyingLungs();
                                    if (fxMainModule.startColor.color != GameManager.instance.oxygenColor)
                                    {
                                        fxMainModule.startColor = GameManager.instance.oxygenColor;
                                        fxMainModuleChild.startColor = GameManager.instance.oxygenColor;
                                        currentRessource = LeverScript.RessourcesType.oxygen;
                                    }
                                    break;
                            }
                            break;
                        case LeverScript.RessourcesType.energy:
                            EmptyingStomach();
                            if (fxMainModule.startColor.color != GameManager.instance.energyColor)
                            {
                                fxMainModule.startColor = GameManager.instance.energyColor;
                                fxMainModuleChild.startColor = GameManager.instance.energyColor;
                                currentRessource = LeverScript.RessourcesType.energy;
                            }
                            break;
                        case LeverScript.RessourcesType.oxygen:
                            EmptyingLungs();
                            if (fxMainModule.startColor.color != GameManager.instance.oxygenColor)
                            {
                                fxMainModule.startColor = GameManager.instance.oxygenColor;
                                fxMainModuleChild.startColor = GameManager.instance.oxygenColor;
                                currentRessource = LeverScript.RessourcesType.oxygen;
                            }
                            break;
                    }
                    break;
            }
        }
        else
        {
            switch (currentRessource)
            {
                case LeverScript.RessourcesType.none:
                    int rand = Random.Range(0, 2);
                    switch (rand)
                    {
                        case 0:
                            EmptyingStomach();
                            if (fxMainModule.startColor.color != GameManager.instance.energyColor)
                            {
                                fxMainModule.startColor = GameManager.instance.energyColor;
                                fxMainModuleChild.startColor = GameManager.instance.energyColor;
                                currentRessource = LeverScript.RessourcesType.energy;
                            }
                            break;
                        case 1:
                            EmptyingLungs();
                            if (fxMainModule.startColor.color != GameManager.instance.oxygenColor)
                            {
                                fxMainModule.startColor = GameManager.instance.oxygenColor;
                                fxMainModuleChild.startColor = GameManager.instance.oxygenColor;
                                currentRessource = LeverScript.RessourcesType.oxygen;
                            }
                            break;
                    }
                    break;
                case LeverScript.RessourcesType.energy:
                    EmptyingStomach();
                    if (fxMainModule.startColor.color != GameManager.instance.energyColor)
                    {
                        fxMainModule.startColor = GameManager.instance.energyColor;
                        fxMainModuleChild.startColor = GameManager.instance.energyColor;
                        currentRessource = LeverScript.RessourcesType.energy;
                    }
                    break;
                case LeverScript.RessourcesType.oxygen:
                    EmptyingLungs();
                    if (fxMainModule.startColor.color != GameManager.instance.oxygenColor)
                    {
                        fxMainModule.startColor = GameManager.instance.oxygenColor;
                        fxMainModuleChild.startColor = GameManager.instance.oxygenColor;
                        currentRessource = LeverScript.RessourcesType.oxygen;
                    }
                    break;
            }
        }
    }

    void EmptyingLungs()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                LungsManager.instance.Emptying(Time.deltaTime);
            }
        }
    }

    void EmptyingStomach()
    {
        if (GameManager.instance.levelStarted)
        {
            if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
            {
                StomachManager.instance.Emptying(Time.deltaTime);
            }
        }
    }

    public void PatchLeak()
    {
        associatedLever.allLeaksZones[associatedPipe] = associatedPipeLeaksZones;
        Instantiate(bandagePrefab, transform.position, transform.rotation);
        Destroy(FX);
        LeaksManager.instance.allLeaks.Remove(gameObject);
        HintLeakManager.instance.activesLeaks.Remove(this);
        if (associatedHint)
            Destroy(associatedHint);
        Destroy(gameObject);
    }
}
