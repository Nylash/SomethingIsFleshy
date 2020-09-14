using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SecondarySystemsManager : MonoBehaviour
{
    public static SecondarySystemsManager instance;

    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("Time to full a secondary system in energy.")]
    public float energyAmoutNeeded = 10f;
    [Tooltip("Time to full a secondary system in oxygen.")]
    public float oxygenAmoutNeeded = 10f;
    [Tooltip("Time before an secondary system expires.")]
    public float timeBeforeExpirationSecondarySystem = 5f;
    [Space]
    [Tooltip("Maximum of secondary systems that can be active simultaneously.")]
    [SerializeField] int maxSimultaneousSecondarySystems;
    [Tooltip("Thereshold of simultaneous seconcady systems when only timer can activate more.")]
    public int criticalSimultaneousSecondarySystems;
    [Space]
    [Tooltip("Time before the first activity.")]
    public float timeBeforeFirstActivity = 10f;
    [Tooltip("Minimal time between two activities.")]
    [SerializeField] float minTimeBetweenActivities = 15f;
    [Tooltip("Maximal time between two activities.")]
    [SerializeField] float maxTimeBetweenActivities = 30f;
    [Tooltip("Value that increase probability of the other ressource when one is chosen." +
        "By default prob are 50-50, but when one is chosen it became 60-40 (if this value is 10).")]
    [Range(0,50)] [SerializeField] int ressourceRandomWeightValue = 10;
    [Space]
    [SerializeField] Pack packA;
    [SerializeField] Pack packB;
    [SerializeField] Pack packC;
    [SerializeField] Pack packD;
    #pragma warning restore 0649
    #endregion

    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    //If this value if positive it increase energy prob, if it is negative it increase oxygen prob
    public int randomRessourceWeight;
    public List<Pack> allSecondarySystems = new List<Pack>();
    public int activesSecondarySystems;
    public Pack lastPack;
    //Lerp pipes height variables
    public float timerLerp;
    bool isIncreasingHeight;

    [System.Serializable]
    public class Pack
    {
        public List<SecondarySystem> secondarySystems = new List<SecondarySystem>();
        [HideInInspector]
        public SecondarySystem currentSecondarySystem;
        [HideInInspector]
        public int drawIndex;
        [HideInInspector]
        public bool packWithOneSecondarySystem;

        public SecondarySystem SelectSecondarySystem()
        {
            drawIndex++;
            currentSecondarySystem = secondarySystems[Random.Range(0, secondarySystems.Count)];
            secondarySystems.Remove(currentSecondarySystem);
            currentSecondarySystem.drawIndex = drawIndex;
            if (packWithOneSecondarySystem)
                drawIndex++;
            return currentSecondarySystem;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);  

        if(packA.secondarySystems.Count > 0)
        {
            allSecondarySystems.Add(packA);
            if (packA.secondarySystems.Count == 1)
                packA.packWithOneSecondarySystem = true;
        }
        if (packB.secondarySystems.Count > 0)
        {
            allSecondarySystems.Add(packB);
            if (packB.secondarySystems.Count == 1)
                packB.packWithOneSecondarySystem = true;
        }
        if (packC.secondarySystems.Count > 0)
        {
            allSecondarySystems.Add(packC);
            if (packC.secondarySystems.Count == 1)
                packC.packWithOneSecondarySystem = true;
        }
        if (packD.secondarySystems.Count > 0)
        {
            allSecondarySystems.Add(packD);
            if (packD.secondarySystems.Count == 1)
                packD.packWithOneSecondarySystem = true;
        }

        if (allSecondarySystems.Count == 0)
            Debug.LogError("You need to assign some Secondary Systems to atleast one pack (GameManager).");
    }

    public void StartGame()
    {
        Invoke("StartActivityByTimer", timeBeforeFirstActivity);
    }

    private void Update()
    {
        if (isIncreasingHeight)
        {
            timerLerp += Time.deltaTime;
            if (timerLerp > 1f)
                isIncreasingHeight = false;
        }
        else
        {
            timerLerp -= Time.deltaTime;
            if (timerLerp < 0f)
                isIncreasingHeight = true;
        }
    }

    void StartActivityByTimer()
    {
        if (activesSecondarySystems < maxSimultaneousSecondarySystems)
            StartActivity();
        Invoke("StartActivityByTimer", Random.Range(minTimeBetweenActivities, maxTimeBetweenActivities));
    }

    public void StartActivityByEnd()
    {
        if (activesSecondarySystems < criticalSimultaneousSecondarySystems)
            StartActivity();
    }

    void StartActivity()
    {
        if (allSecondarySystems.Count > 0)
        {
            int selectedPack = Random.Range(0, allSecondarySystems.Count);
            if (lastPack != null)
            {
                if (lastPack.secondarySystems.Count > 0)
                    allSecondarySystems.Add(lastPack);
            }
            SecondarySystem selectedSecondarySystem = allSecondarySystems[selectedPack].SelectSecondarySystem();
            selectedSecondarySystem.associatedPack = allSecondarySystems[selectedPack];
            lastPack = allSecondarySystems[selectedPack];
            allSecondarySystems.RemoveAt(selectedPack);
            selectedSecondarySystem.animator.SetBool("OnActivity", true);
            if (selectedSecondarySystem.memberAnimator)
                selectedSecondarySystem.memberAnimator.speed = 1f;
            switch (GetRandomType())
            {
                case LeverScript.RessourcesType.energy:
                    selectedSecondarySystem.currentEnergy = 0f;
                    selectedSecondarySystem.energyGauge.SetActive(true);
                    selectedSecondarySystem.energyNeeded = true;
                    break;
                case LeverScript.RessourcesType.oxygen:
                    selectedSecondarySystem.currentOxygen = 0f;
                    selectedSecondarySystem.oxygenGauge.SetActive(true);
                    selectedSecondarySystem.oxygenNeeded = true;
                    break;
            }
            activesSecondarySystems++;
            HintSecondarySystemManager.instance.activeSecondarySystems.Add(selectedSecondarySystem);
            TimerSecondarySystem timerObject = Instantiate(GameManager.instance.UI_timerSS, UI_Manager.instance.transform).GetComponent<TimerSecondarySystem>();
            timerObject.associatedSystem = selectedSecondarySystem;
        }
        else
            print("bouya");
    }

    LeverScript.RessourcesType GetRandomType()
    {
        int random = Random.Range(0, 100);
        random += randomRessourceWeight;
        if (random < 50)
        {
            if (randomRessourceWeight < 0)
                randomRessourceWeight = ressourceRandomWeightValue;
            else
                randomRessourceWeight += ressourceRandomWeightValue;
            return LeverScript.RessourcesType.oxygen;
        }
        else
        {
            if (randomRessourceWeight > 0)
                randomRessourceWeight = -ressourceRandomWeightValue;
            else
                randomRessourceWeight -= ressourceRandomWeightValue;
            return LeverScript.RessourcesType.energy;
        }
    }

    public void StopActivityCall()
    {
        CancelInvoke("StartActivityByTimer");
    }

    public void LaunchSpecificSS(SecondarySystem specificSecondarySystem, Pack associatedPack)
    {
        if (lastPack != null)
        {
            if (lastPack.secondarySystems.Count > 0)
                allSecondarySystems.Add(lastPack);
        }
        specificSecondarySystem.associatedPack = associatedPack;
        lastPack = associatedPack;
        allSecondarySystems.Remove(associatedPack);
        specificSecondarySystem.animator.SetBool("OnActivity", true);
        if (specificSecondarySystem.memberAnimator)
            specificSecondarySystem.memberAnimator.speed = 1f;
        specificSecondarySystem.currentEnergy = 0f;
        specificSecondarySystem.energyGauge.SetActive(true);
        specificSecondarySystem.energyNeeded = true;
        activesSecondarySystems++;
        HintSecondarySystemManager.instance.activeSecondarySystems.Add(specificSecondarySystem);
        TimerSecondarySystem timerObject = Instantiate(GameManager.instance.UI_timerSS, UI_Manager.instance.transform).GetComponent<TimerSecondarySystem>();
        timerObject.associatedSystem = specificSecondarySystem;
    }

    public void TutorialCompleted()
    {
        CancelInvoke("StartActivityByTimer");
        Invoke("StartActivityByTimer", timeBeforeFirstActivity);
    }
}
