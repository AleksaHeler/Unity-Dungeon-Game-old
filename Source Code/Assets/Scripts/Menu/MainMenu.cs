using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine;

public class MainMenu : MonoBehaviour {

	public AudioMixer audioMixer;

	private void Start() {
		FindObjectOfType<AudioManager>().SetPitch("Music", 0.9f);
	}

	public void Play() {
		FindObjectOfType<AudioManager>().SetPitch("Music", 1f);
		SceneManager.LoadScene(1);
	}
	public void Quit() {
		Application.Quit();
	}
	public void SetVolume(float volume) {
		audioMixer.SetFloat("volume", volume);
	}
}
