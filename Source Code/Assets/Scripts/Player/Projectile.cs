using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Projectile : MonoBehaviour {

	public float speed;
	public int damage;

	public ParticleSystem deathParticles;
	[HideInInspector] public bool friendly = false;

    void Update() {
		transform.position += transform.right * speed * Time.deltaTime;
    }

	private void OnCollisionEnter2D(Collision2D collision) {
		// When we hit something instantiate particles, see what we hit, destroy bullet
		Instantiate(deathParticles, transform.position, transform.localRotation);

		if (collision.gameObject.tag == "Player" && !friendly)
			collision.gameObject.GetComponent<CharacterController>().TakeDamage(damage);
		if (collision.gameObject.tag == "Enemy" && friendly)
			collision.gameObject.GetComponent<Enemy>().Damage(damage);

		if (collision.gameObject.tag == "Barrel")
			Destroy(collision.gameObject);
		
		Destroy(gameObject);
	}
}
