using UnityEngine;

public class HeartManager : MonoBehaviour
{
    public static HeartManager instance;
#pragma warning disable 0649
	[Header("PARAMETERS")]
	[Tooltip("Max health, 1 HP = 1 sec")]
	[SerializeField] float maxHealth;
#pragma warning restore 0649
	[Space]
	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public float currentHealth;
	Material fillingMaterial;

	private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

	private void Start()
	{
		currentHealth = maxHealth;
		fillingMaterial = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
		fillingMaterial.SetFloat("Height", currentHealth / maxHealth);
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
		print("t'as perdu, cheh");
	}
}
