using UnityEngine;

public class InversionBlocks : MonoBehaviour
{
    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("Time before the first activity.")]
    [SerializeField] float timeBetweenSwitch = 10f;
    [Tooltip("Check this if you want blocksB to be the first activated.")]
    [SerializeField] bool startWithBlocksB;
#pragma warning restore 0649
    #endregion

    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public Animator[] blocksA;
    public Animator[] blocksB;
    bool blockAactivated = true;

    private void Start()
    {
        int nbAblocks = transform.GetChild(0).transform.childCount;
        blocksA = new Animator[nbAblocks];
        for (int i = 0; i < nbAblocks; i++)
        {
            blocksA[i] = transform.GetChild(0).transform.GetChild(i).GetComponent<Animator>();
        }
        int nbBblocks = transform.GetChild(1).transform.childCount;
        blocksB = new Animator[nbBblocks];
        for (int i = 0; i < nbBblocks; i++)
        {
            blocksB[i] = transform.GetChild(1).transform.GetChild(i).GetComponent<Animator>();
        }
        if (!startWithBlocksB)
        {
            foreach (Animator item in blocksB)
                item.SetTrigger("Depop");
        }
        else
        {
            blockAactivated = false;
            foreach (Animator item in blocksA)
                item.SetTrigger("Depop");
        }
        InvokeRepeating("InverseBlocks", timeBetweenSwitch, timeBetweenSwitch);
    }

    void InverseBlocks()
    {
        if (blockAactivated)
        {
            blockAactivated = false;
            foreach (Animator item in blocksA)
                item.SetTrigger("Depop");
            foreach (Animator item in blocksB)
                item.SetTrigger("Pop");
        }
        else
        {
            blockAactivated = true;
            foreach (Animator item in blocksA)
                item.SetTrigger("Pop");
            foreach (Animator item in blocksB)
                item.SetTrigger("Depop");
        }
    }
}
