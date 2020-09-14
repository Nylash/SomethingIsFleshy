using UnityEngine;

public abstract class PrimarySystem : MonoBehaviour
{
	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public float currentCapacity;
	public bool filling;
	Material fillingMaterial;
	Animator anim;
	protected AudioSource audioSource;

	protected virtual void Start()
	{
		currentCapacity = GameManager.instance.startCapacityPrimarySystem;
		fillingMaterial = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
		fillingMaterial.SetFloat("Height", currentCapacity / GameManager.instance.maxCapacityPrimarySystem);
		anim = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
	}

	protected virtual void Update()
	{
		if (GameManager.instance.levelStarted)
		{
			if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
			{
				ContinuousFilling();
				fillingMaterial.SetFloat("Height", currentCapacity / GameManager.instance.maxCapacityPrimarySystem);
				if (currentCapacity / GameManager.instance.maxCapacityPrimarySystem < .05f)
				{
					if (!anim.GetBool("LowRessource"))
                    {
						anim.SetBool("LowRessource", true);
						StartLowRessource();
					}
				}
				else
				{
					if (anim.GetBool("LowRessource"))
						anim.SetBool("LowRessource", false);
				}
			}
		}
	}

	void ContinuousFilling()
	{
		if (filling)
			Filling(Time.deltaTime * GameManager.instance.fillingMultiplierPrimarySystem);
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

	public abstract void StartLowRessource();
}
