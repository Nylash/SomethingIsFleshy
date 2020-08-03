using UnityEngine;

public abstract class PrimarySystem : MonoBehaviour
{
	#region CONFIGURATION
#pragma warning disable 0649
	[Header("PARAMETERS")]
	[Tooltip("Time before being empty when full and only one secondary system is open. If two secondary systems are open divide this value by 2 and so on..")]
	public float maxCapacity = 35f;
	[Tooltip("Use this parameters to set at which capacity this system start.")]
	[SerializeField] float startCapacity = 20f;
	[Tooltip("Multiplier ratio for filling over time. By default 1.")]
	[Min(1f)] [SerializeField] float fillingMultiplier = 1;
#pragma warning restore 0649
	#endregion
	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public float currentCapacity;
	public bool filling;
	Material fillingMaterial;

	protected virtual void Awake()
	{
		currentCapacity = startCapacity;
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
		if (GameManager.instance.levelStarted)
		{
			if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused)
			{
				if (filling)
					Filling(Time.deltaTime * fillingMultiplier);
			}
		}
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
}
