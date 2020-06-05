using UnityEngine;
using UnityEngine.U2D;

public abstract class PrimarySystem : MonoBehaviour
{
	#region CONFIGURATION
#pragma warning disable 0649
	[Header("Parameters")]
	[Tooltip("Time before being empty when full and only one secondary system is open. If two secondary systems are open divide this value by 2 and so on..")]
	[SerializeField] float maxCapacity = 35f;
	[Tooltip("Use this parameters to set at which capacity this system start.")]
	[SerializeField] float startCapacity = 20f;
	[Header("World objects")]
	[Tooltip("Assign a lever to this parameter to assiocate it with filling boolean.")]
	[SerializeField] LeverScript lever;
	[Tooltip("Assign a pipe to this parameter to assiocate it with filling boolean.")]
	[SerializeField] GameObject pipe;
	[Tooltip("Check it if this system pipe starts open.")]
	[SerializeField] bool startsOpen;
#pragma warning restore 0649
	#endregion

	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public float currentCapacity;
	public bool filling;
	protected Color colorPipeClose;
	protected Color colorPipeOpen;
	SpriteShapeController controllerPipe;
	SpriteShapeRenderer rendererPipe;
	Material fillingMaterial;

	protected virtual void Awake()
	{
		if (lever)
			lever.associatedPrimarySystem = this;
		currentCapacity = startCapacity;
	}

	protected virtual void Start()
	{
		if (pipe)
		{
			controllerPipe = pipe.GetComponent<SpriteShapeController>();
			rendererPipe = pipe.GetComponent<SpriteShapeRenderer>();
			if (!startsOpen)
			{
				for (int i = 0; i < controllerPipe.spline.GetPointCount(); i++)
					controllerPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
				rendererPipe.color = colorPipeClose;
			}
		}
		if (startsOpen && lever)
			lever.Switch();
		fillingMaterial = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
		fillingMaterial.SetFloat("Height", currentCapacity / maxCapacity);
	}

	protected virtual void Update()
	{
		ContinuousFilling();
		fillingMaterial.SetFloat("Height", currentCapacity / maxCapacity);
	}

	void ContinuousFilling()
	{
		if (filling)
			Filling(Time.deltaTime);
	}

	public virtual void Filling(float amount)
	{
		if (currentCapacity + amount >= maxCapacity)
			currentCapacity = maxCapacity;
		else
			currentCapacity += amount;
	}

	public virtual bool Emptying(float amount)
	{
		if (currentCapacity - amount >= 0)
		{
			currentCapacity -= amount;
			return true;
		}
		else
			return false;
	}

	public virtual void SwitchPipe()
	{
		if (filling)
		{
			for (int i = 0; i < controllerPipe.spline.GetPointCount(); i++)
				controllerPipe.spline.SetHeight(i, 1);
			rendererPipe.color = colorPipeOpen;
		}
		else
		{
			for (int i = 0; i < controllerPipe.spline.GetPointCount(); i++)
				controllerPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
			rendererPipe.color = colorPipeClose;
		}
	}
}
