using UnityEngine;
using UnityEngine.EventSystems;

public class HeartManager : MonoBehaviour
{
    public static HeartManager instance;

	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public float currentHealth;
	public float currentTimer;
	public bool defeatOrVictory;
	Material fillingMaterial;

	private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
		UI_Manager.instance.UI_timerValue.text = GameManager.instance.timeToFinishLevel.ToString();
		currentTimer = GameManager.instance.timeToFinishLevel;
    }

	private void Start()
	{
		currentHealth = GameManager.instance.maxHealth;
		fillingMaterial = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
		fillingMaterial.SetFloat("Height", currentHealth / GameManager.instance.maxHealth);
	}

	private void Update()
	{
		if (GameManager.instance.levelStarted)
		{
			if(!defeatOrVictory && !GameManager.instance.levelPaused)
			{
				currentTimer -= Time.deltaTime;
				UI_Manager.instance.UI_timerValue.text = ((int)currentTimer).ToString();
				if (currentTimer <= 0)
					Victory();
			}
		}
	}

	public void TakeDamage(float amount)
	{
		currentHealth -= amount;
		if(currentHealth < 0)
		{
			currentHealth = 0;
			Defeat();
		}
		fillingMaterial.SetFloat("Height", currentHealth / GameManager.instance.maxHealth);
	}

	void Defeat()
	{
		UI_Manager.instance.UI_defeatFullTimer.text = GameManager.instance.timeToFinishLevel.ToString() + " seconds";
		UI_Manager.instance.UI_defeatActualTimer.text = (GameManager.instance.timeToFinishLevel - (int)currentTimer).ToString() + " seconds";
		UI_Manager.instance.UI_defeatCanvas.enabled = true;
		EventSystem.current.SetSelectedGameObject(UI_Manager.instance.UI_defeatButton.gameObject);
		defeatOrVictory = true;
		SecondarySystemsManager.instance.StopActivityCall();
	}

	void Victory()
	{
		UI_Manager.instance.UI_victoryCanvas.enabled = true;
		EventSystem.current.SetSelectedGameObject(UI_Manager.instance.UI_victoryButton.gameObject);
		defeatOrVictory = true;
		SecondarySystemsManager.instance.StopActivityCall();
	}
}
