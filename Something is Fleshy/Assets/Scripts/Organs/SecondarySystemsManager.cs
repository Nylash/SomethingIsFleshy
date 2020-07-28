using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SecondarySystemsManager : MonoBehaviour
{
    public static SecondarySystemsManager instance;

    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("Time before the first activity.")]
    [SerializeField] float timeBeforeFirstActivity = 10f;
    [Tooltip("Minimal time between two activities.")]
    [SerializeField] float minTimeBetweenActivities = 15f;
    [Tooltip("Maximal time between two activities.")]
    [SerializeField] float maxTimeBetweenActivities = 30f;
    [Tooltip("Time during which player knows that a secondary system will need a ressource, but without HP loss.")]
    [SerializeField] float timeBeforeHealthLoss = 5f;
    [Range(0,50)] [SerializeField] int ressourceRandomEqualizerValue = 10;
    [SerializeField] List<SecondarySystem> packA = new List<SecondarySystem>();
    [SerializeField] List<SecondarySystem> packB = new List<SecondarySystem>();
    [SerializeField] List<SecondarySystem> packC = new List<SecondarySystem>();
    [SerializeField] List<SecondarySystem> packD = new List<SecondarySystem>();
#pragma warning restore 0649
    #endregion

    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public int randomRessourceEqualizer;
    public List<List<SecondarySystem>> allSecondarySystems = new List<List<SecondarySystem>>();
    bool startWhenOnePackIsReady;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);  

        if(packA.Count > 0)
            allSecondarySystems.Add(packA);
        if (packB.Count > 0)
            allSecondarySystems.Add(packB);
        if (packC.Count > 0)
            allSecondarySystems.Add(packC);
        if (packD.Count > 0)
            allSecondarySystems.Add(packD);

        Invoke("StartActivity", timeBeforeFirstActivity);
    }

    void StartActivity()
    {
        if (allSecondarySystems.Count > 0)
        {
            int selectedPack = Random.Range(0, allSecondarySystems.Count);
            int selectedSecondarySystem = Random.Range(0, allSecondarySystems[selectedPack].Count);
            allSecondarySystems[selectedPack][selectedSecondarySystem].animator.SetBool("OnActivity", true);
            switch (GetRandomType())
            {
                case LeverScript.RessourcesType.energy:
                    allSecondarySystems[selectedPack][selectedSecondarySystem].currentEnergy = 0f;
                    allSecondarySystems[selectedPack][selectedSecondarySystem].energyGauge.SetActive(true);
                    allSecondarySystems[selectedPack][selectedSecondarySystem].energyNeeded = true;
                    break;
                case LeverScript.RessourcesType.oxygen:
                    allSecondarySystems[selectedPack][selectedSecondarySystem].currentOxygen = 0f;
                    allSecondarySystems[selectedPack][selectedSecondarySystem].oxygenGauge.SetActive(true);
                    allSecondarySystems[selectedPack][selectedSecondarySystem].oxygenNeeded = true;
                    break;
            }
            StartCoroutine(LaunchActivity(allSecondarySystems[selectedPack][selectedSecondarySystem]));
            allSecondarySystems[selectedPack][selectedSecondarySystem].associatedPack = allSecondarySystems[selectedPack];
            allSecondarySystems.Remove(allSecondarySystems[selectedPack]);
        }
        else
            startWhenOnePackIsReady = true;
        Invoke("StartActivity", Random.Range(minTimeBetweenActivities, maxTimeBetweenActivities));
    }

    IEnumerator LaunchActivity(SecondarySystem selectedSystem)
    {
        yield return new WaitForSeconds(timeBeforeHealthLoss);
        selectedSystem.canDealDamage = true;
    }

    LeverScript.RessourcesType GetRandomType()
    {
        int random = Random.Range(0, 100);
        random += randomRessourceEqualizer;
        if (random < 50)
        {
            if (randomRessourceEqualizer < 0)
                randomRessourceEqualizer = ressourceRandomEqualizerValue;
            else
                randomRessourceEqualizer += ressourceRandomEqualizerValue;
            return LeverScript.RessourcesType.oxygen;
        }
        else
        {
            if (randomRessourceEqualizer > 0)
                randomRessourceEqualizer = -ressourceRandomEqualizerValue;
            else
                randomRessourceEqualizer -= ressourceRandomEqualizerValue;
            return LeverScript.RessourcesType.energy;
        }
    }

    public void AddPack(List<SecondarySystem> pack)
    {
        allSecondarySystems.Add(pack);
        if(allSecondarySystems.Count == 1 && startWhenOnePackIsReady)
        {
            startWhenOnePackIsReady = false;
            CancelInvoke("StartActivity");
            Invoke("StartActivity", 1);
        }
    }

    public void StopActivityCall()
    {
        CancelInvoke("StartActivity");
        StopCoroutine("LaunchActivity");
    }
}
