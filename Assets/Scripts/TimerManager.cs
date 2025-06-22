using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{

	#region Variables
	public TextMeshProUGUI timer;

	private float seconds;
	private bool isRunning;
	#endregion

	#region Unity Methods    

	// Start is called before the first frame update
	void Start()
    {
		seconds = 0f;
		isRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
		{
			seconds += Time.deltaTime;
			timer.text = FormatTimeMilliseconds(seconds);
		}
	}

	public void SavePersonalBest()
	{
		float pb = GetPersonalBest();
		if (pb == 0f || seconds < pb)
			PlayerPrefs.SetFloat("PersonalBest", seconds);
	}
	
	public float GetPersonalBest()
	{
		return PlayerPrefs.GetFloat("PersonalBest", 0f);
	}

	private string FormatTimeMilliseconds(float timeInSeconds)
	{
		int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
		int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
		int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000f) % 1000f);
		return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
	}

	public void StartTimer()
	{
		isRunning = true;
	}

	public void StopTimer()
	{
		isRunning = false;
	}

	#endregion
}
