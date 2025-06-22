using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{

	#region Variables
	public GameObject timerCanvas;
	public TextMeshProUGUI timer;
	public TextMeshProUGUI personalBest;

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
		seconds = 0f;
		ShowPersonalBest();
		timerCanvas.SetActive(true);
	}

	public void StopTimer()
	{
		isRunning = false;
		SavePersonalBest();
		timerCanvas.SetActive(false);
	}

	 /* ---------- NUEVO ---------- */
    /// <summary>Pausa sin resetear el contador.</summary>
    public void PauseTimer()   => isRunning = false;

    /// <summary>Reanuda desde el tiempo acumulado.</summary>
    public void ResumeTimer()  => isRunning = true;
    /* --------------------------- */

	private void ShowPersonalBest()
	{
		float pb = GetPersonalBest();
		personalBest.SetText(pb == 0 ? "--:--:---" : FormatTimeMilliseconds(pb));
	}

	public string GetTimeString()
	{
		return FormatTimeMilliseconds(seconds);
	}

	public bool IsNewRecord()
	{
		float pb = GetPersonalBest();
		return pb == 0f || seconds < pb;
	}

	public string GetRecordString()
	{
		float pb = GetPersonalBest();
		return pb == 0f ? "--:--:---" : FormatTimeMilliseconds(pb);
	}

	#endregion
}
