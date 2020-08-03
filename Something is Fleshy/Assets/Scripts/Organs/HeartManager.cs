using UnityEngine;
using UnityEngine.EventSystems;

public class HeartManager : MonoBehaviour
{
    public static HeartManager instance;
#pragma warning disable 0649
	[Header("PARAMETERS")]
	[Tooltip("Time to end level (in seconds).")]
	[SerializeField] int timeToFinish = 180;
	[Tooltip("Max health, 1 HP = 1 sec")]
	public float maxHealth;
	[Space]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	[Header("Variables")]
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
		UI_Manager.instance.UI_timerValue.text = timeToFinish.ToString();
		currentTimer = timeToFinish;
    }

	private void Start()
	{
		currentHealth = maxHealth;
		fillingMaterial = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
		fillingMaterial.SetFloat("Height", currentHealth / maxHealth);
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
		fillingMaterial.SetFloat("Height", currentHealth / maxHealth);
	}

	void Defeat()
	{
		UI_Manager.instance.UI_defeatFullTimer.text = timeToFinish.ToString() + " seconds";
		UI_Manager.instance.UI_defeatActualTimer.text = (timeToFinish - (int)currentTimer).ToString() + " seconds";
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
