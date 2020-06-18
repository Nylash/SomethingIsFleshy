using UnityEngine;
using System.Collections;

public class SecondarySystem : MonoBehaviour
{
#pragma warning disable 0649
	[Header("PARAMETERS")]
	[Tooltip("Time to full this system in energy.")]
	[SerializeField] float energyAmoutNeeded = 10f;
	[Tooltip("Time to full this system in oxygen.")]
	[SerializeField] float oxygenAmoutNeeded = 10f;
	[Tooltip("Time before the system can pick again for an activity.")]
	[SerializeField] float timeBeforeReadyForNewActivity = 10f;
#pragma warning restore 0649
	[Space]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	//[ConditionalHide("showVariables", true)]
	[Tooltip("Associated energy filling renderer.")]
	public GameObject energyGauge;
	[Tooltip("Associated oxygen filling renderer.")]
	public GameObject oxygenGauge;
	[Header("Components")]
	public Animator animator;
	[Header("Variables")]
	public float currentEnergy;
	public float currentOxygen;
	public bool energyNeeded;
	public bool oxygenNeeded;
	public bool filling;
	MaterialPropertyBlock energyPropertyBlock;
	MaterialPropertyBlock oxygenPropertyBlock;
	SpriteRenderer energyRenderer;
	SpriteRenderer oxygenRenderer;

	private void Awake()
	{
		energyPropertyBlock = new MaterialPropertyBlock();
		energyRenderer = energyGauge.GetComponent<SpriteRenderer>();
		energyPropertyBlock.SetFloat("Height", currentEnergy / energyAmoutNeeded);
		energyRenderer.SetPropertyBlock(energyPropertyBlock);

		oxygenPropertyBlock = new MaterialPropertyBlock();
		oxygenRenderer = oxygenGauge.GetComponent<SpriteRenderer>();
		oxygenPropertyBlock.SetFloat("Height", currentOxygen / oxygenAmoutNeeded);
		oxygenRenderer.SetPropertyBlock(oxygenPropertyBlock);

		energyGauge.SetActive(false);
		oxygenGauge.SetActive(false);
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (GameManager.instance.levelStarted)
		{
			if (!HeartManager.instance.defeatOrVictory && !GameManager.instance.levelPaused)
			{
				if (energyNeeded)
				{
					if (filling)
						FillingEnergy();
					else
						HeartManager.instance.TakeDamage(Time.deltaTime);
					energyPropertyBlock.SetFloat("Height", currentEnergy / energyAmoutNeeded);
					energyRenderer.SetPropertyBlock(energyPropertyBlock);
				}
				else if (oxygenNeeded)
				{
					if (filling)
						FillingOxygen();
					else
						HeartManager.instance.TakeDamage(Time.deltaTime);
					oxygenPropertyBlock.SetFloat("Height", currentOxygen / oxygenAmoutNeeded);
					oxygenRenderer.SetPropertyBlock(oxygenPropertyBlock);
				}
				CheckStopActivity();
			}
		}
	}

	void FillingEnergy()
	{
		if (StomachManager.instance.Emptying(Time.deltaTime))
		{
			if (currentEnergy + Time.deltaTime >= energyAmoutNeeded)
				currentEnergy = energyAmoutNeeded;
			else
				currentEnergy += Time.deltaTime;
		}
	}

	void FillingOxygen()
	{
		if (LungsManager.instance.Emptying(Time.deltaTime))
		{
			if (currentOxygen + Time.deltaTime >= oxygenAmoutNeeded)
				currentOxygen = oxygenAmoutNeeded;
			else
				currentOxygen += Time.deltaTime;
		}
	}

	void CheckStopActivity()
	{
		if (energyNeeded)
		{
			if (currentEnergy / energyAmoutNeeded >= 1)
				StartCoroutine(StopActivity());
		}
		else if (oxygenNeeded)
		{
			if (currentOxygen / oxygenAmoutNeeded >= 1)
				StartCoroutine(StopActivity());
		}
	}

	IEnumerator StopActivity()
	{
		energyNeeded = false;
		oxygenNeeded = false;
		energyGauge.SetActive(false);
		oxygenGauge.SetActive(false);
		animator.SetBool("OnActivity", false);
		currentEnergy = 0f;
		currentOxygen = 0f;
		yield return new WaitForSeconds(timeBeforeReadyForNewActivity);
		SecondarySystemsManager.instance.secondarySystems.Add(this);
	}
}