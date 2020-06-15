using UnityEngine;
using UnityEditor.Rendering;

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
	#endregion

	private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
