using UnityEngine;
using System.Collections.Generic;

public class LeverScript : MonoBehaviour
{
    [Header("Parameters")]
    public bool doublePipeLever;
#pragma warning disable 0649
    [ConditionalHide("doublePipeLever", true)]
    [SerializeField] bool startOnRight;
#pragma warning restore 0649
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    //Each lever is assiocated only to one system by script, either a primary or a secondary, in case of a secondary system, the ressource type must be specified
    //Assignation is handle in Primary/Secondary system script.
    public PrimarySystem associatedPrimarySystem;
    public SecondarySystem associatedSecondarySystem;
    public RessourcesType associatedRessource;
    //Double pipe varaibles
    public List<PrimarySystem> doubleAssociatedPrimary = new List<PrimarySystem>();
    public List<SecondarySystem> doubleAssociatedSecondary = new List<SecondarySystem>();
    public RessourcesType[] doubleAssociatedRessources = new RessourcesType[2];
    //This bool is only used to animation
    public bool isOpen;

    public void Start()
    {
        if (doublePipeLever)
        {
            if (doubleAssociatedPrimary.Count != 2 && doubleAssociatedSecondary.Count != 2)
                Debug.LogError("This lever is declared as a double pipe lever, but it hasn't two systems associated." + gameObject.name);
            if (doubleAssociatedPrimary.Count > 0 && doubleAssociatedSecondary.Count > 0)
                Debug.LogError("This lever is declared as double pipe lever, but it is associated to a primary and a secondary system. Please don't mix systems type." + gameObject.name);
            if (startOnRight)
                AnimHandler();
        }
        else
        {
            if (associatedPrimarySystem && associatedSecondarySystem)
                Debug.LogError("A lever can't be associated to 2 systems at one time. " + gameObject.name);
        }
    }

    public void Switch()
    {
        if (doublePipeLever)
        {
            foreach (PrimarySystem item in doubleAssociatedPrimary)
            {
                item.filling = !item.filling;
                item.SwitchPipe();
            }
            for (int i = 0; i < 2; i++)
            {
                switch (doubleAssociatedRessources[i])
                {
                    case RessourcesType.energy:
                        doubleAssociatedSecondary[i].fillingEnergy = !doubleAssociatedSecondary[i].fillingEnergy;
                        doubleAssociatedSecondary[i].SwitchEnergyPipe();
                        break;
                    case RessourcesType.oxygen:
                        doubleAssociatedSecondary[i].fillingOxygen = !doubleAssociatedSecondary[i].fillingOxygen;
                        doubleAssociatedSecondary[i].SwitchOxygenPipe();
                        break;
                }
            }
            AnimHandler();
        }
        else
        {
            if (associatedPrimarySystem)
            {
                associatedPrimarySystem.filling = !associatedPrimarySystem.filling;
                associatedPrimarySystem.SwitchPipe();
                AnimHandler();
                return;
            }
            if (associatedSecondarySystem)
            {
                switch (associatedRessource)
                {
                    case RessourcesType.energy:
                        associatedSecondarySystem.fillingEnergy = !associatedSecondarySystem.fillingEnergy;
                        associatedSecondarySystem.SwitchEnergyPipe();
                        break;
                    case RessourcesType.oxygen:
                        associatedSecondarySystem.fillingOxygen = !associatedSecondarySystem.fillingOxygen;
                        associatedSecondarySystem.SwitchOxygenPipe();
                        break;
                }
                AnimHandler();
                return;
            }
            Debug.LogError("This lever is associated to no system. " + gameObject.name);
        }
    }

    void AnimHandler()
    {
        isOpen = !isOpen;
        if (isOpen)
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, -85);
        else
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public enum RessourcesType
    {
        none, energy, oxygen,
    }
}
