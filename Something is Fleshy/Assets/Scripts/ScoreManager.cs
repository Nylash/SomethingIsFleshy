using UnityEngine;
using System;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
	public static ScoreManager instance;

	[Header("Variables")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public float currentTimer;
	public bool levelEnded;
	public int currentScore = 0;
	public int secondarySystemsExpired;
	public int secondarySystemsFilled;
	Coroutine endLevel;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}

	private void Start()
	{
		UI_Manager.instance.UI_scoreValue.text = currentScore.ToString();
		currentTimer = GameManager.instance.levelDuration;
		UI_Manager.instance.UI_timerValue.text = TimeSpan.FromSeconds(currentTimer).ToString("mm' : 'ss");
	}

	private void Update()
	{
		if (GameManager.instance.levelStarted)
		{
			if (!levelEnded && !GameManager.instance.levelPaused)
			{
				currentTimer -= Time.deltaTime;
				UI_Manager.instance.UI_timerValue.text = TimeSpan.FromSeconds(currentTimer).ToString("mm' : 'ss");
				if (currentTimer <= 10 && endLevel == null && !levelEnded)
					endLevel = StartCoroutine(TimerEndLevel());
				if (currentTimer <= 0)
					EndLevel();
			}
		}
	}

	void EndLevel()
    {
		levelEnded = true;
		if (currentScore >= GameManager.instance.pointsForGold)
			UI_Manager.instance.UI_medal.sprite = GameManager.instance.goldMedal;
		else if (currentScore >= GameManager.instance.pointsForSilver)
			UI_Manager.instance.UI_medal.sprite = GameManager.instance.silverMedal;
		else if (currentScore >= GameManager.instance.pointsForBronze)
			UI_Manager.instance.UI_medal.sprite = GameManager.instance.bronzeMedal;
		else
			UI_Manager.instance.UI_medal.sprite = GameManager.instance.failMedal;
		if (UI_Manager.instance.UI_medal.sprite == GameManager.instance.failMedal)
			SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Defeat, SecondarySystemsManager.instance.ssManagerSource);
		else
			SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Victory, SecondarySystemsManager.instance.ssManagerSource);
		UI_Manager.instance.UI_endScore.text = "Your score : " + currentScore.ToString();
		UI_Manager.instance.UI_goldScore.text = ": " + GameManager.instance.pointsForGold.ToString();
		UI_Manager.instance.UI_silverScore.text = ": " + GameManager.instance.pointsForSilver.ToString();
		UI_Manager.instance.UI_bronzeScore.text = ": " + GameManager.instance.pointsForBronze.ToString();
		UI_Manager.instance.UI_endCanvas.enabled = true;
		SecondarySystemsManager.instance.StopActivityCall();
		LeaksManager.instance.StopLeakCall();
	}

	IEnumerator TimerEndLevel()
    {
		SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.EndTimer, UI_Manager.instance.UI_audioSource);
		yield return new WaitForSeconds(1);
		if(!levelEnded)
			StartCoroutine(TimerEndLevel());
    }

	public void LosePoints(int amount)
    {
		secondarySystemsExpired++;
		currentScore -= amount;
		if (currentScore < 0)
			currentScore = 0;
		UI_Manager.instance.UI_scoreValue.text = currentScore.ToString();
		CameraManager.instance.ShakeScreen();
	}

	public void WinPoints(int amount)
    {
		secondarySystemsFilled++;
		currentScore += amount;
		UI_Manager.instance.UI_scoreValue.text = currentScore.ToString();
	}
}
