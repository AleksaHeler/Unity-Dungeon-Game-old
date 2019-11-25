using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour {

	public new string name;
	public int damage = 10;
	public bool friendly = true;
	public bool automatic = false;
	public int clipSize = 10;
	public int fireRate = 180; // Shots per minute

	public Projectile projectile;
	public Transform firePoint;

	int clip;
	float fireCountdown;

	private void Start() {
		if (fireRate <= 1)
			fireRate = 1;
		if (clipSize <= 1)
			clipSize = 1;

		clip = clipSize;
		fireCountdown = 60.0f / fireRate;
	}

	void Update()
    {
		// Cooldown for firing
		if(fireCountdown > 0)
			fireCountdown -= Time.deltaTime;

		// Reload
		if (Input.GetKeyDown(KeyCode.R))
			clip = clipSize;
	}

	public void Shoot() {
		if (fireCountdown <= 0 && clip > 0) {
			Projectile p = Instantiate(projectile.gameObject, firePoint.position, transform.localRotation).GetComponent<Projectile>();
			p.damage = damage;
			p.friendly = friendly;
			clip--;
			fireCountdown = 60.0f / fireRate;
		}
	}
}
