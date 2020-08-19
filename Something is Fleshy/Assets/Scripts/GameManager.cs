using UnityEngine;
using UnityEngine.U2D;

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
	public GameObject UI_timerSS;
	[Space]
	[Header("PARAMETERS")]
	[Tooltip("Time to end level (in seconds).")]
	public int timeToFinishLevel = 180;
	[Tooltip("Max health, 1 HP = 1 sec")]
	public float maxHealth;
	[Space]
	[Tooltip("Time before being empty when full and only one secondary system is open. If two secondary systems are open divide this value by 2 and so on..")]
	public float maxCapacityPrimarySystem = 35f;
	[Tooltip("Use this parameters to set at which capacity this system start.")]
	public float startCapacityPrimarySystem = 20f;
	[Tooltip("Multiplier ratio for filling over time. By default 1.")]
	[Min(1f)] public float fillingMultiplierPrimarySystem = 1;
	[Space]
	[Tooltip("Damage done when a SS explode.")]
	public float SSexplosionDamage = 20f;
	[Tooltip("How many damage is deal to the heart when the player hit a nerve.")]
	public float nerveDamage = 5f;
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
    }

	private void Start()
	{
		levelStarted = true;
	}
}
