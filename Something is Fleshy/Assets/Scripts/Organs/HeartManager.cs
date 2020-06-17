using UnityEngine;
using TMPro;

public class HeartManager : MonoBehaviour
{
    public static HeartManager instance;
#pragma warning disable 0649
	[Header("PARAMETERS")]
	[Tooltip("Time to end level (in seconds).")]
	[SerializeField] int timeToFinish = 180;
	[Tooltip("Max health, 1 HP = 1 sec")]
	[SerializeField] float maxHealth;
	[Space]
	[Header("UI Objects")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	[SerializeField] TextMeshProUGUI UI_timer;
	[SerializeField] Canvas UI_defeatCanvas;
	[SerializeField] TextMeshProUGUI UI_defeatFullTimer;
	[SerializeField] TextMeshProUGUI UI_defeatActualTimer;
#pragma warning restore 0649
	[Header("Variables")]
	public float currentHealth;
	Material fillingMaterial;
	float currentTimer;

	private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
		UI_timer.text = timeToFinish.ToString();
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
		currentTimer -= Time.deltaTime;
		UI_timer.text = ((int)currentTimer).ToString();
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
		UI_defeatFullTimer.text = timeToFinish.ToString() + " seconds";
		UI_defeatActualTimer.text = ((int)currentTimer).ToString() + " seconds";
		UI_defeatCanvas.enabled = true;
	}
}
