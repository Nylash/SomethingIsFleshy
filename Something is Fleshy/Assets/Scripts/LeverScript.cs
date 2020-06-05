using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    //Each lever is assiocated only to one system by script, either a primary or a secondary, in case of a secondary system, the ressource type must be specified
    //Assignation is handle in Primary/Secondary system script.
    public PrimarySystem associatedPrimarySystem;
    public SecondarySystem associatedSecondarySystem;
    public RessourcesType associatedRessource;
    //This bool is only used to animation
    public bool isOpen;

    public void Start()
    {
        if (associatedPrimarySystem && associatedSecondarySystem)
            Debug.LogError("A lever can't be associated to 2 systems at one time. " + gameObject.name);
    }

    public void Switch()
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
