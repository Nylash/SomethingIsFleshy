using UnityEngine;

public class SecondarySystem : MonoBehaviour
{
	#region CONFIGURATION
#pragma warning disable 0649
	[Header("Parameters")]
	[Tooltip("Check it if this organ need energy.")]
	[SerializeField] bool energyNeeded;
	[Tooltip("Check it if this organ need oxygen.")]
	[SerializeField] bool oxygenNeeded;
	[Tooltip("Time before being empty energy stock when full and not currelty filling.")]
	[SerializeField] float maxEnergy = 15f;
	[Tooltip("Time before being empty oxygen stock when full and not currelty filling.")]
	[SerializeField] float maxOxygen = 15f;
	[Tooltip("Use this parameters to set at which energy capacity this system start.")]
	[SerializeField] float startEnergy = 10f;
	[Tooltip("Use this parameters to set at which oxygen capacity this system start.")]
	[SerializeField] float startOxygen = 10f;
	[Header("World objects")]
	[Tooltip("Assign a lever to this parameter to assiocate it with energy filling boolean.")]
	[SerializeField] LeverScript energyLever;
	[Tooltip("Assign a lever to this parameter to assiocate it with oxygen filling boolean.")]
	[SerializeField] LeverScript oxygenLever;
	[Tooltip("Assign a pipe to this parameter to assiocate it with energy filling boolean.")]
	[SerializeField] GameObject energyPipe;
	[Tooltip("Assign a pipe to this parameter to assiocate it with oxygen filling boolean.")]
	[SerializeField] LeverScript oxygenPipe;
	[Tooltip("Check it if this system energy pipe starts open.")]
	[SerializeField] bool energyStartsOpen;
	[Tooltip("Check it if this system oxygen pipe starts open.")]
	[SerializeField] bool oxygenStartsOpen;
#pragma warning restore 0649
	#endregion

	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public float currentEnergy;
	public float currentOxygen;
	public bool fillingEnergy;
	public bool fillingOxygen;

	private void Awake()
	{
		if (energyLever)
		{
			energyLever.associatedSecondarySystem = this;
			energyLever.associatedRessource = LeverScript.RessourcesType.energy;
			if (energyStartsOpen)
				energyLever.Switch();
		}
		if (oxygenLever)
		{
			oxygenLever.associatedSecondarySystem = this;
			oxygenLever.associatedRessource = LeverScript.RessourcesType.oxygen;
			if (oxygenStartsOpen)
				oxygenLever.Switch();
		}
		currentEnergy = startEnergy;
		currentOxygen = startOxygen;
	}

	private void Update()
	{
		if (energyNeeded)
		{
			if (fillingEnergy)
				FillingEnergy();
			else
				EmptyingEnergy();
		}
		if (oxygenNeeded)
		{
			if (fillingOxygen)
				FillingOxygen();
			else
				EmptyingOxygen();
		}
	}

	void FillingEnergy()
	{
		if (StomachManager.instance.Emptying(Time.deltaTime))
		{
			if (currentEnergy + Time.deltaTime >= maxEnergy)
				currentEnergy = maxEnergy;
			else
				currentEnergy += Time.deltaTime;
		}
		else
			EmptyingEnergy();
	}

	void EmptyingEnergy()
	{
		if (currentEnergy - Time.deltaTime >= 0)
			currentEnergy -= Time.deltaTime;
		else
			currentEnergy = 0;
	}

	void FillingOxygen()
	{
		if (LungsManager.instance.Emptying(Time.deltaTime))
		{
			if (currentOxygen + Time.deltaTime >= maxOxygen)
				currentOxygen = maxOxygen;
			else
				currentOxygen += Time.deltaTime;
		}
		else
			EmptyingOxygen();
	}

	void EmptyingOxygen()
	{
		if (currentOxygen - Time.deltaTime >= 0)
			currentOxygen -= Time.deltaTime;
		else
			currentOxygen = 0;
	}
}
