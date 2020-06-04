using UnityEngine;
using UnityEditor.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

	#region CONFIGURATION
	[Header("Global parameters")]
	[Tooltip("Pipe's height when close.")]
	[Range(.2f,1)] public float pipeCloseHeight;
	[Tooltip("Energy pipe's color when open.")]
	public Color energyPipeOpenColor;
	[Tooltip("Energy pipe's color when close.")]
	public Color energyPipeCloseColor;
	[Tooltip("Oxygen pipe's color when open.")]
	public Color oxygenPipeOpenColor;
	[Tooltip("Oxygen pipe's color when close.")]
	public Color oxygenPipeCloseColor;
	#endregion

	private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
