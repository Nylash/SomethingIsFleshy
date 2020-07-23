using UnityEngine;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

	#region CONFIGURATION
	[Header("PARAMETERS")]
	[Tooltip("Pipe's color when open and full of energy.")]
	public Color energyPipeOpenColor;
	[Tooltip("Pipe's color when open and full of oxygen.")]
	public Color oxygenPipeOpenColor;
	[Tooltip("Pipe's color when open and empty.")]
	public Color emptyPipeOpenColor;
	[Tooltip("Pipe's color when close.")]
	public Color pipeCloseColor;
	[Tooltip("Pipe's shape when open.")]
	public SpriteShape pipeOpenShape;
	[Tooltip("Pipe's shape when close.")]
	public SpriteShape pipeCloseShape;
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
