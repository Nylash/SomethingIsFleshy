using System.Collections.Generic;
using UnityEngine;

public class Leak : MonoBehaviour
{
    [Header("PARAMETERS")]
#pragma warning disable 0649
    [SerializeField] GameObject FXprefab;
    [SerializeField] GameObject bandagePrefab;
#pragma warning restore 0649
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public List<LeakZone> associatedPipeLeaksZones;
    public LeverScript associatedLever;
    public int associatedPipe;
    GameObject FX;
    ParticleSystem.MainModule fxMainModule;
    ParticleSystem.EmissionModule fxEmissionModule;

    private void Start()
    {
        FX = Instantiate(FXprefab, transform.position, FXprefab.transform.rotation);
        fxMainModule = FX.GetComponent<ParticleSystem>().main;
        fxEmissionModule = FX.GetComponent<ParticleSystem>().emission;
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
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
                        fxMainModule.startColor = GameManager.instance.energyColor;
                    break;
                case LeverScript.RessourcesType.oxygen:
                    LungsManager.instance.Emptying(Time.deltaTime);
                    if (fxMainModule.startColor.color != GameManager.instance.oxygenColor)
                        fxMainModule.startColor = GameManager.instance.oxygenColor;
                    break;
                case LeverScript.RessourcesType.none:
                    fxEmissionModule.enabled = false;
                    break;
            }
            if (!fxEmissionModule.enabled && associatedLever.currentRessource != LeverScript.RessourcesType.none)
                fxEmissionModule.enabled = true;
        }
        else
        {
            if (fxEmissionModule.enabled)
                fxEmissionModule.enabled = false;
        }
    }

    public void PatchLeak()
    {
        associatedLever.allLeaksZones[associatedPipe] = associatedPipeLeaksZones;
        Instantiate(bandagePrefab, transform.position, transform.rotation);
        Destroy(FX);
        LeaksManager.instance.allLeaks.Remove(gameObject);
        Destroy(gameObject);
    }
}
