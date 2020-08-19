using UnityEngine;

public abstract class PrimarySystem : MonoBehaviour
{
	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public float currentCapacity;
	public bool filling;
	Material fillingMaterial;

	protected virtual void Awake()
	{
		currentCapacity = GameManager.instance.startCapacityPrimarySystem;
		fillingMaterial = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
		fillingMaterial.SetFloat("Height", currentCapacity / GameManager.instance.maxCapacityPrimarySystem);
	}

	protected virtual void Update()
	{
		ContinuousFilling();
		fillingMaterial.SetFloat("Height", currentCapacity / GameManager.instance.maxCapacityPrimarySystem);
	}

	void ContinuousFilling()
	{
		if (GameManager.instance.levelStarted)
		{
			if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused)
			{
				if (filling)
					Filling(Time.deltaTime * GameManager.instance.fillingMultiplierPrimarySystem);
			}
		}
	}

	public virtual void Filling(float amount)
	{
		if (currentCapacity + amount >= GameManager.instance.maxCapacityPrimarySystem)
			currentCapacity = GameManager.instance.maxCapacityPrimarySystem;
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
