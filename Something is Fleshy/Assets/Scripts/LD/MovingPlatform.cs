using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [Tooltip("The speed of the moving platform.")]
    [SerializeField] float travelSpeed;
    [Tooltip("Check this if you want that the moving platform goes to B first.")]
    [SerializeField] bool inverseDirection; 
#pragma warning restore 0649
    #endregion

    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public Transform PointA;
    public Transform PointB;
    public bool goingToA = true;
    Vector2 pointAPosition;
    Vector2 pointBPosition;
    Vector3 initialPosition;
    float timer;


    private void Start()
    {
        pointAPosition = PointA.position;
        pointBPosition = PointB.position;
        if (inverseDirection)
            goingToA = false;
        if (goingToA)
            initialPosition = pointBPosition;
        else
            initialPosition = pointAPosition;
    }

    private void Update()
    {
        if (goingToA)
        {
            if(Vector2.Distance(transform.position,pointAPosition) > .05f)
            {
                transform.position = Vector3.Lerp(initialPosition, pointAPosition, timer * travelSpeed);
                timer += Time.deltaTime;
            }
            else
            {
                goingToA = false;
                initialPosition = pointAPosition;
                timer = 0f;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, pointBPosition) > .05f)
            {
                transform.position = Vector3.Lerp(initialPosition, pointBPosition, timer * travelSpeed);
                timer += Time.deltaTime;
            }
            else
            {
                goingToA = true;
                initialPosition = pointBPosition;
                timer = 0f;
            }
        }
    }
}
