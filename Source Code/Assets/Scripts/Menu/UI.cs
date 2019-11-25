using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour {

	public static bool gameIsPaused = false;

	[Header("Menu objects")]
	public GameObject pauseMenuUI;
	public GameObject endMenuUI;
	public GameObject minimap;
	public GameObject UIGameObject;

	[Header("Stats objects")]
	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI coinsText;
	public Slider healthSlider;
	public TextMeshProUGUI quote;

	void Update() {

		// Pause menu
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (gameIsPaused) Resume();
			else Pause();
		}
		if (Input.GetKeyDown(KeyCode.Q)) {
			GameOver();
		}

		// If paused disable UI object
		if (gameIsPaused) {
			UIGameObject.SetActive(false);
			return;
		} else {
			UIGameObject.SetActive(true);

			CharacterController player = FindObjectOfType<CharacterController>();
			scoreText.text = "Score: " + player.score;
			coinsText.text = "Coins: " + player.coins;
			healthSlider.value = player.health;
			// health level
		}
	}
	public void LoadMenu() {
		Time.timeScale = 1f;
		gameIsPaused = false;
		SceneManager.LoadScene(0);
	}

	public void Resume() {
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		gameIsPaused = false;
		FindObjectOfType<AudioManager>().SetPitch("Music", 1f);
	}

	public void Pause() {
		pauseMenuUI.SetActive(true);
		Time.timeScale = 0f;
		gameIsPaused = true;
		FindObjectOfType<AudioManager>().SetPitch("Music", 0.8f);
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void GameOver() {
		// Enable/disable gameObjects
		endMenuUI.SetActive(true);
		minimap.SetActive(false);

		// Slow time
		Time.timeScale = 0.1f;
		gameIsPaused = true;

		// Play music and display demotivational quote
		FindObjectOfType<AudioManager>().SetPitch("Music", 0.6f);
		quote.text = Quotes.GetRandom();
	}
}
