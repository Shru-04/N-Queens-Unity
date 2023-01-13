using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Meniu : MonoBehaviour
{

	int nrRegine;
	public InputField input;

	// Use this for initialization
	void Start()
	{
		nrRegine = 4;
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{

	}

	public void Play()
	{
		if (input.text != "")
			nrRegine = int.Parse(input.text);

		PlayerPrefs.SetInt("Regine", nrRegine);

		StartCoroutine(PlayGame());
	}

	IEnumerator PlayGame()
	{
		yield return new WaitForSeconds(.5f);
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
	}
	public void Exit()
	{
		Application.Quit();
	}
}