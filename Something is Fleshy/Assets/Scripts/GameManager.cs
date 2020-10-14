using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

	#region CONFIGURATION
	[Header("REFERENCES")]
	[Tooltip("Color associated to energy, uses for FX,pipes...")]
	public Color energyColor;
	[Tooltip("Color associated to oxygen, uses for FX,pipes...")]
	public Color oxygenColor;
	[Tooltip("Pipe's color when open and empty.")]
	public Color emptyPipeOpenColor;
	[Tooltip("Pipe's color when close.")]
	public Color pipeCloseColor;
	[Tooltip("Pipe's shape when open.")]
	public SpriteShape pipeOpenShape;
	[Tooltip("Pipe's shape when close.")]
	public SpriteShape pipeCloseShape;
	[Tooltip("Prefab to track time before expiration of the secondary system")]
	public GameObject UI_timerSS;
	public Sprite goldMedal;
	public Sprite silverMedal;
	public Sprite bronzeMedal;
	public Sprite failMedal;
	[Space]
	[Header("PARAMETERS")]
	[Tooltip("Level duration (in seconds).")]
	public float levelDuration = 180;
	[Space]
	[Tooltip("Time before being empty when full and only one secondary system is open. If two secondary systems are open divide this value by 2 and so on..")]
	public float maxCapacityPrimarySystem = 35f;
	[Tooltip("Use this parameters to set at which capacity this system start.")]
	public float startCapacityPrimarySystem = 20f;
	[Tooltip("Multiplier ratio for filling over time. By default 1.")]
	[Min(1f)] public float fillingMultiplierPrimarySystem = 1;
	[Space]
	[Tooltip("Curve used to determine points earned when a secondary system is filled, depending on the time taken.")]
	public AnimationCurve pointsWinSecondarySystemFilled;
	[Tooltip("Points loss when a secondary system expires.")]
	public int pointsLossSecondarySystemExpiration = 20;
	[Tooltip("Points loss when a nerve is hit.")]
	public int pointsLossNerveHit = 5;
	[Space]
	[Tooltip("Points required to get gold medal.")]
	public int pointsForGold = 300;
	[Tooltip("Points required to get silver medal.")]
	public int pointsForSilver = 200;
	[Tooltip("Points required to get bronze medal.")]
	public int pointsForBronze = 120;
	#endregion
	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public bool levelStarted;
	public bool levelPaused;

	private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        for (int i = 0; i < 2; i++)
        {
			PlayersManager.instance.players[i].transform.position = transform.GetChild(i).transform.position;
			PlayersManager.instance.players[i].GetComponent<CharacterController2D>().enabled = true;
			PlayersManager.instance.players[i].GetComponent<InteractionManager>().enabled = true;
			PlayersManager.instance.players[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			PlayersManager.instance.players[i].GetComponent<PlayerInput>().enabled = true;
			PlayersManager.instance.players[i].GetComponentInChildren<PlayerAnimationsMethods>().enabled = true;
			PlayersManager.instance.players[i].GetComponent<PlayerInput>().notificationBehavior = PlayerNotifications.InvokeUnityEvents;
		}
    }
}
