using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class SecondarySystem : MonoBehaviour
{
#pragma warning disable 0649
	[Header("PARAMETERS")]
	
	[Tooltip("Check this if the member associated to this SS  uses bones.")]
	[SerializeField] bool memberIsBoned;
#pragma warning restore 0649
	[Space]
	[Header("References")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	//[ConditionalHide("showVariables", true)]
	[Tooltip("Associated energy filling renderer.")]
	public GameObject energyGauge;
	[Tooltip("Associated oxygen filling renderer.")]
	public GameObject oxygenGauge;
	[Header("Components")]
	public Animator animator;
	public Animator memberAnimator;
	[Header("Variables")]
	public LeverScript associatedLever;
	public List<SecondarySystem> associatedPack = new List<SecondarySystem>();
	public bool canBeSelectedAgain;
	public GameObject associatedHint;
	public float currentEnergy;
	public float currentOxygen;
	public bool energyNeeded;
	public bool oxygenNeeded;
	public bool filling;
	public float timerBeforeExpiration;
	bool checkIfCanBeSelectedAgain;
	MaterialPropertyBlock energyPropertyBlock;
	MaterialPropertyBlock oxygenPropertyBlock;
	SpriteRenderer energyRenderer;
	SpriteRenderer oxygenRenderer;

	private void Start()
	{
		energyPropertyBlock = new MaterialPropertyBlock();
		energyRenderer = energyGauge.GetComponent<SpriteRenderer>();
		energyPropertyBlock.SetFloat("Height", currentEnergy / SecondarySystemsManager.instance.energyAmoutNeeded);
		energyRenderer.SetPropertyBlock(energyPropertyBlock);

		oxygenPropertyBlock = new MaterialPropertyBlock();
		oxygenRenderer = oxygenGauge.GetComponent<SpriteRenderer>();
		oxygenPropertyBlock.SetFloat("Height", currentOxygen / SecondarySystemsManager.instance.oxygenAmoutNeeded);
		oxygenRenderer.SetPropertyBlock(oxygenPropertyBlock);

		energyGauge.SetActive(false);
		oxygenGauge.SetActive(false);
		animator = GetComponent<Animator>();

		if (memberIsBoned)
			memberAnimator = transform.parent.parent.parent.GetComponent<Animator>();
        else
        {
			if(transform.parent.GetComponent<Animator>())
				memberAnimator = transform.parent.GetComponent<Animator>();
		}
		if (memberAnimator)
			memberAnimator.speed = 0;
	}

	private void Update()
	{
		if (GameManager.instance.levelStarted)
		{
			if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused)
			{
				if (energyNeeded)
				{
					if (filling)
						FillingEnergy();
					timerBeforeExpiration += Time.deltaTime;
				energyPropertyBlock.SetFloat("Height", currentEnergy / SecondarySystemsManager.instance.energyAmoutNeeded);
				energyRenderer.SetPropertyBlock(energyPropertyBlock);
				}
				else if (oxygenNeeded)
				{
					if (filling)
						FillingOxygen();
					timerBeforeExpiration += Time.deltaTime;
				oxygenPropertyBlock.SetFloat("Height", currentOxygen / SecondarySystemsManager.instance.oxygenAmoutNeeded);
				oxygenRenderer.SetPropertyBlock(oxygenPropertyBlock);
				}
				CheckStopActivity();
                if (checkIfCanBeSelectedAgain)
                {
                    if (canBeSelectedAgain)
                    {
						checkIfCanBeSelectedAgain = false;
						SecondarySystemsManager.instance.AddPack(associatedPack);
					}
                }
			}
		}
	}

	void FillingEnergy()
	{
		if (StomachManager.instance.Emptying(Time.deltaTime))
		{
			if (currentEnergy + Time.deltaTime >= SecondarySystemsManager.instance.energyAmoutNeeded)
				currentEnergy = SecondarySystemsManager.instance.energyAmoutNeeded;
			else
				currentEnergy += Time.deltaTime;
		}
	}

	void FillingOxygen()
	{
		if (LungsManager.instance.Emptying(Time.deltaTime))
		{
			if (currentOxygen + Time.deltaTime >= SecondarySystemsManager.instance.oxygenAmoutNeeded)
				currentOxygen = SecondarySystemsManager.instance.oxygenAmoutNeeded;
			else
				currentOxygen += Time.deltaTime;
		}
	}

	void CheckStopActivity()
	{
		if (energyNeeded)
		{
			if (currentEnergy / SecondarySystemsManager.instance.energyAmoutNeeded >= 1)
				StopActivity(true);
		}
		else if (oxygenNeeded)
		{
			if (currentOxygen / SecondarySystemsManager.instance.oxygenAmoutNeeded >= 1)
				StopActivity(true);
		}
		if(timerBeforeExpiration >= SecondarySystemsManager.instance.timeBeforeExpirationSecondarySystem)
			StopActivity(false);
	}

	void StopActivity(bool suceed)
	{
        if (suceed)
        {
			ScoreManager.instance.WinPoints((int)GameManager.instance.pointsWinSecondarySystemFilled.Evaluate(timerBeforeExpiration/SecondarySystemsManager.instance.timeBeforeExpirationSecondarySystem));
		}
        else
        {
			ScoreManager.instance.LosePoints(GameManager.instance.pointsLossSecondarySystemExpiration);
        }
		timerBeforeExpiration = 0f;
		filling = false;
		associatedLever.IsSecondarySystemFilling(false);
		energyNeeded = false;
		oxygenNeeded = false;
		energyGauge.SetActive(false);
		oxygenGauge.SetActive(false);
		animator.SetBool("OnActivity", false);
		if (memberAnimator)
			memberAnimator.speed = 0;
		currentEnergy = 0f;
		currentOxygen = 0f;
		energyPropertyBlock.SetFloat("Height", currentEnergy / SecondarySystemsManager.instance.energyAmoutNeeded);
		energyRenderer.SetPropertyBlock(energyPropertyBlock);
		oxygenPropertyBlock.SetFloat("Height", currentOxygen / SecondarySystemsManager.instance.oxygenAmoutNeeded);
		oxygenRenderer.SetPropertyBlock(oxygenPropertyBlock);
		if (canBeSelectedAgain)
			SecondarySystemsManager.instance.AddPack(associatedPack);
		else
			checkIfCanBeSelectedAgain = true;
		HintSecondarySystemManager.instance.activeSecondarySystems.Remove(this);
		if (associatedHint)
			Destroy(associatedHint);
	}
}