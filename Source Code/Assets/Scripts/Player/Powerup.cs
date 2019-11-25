using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
	public enum effects { SLOWMO, FASTER_MOVEMENT, HEALTH_REGEN, UNLIMITED_DASH, SHIELD };

	[Header("Pickup")]
	public effects effect;
	public float effectDuration;

	[Header("Settings")]
	public float speedMultiplier = 1.5f;
	public int healthGainAmount = 50;

	[Space]
	public GameObject particles;

	private CharacterController player;
	private float playerDashWait;

	private void Start() {
		player = FindObjectOfType<CharacterController>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			player = collision.GetComponent<CharacterController>();
			PickUp();
		}
	}

	void PickUp() {
		Instantiate(particles, transform.position, transform.rotation);

		switch (effect) {
			case effects.SLOWMO:
				Time.timeScale = 0.5f;
				AudioManager.instance.SetPitch("Music", 0.8f);
				break;
			case effects.FASTER_MOVEMENT:
				player.speed *= speedMultiplier;
				break;
			case effects.HEALTH_REGEN:
				player.health += healthGainAmount;
				break;
			case effects.UNLIMITED_DASH:
				playerDashWait = player.dashWait;
				player.dashWait = 0;
				break;
			case effects.SHIELD:
				player.ActivateShield();
				break;
			default:
				break;
		}

		GetComponent<SpriteRenderer>().enabled = false;
		Destroy(gameObject, effectDuration);
	}

	private void OnDestroy() {
		if (!player)
			return;

		if (!UI.gameIsPaused && effect == effects.SLOWMO) {
			Time.timeScale = 1f;
			AudioManager.instance.SetPitch("Music", 1f);
		} else if (!UI.gameIsPaused && effect == effects.FASTER_MOVEMENT) {
			player.speed /= speedMultiplier;
		} else if (!UI.gameIsPaused && effect == effects.UNLIMITED_DASH) {
			player.dashWait = playerDashWait;
		}
	}
}
