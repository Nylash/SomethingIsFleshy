using UnityEngine;
using UnityEngine.U2D;

public class ElectricSwitch : MonoBehaviour
{
    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("Check this if you want platformsB to be the first activated.")]
    [SerializeField] bool startWithPlatformsB;
    [Tooltip("Color of electric platforms when inactive")]
    [SerializeField] Color initialColor;
#pragma warning restore 0649
    #endregion

    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public SpriteShapeRenderer[] platformsA;
    public SpriteShapeRenderer[] platformsB;
    bool platformsA_activated = true;
    Color colorPlatformsA;
    Color colorPlatformsB;
    //Only used for animation purpose
    bool isSwitched;

    private void Start()
    {
        int nbPlatformsA = transform.GetChild(1).transform.childCount;
        platformsA = new SpriteShapeRenderer[nbPlatformsA];
        for (int i = 0; i < nbPlatformsA; i++)
        {
            platformsA[i] = transform.GetChild(1).transform.GetChild(i).GetComponent<SpriteShapeRenderer>();
        }
        int nbPlatformsB = transform.GetChild(2).transform.childCount;
        platformsB = new SpriteShapeRenderer[nbPlatformsB];
        for (int i = 0; i < nbPlatformsB; i++)
        {
            platformsB[i] = transform.GetChild(2).transform.GetChild(i).GetComponent<SpriteShapeRenderer>();
        }
        colorPlatformsA = platformsA[0].color;
        colorPlatformsB = platformsB[0].color;
        if (!startWithPlatformsB)
        {
            MakeSwitch(platformsA, platformsB, colorPlatformsA);
        }
        else
        {
            platformsA_activated = false;
            MakeSwitch(platformsB, platformsA, colorPlatformsB);
        }
    }

    public void Switch()
    {
        if (platformsA_activated)
        {
            platformsA_activated = false;
            MakeSwitch(platformsB, platformsA, colorPlatformsB);
        }
        else
        {
            platformsA_activated = true;
            MakeSwitch(platformsA, platformsB, colorPlatformsA);
        }
        AnimHandler();
    }

    void MakeSwitch(SpriteShapeRenderer[] activated, SpriteShapeRenderer[] desactivated, Color activatedColor)
    {
        foreach (SpriteShapeRenderer item in activated)
        {
            if(item.gameObject.layer != 8)
            {
                item.GetComponent<ElectricWall>().Activate();
            }
            else
            {
                item.color = activatedColor;
                item.tag = "ElectricPlatform";
            }
        }
        foreach (SpriteShapeRenderer item in desactivated)
        {
            if (item.gameObject.layer != 8)
            {
                item.GetComponent<ElectricWall>().Desactivate();
            }
            else
            {
                item.color = initialColor;
                item.tag = "Untagged";
            }
        }
    }

    void AnimHandler()
    {
        isSwitched = !isSwitched;
        if (isSwitched)
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, -85);
        else
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
