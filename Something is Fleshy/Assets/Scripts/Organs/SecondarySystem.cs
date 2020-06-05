using UnityEngine;
using UnityEngine.U2D;

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
	[SerializeField] GameObject oxygenPipe;
	[Tooltip("Check it if this system energy pipe starts open.")]
	[SerializeField] bool energyStartsOpen;
	[Tooltip("Check it if this system oxygen pipe starts open.")]
	[SerializeField] bool oxygenStartsOpen;
	[Tooltip("Associated energy filling renderer.")]
	[SerializeField] SpriteRenderer energyGaugeRenderer;
	[Tooltip("Associated oxygen filling renderer.")]
	[SerializeField] SpriteRenderer oxygenGaugeRenderer;
#pragma warning restore 0649
	#endregion

	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public float currentEnergy;
	public float currentOxygen;
	public bool fillingEnergy;
	public bool fillingOxygen;
	SpriteShapeController controllerEnergyPipe;
	SpriteShapeController controllerOxygenPipe;
	SpriteShapeRenderer rendererEnergyPipe;
	SpriteShapeRenderer rendererOxygenPipe;
	Material energyFillingMaterial;
	Material oxygenFillingMaterial;

	private void Awake()
	{
		if (energyLever)
		{
			energyLever.associatedSecondarySystem = this;
			energyLever.associatedRessource = LeverScript.RessourcesType.energy;
		}
		if (oxygenLever)
		{
			oxygenLever.associatedSecondarySystem = this;
			oxygenLever.associatedRessource = LeverScript.RessourcesType.oxygen;
		}
		currentEnergy = startEnergy;
		currentOxygen = startOxygen;
		if (energyGaugeRenderer)
		{
			energyFillingMaterial = energyGaugeRenderer.material;
			energyFillingMaterial.SetFloat("Height", currentEnergy / maxEnergy);
		}
			
		if (oxygenGaugeRenderer)
		{
			oxygenFillingMaterial = oxygenGaugeRenderer.material;
			oxygenFillingMaterial.SetFloat("Height", currentOxygen / maxOxygen);
		}			
	}

	private void Start()
	{
		if (energyPipe)
		{
			controllerEnergyPipe = energyPipe.GetComponent<SpriteShapeController>();
			rendererEnergyPipe = energyPipe.GetComponent<SpriteShapeRenderer>();
			if (!energyStartsOpen)
			{
				for (int i = 0; i < controllerEnergyPipe.spline.GetPointCount(); i++)
					controllerEnergyPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
				rendererEnergyPipe.color = GameManager.instance.energyPipeCloseColor;
			}
		}
		if (oxygenPipe)
		{
			controllerOxygenPipe = oxygenPipe.GetComponent<SpriteShapeController>();
			rendererOxygenPipe = oxygenPipe.GetComponent<SpriteShapeRenderer>();
			if (!oxygenStartsOpen)
			{
				for (int i = 0; i < controllerOxygenPipe.spline.GetPointCount(); i++)
					controllerOxygenPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
				rendererOxygenPipe.color = GameManager.instance.oxygenPipeCloseColor;
			}
		}
		if (energyStartsOpen && energyLever)
			energyLever.Switch();
		if (oxygenStartsOpen && oxygenLever)
			oxygenLever.Switch();
	}

	public void SwitchEnergyPipe()
	{
		if (fillingEnergy)
		{
			for (int i = 0; i < controllerEnergyPipe.spline.GetPointCount(); i++)
				controllerEnergyPipe.spline.SetHeight(i, 1);
			rendererEnergyPipe.color = GameManager.instance.energyPipeOpenColor;
		}
		else
		{
			for (int i = 0; i < controllerEnergyPipe.spline.GetPointCount(); i++)
				controllerEnergyPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
			rendererEnergyPipe.color = GameManager.instance.energyPipeCloseColor;
		}
	}

	public void SwitchOxygenPipe()
	{
		if (fillingOxygen)
		{
			for (int i = 0; i < controllerOxygenPipe.spline.GetPointCount(); i++)
				controllerOxygenPipe.spline.SetHeight(i, 1);
			rendererOxygenPipe.color = GameManager.instance.oxygenPipeOpenColor;
		}
		else
		{
			for (int i = 0; i < controllerOxygenPipe.spline.GetPointCount(); i++)
				controllerOxygenPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
			rendererOxygenPipe.color = GameManager.instance.oxygenPipeCloseColor;
		}
	}

	private void Update()
	{
		if (energyNeeded)
		{
			if (fillingEnergy)
				FillingEnergy();
			else
				EmptyingEnergy();
			energyFillingMaterial.SetFloat("Height", currentEnergy / maxEnergy);
		}
		if (oxygenNeeded)
		{
			if (fillingOxygen)
				FillingOxygen();
			else
				EmptyingOxygen();
			oxygenFillingMaterial.SetFloat("Height", currentOxygen / maxOxygen);
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
