using System.Collections.Generic;
using UnityEngine;

public class LeaksManager : MonoBehaviour
{
    public static LeaksManager instance;

    [Header("PARAMETERS")]
#pragma warning disable 0649
    [SerializeField] float timeBeforeFirstCheckForLeak;
    [SerializeField] float timeIntervalForCheckForLeak;
    [Range(0, 100)] [SerializeField] int probabilityLeak;
    [SerializeField] GameObject leakPrefab;
    [Tooltip("Increase probability a leak occurs by this value each time a leak doesn't occur.")]
    [Range(0, 50)] [SerializeField] int leakProbWeightValue = 20;
    [Tooltip("The player must hold interaction input for this time to repair the leak.")]
    public float timeToRepair = 1.5f;
#pragma warning restore 0649
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public List<LeverScript> leversWithLeaksZones = new List<LeverScript>();
    public List<GameObject> allLeaks = new List<GameObject>();
    int leakProbWeight;
    int runStateWeight;

    ActionsMap actionsMap;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        actionsMap = new ActionsMap();
        actionsMap.Gameplay.Debug.started += ctx => StartLeak();

        InvokeRepeating("CheckLeak", timeBeforeFirstCheckForLeak, timeIntervalForCheckForLeak);
    }

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    void CheckLeak()
    {
        float prob = Random.Range(0, 100);
        runStateWeight = 0;
        CheckPlayerRunState();
        if (prob < probabilityLeak + leakProbWeight + runStateWeight)
        {
            StartLeak();
            leakProbWeight = 0;
        }
        else
            leakProbWeight += leakProbWeightValue;
    }

    void CheckPlayerRunState()
    {
        if (allLeaks.Count != 0)
            return;
        if(StomachManager.instance.currentCapacity / StomachManager.instance.maxCapacity > .6f && LungsManager.instance.currentCapacity / LungsManager.instance.maxCapacity > .6f)
            runStateWeight += leakProbWeightValue;
        if (HeartManager.instance.currentHealth / HeartManager.instance.maxHealth < .2f)
            runStateWeight -= leakProbWeightValue;
    }

    void StartLeak()
    {
        if (leversWithLeaksZones.Count != 0)
        {
            bool checkIfLeakAvailable = false;
            foreach (LeverScript item in leversWithLeaksZones)
            {
                for (int i = 0; i < 2; i++)
                {
                    if(item.allLeaksZones[i] != null)
                    {
                        checkIfLeakAvailable = true;
                        goto LoopEnd;
                    }
                }
            }
            LoopEnd:
            if (!checkIfLeakAvailable)
                return; // no leak available
            int lever;
            do
                lever = Random.Range(0, leversWithLeaksZones.Count);
            while 
            (leversWithLeaksZones[lever].allLeaksZones[0] == null && leversWithLeaksZones[lever].allLeaksZones[1] == null);
            int pipe;
            if(leversWithLeaksZones[lever].allLeaksZones[0] != null && leversWithLeaksZones[lever].allLeaksZones[1] != null)
                pipe = Random.Range(0, 2);
            else
            {
                if (leversWithLeaksZones[lever].allLeaksZones[0] != null)
                    pipe = 0;
                else
                    pipe = 1;
            }
            int leakZone = Random.Range(0, leversWithLeaksZones[lever].allLeaksZones[pipe].Count);
            GameObject leak = Instantiate(leakPrefab, leversWithLeaksZones[lever].allLeaksZones[pipe][leakZone].GetRandomLeakPosition(), Quaternion.identity);
            Leak leakScript = leak.GetComponent<Leak>();
            leakScript.associatedPipeLeaksZones = leversWithLeaksZones[lever].allLeaksZones[pipe];
            leakScript.associatedLever = leversWithLeaksZones[lever];
            leakScript.associatedPipe = pipe;
            leversWithLeaksZones[lever].allLeaksZones[pipe] = null;
            allLeaks.Add(leak);
        }
    }
}
