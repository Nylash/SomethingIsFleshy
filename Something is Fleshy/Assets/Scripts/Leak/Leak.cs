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
    ParticleSystem.EmissionModule fxEmissionModule;
    ParticleSystem.MainModule fxMainModuleChild;
    ParticleSystem.EmissionModule fxEmissionModuleChild;

    private void Start()
    {
        int random = Random.Range(0, crackSprites.Count);
        GetComponent<SpriteRenderer>().sprite = crackSprites[random];
        FX = Instantiate(FXprefab, transform.position, FXprefab.transform.rotation);
        fxMainModule = FX.GetComponent<ParticleSystem>().main;
        fxEmissionModule = FX.GetComponent<ParticleSystem>().emission;
        fxMainModuleChild = FX.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        fxEmissionModuleChild = FX.transform.GetChild(0).GetComponent<ParticleSystem>().emission;
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
        HintLeakManager.instance.activesLeaks.Add(this);
    }

    private void Update()
    {
        if(associatedLever.currentPipe == associatedPipe)
        {
            switch (associatedLever.currentRessource)
            {
                case LeverScript.RessourcesType.energy:
                    StomachManager.instance.Emptying(Time.deltaTime);
                    if(fxMainModule.startColor.color != GameManager.instance.energyColor)
                    {
                        fxMainModule.startColor = GameManager.instance.energyColor;
                        fxMainModuleChild.startColor = GameManager.instance.energyColor;
                    }  
                    break;
                case LeverScript.RessourcesType.oxygen:
                    LungsManager.instance.Emptying(Time.deltaTime);
                    if (fxMainModule.startColor.color != GameManager.instance.oxygenColor)
                    {
                        fxMainModule.startColor = GameManager.instance.oxygenColor;
                        fxMainModuleChild.startColor = GameManager.instance.oxygenColor;
                    }
                    break;
                case LeverScript.RessourcesType.none:
                    fxEmissionModule.enabled = false;
                    fxEmissionModuleChild.enabled = false;
                    break;
            }
            if (!fxEmissionModule.enabled && associatedLever.currentRessource != LeverScript.RessourcesType.none)
            {
                fxEmissionModule.enabled = true;
                fxEmissionModuleChild.enabled = true;
            }    
        }
        else
        {
            if (fxEmissionModule.enabled)
            {
                fxEmissionModule.enabled = false;
                fxEmissionModuleChild.enabled = false;
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
