using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour {

	public ParticleSystem barrelParticles;
	public GameObject coinPrefab;
	public GameObject[] powerupPrefabs;
	public float coinOrPowerup = 0.6f;
	
	private Vector3 offset = new Vector3(0.5f, 0.5f, 0);

	public void OnDestroy() {
		// If we can find audio manager script, then play a sound
		if (FindObjectOfType<AudioManager>())
			FindObjectOfType<AudioManager>().Play("WoodHit");
		// If we can find palyer script, then add score
		if (FindObjectOfType<CharacterController>())
			FindObjectOfType<CharacterController>().score += 10;

		// Drop a coin or a prefab (if rand < coinOrPowerup -> coin : powerup);
		if(Random.value < coinOrPowerup) {
			Instantiate(coinPrefab, transform.position + offset, Quaternion.identity);
		} else {
			Instantiate(powerupPrefabs[(int)(Random.value*powerupPrefabs.Length)], transform.position + offset, Quaternion.identity);
		}

		Destroy(Instantiate(barrelParticles, transform.position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity).gameObject, 0.2f);
	}
}
