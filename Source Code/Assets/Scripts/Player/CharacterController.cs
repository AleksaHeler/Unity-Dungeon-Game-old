using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CharacterController : MonoBehaviour {

	public static CharacterController instance;

	[Header("Parameters")]
	public float speed;
	[Range(0, 1)] public float lookSpeed;
	[Range(0, 2)] public float dashSpeed;
	public float dashWait;
	public float swordWait;
	public int swordDamage = 30;

	[Header("GameObjects")]
	public GameObject sprite;
	public GameObject sword;
	public GameObject weapon;
	public GameObject shield;
	public GameObject minimapSprite;
	public ParticleSystem dashParticles;

	[Header("Stats")]
	public int coins = 0;
	public int score = 0;
	public int health = 100;
	public int level = 0;

	Animator animator;
	Rigidbody2D rb;

	Vector3 prevPosition;
	Vector3 lookDirection;

	float swordRange = 1;
	float dashCountdown = 0;
	float swordCountdown = 0;
	bool oneTimeDash = false;
	bool dashed = false;

	void Awake() {
		// Get basic components
		rb = GetComponent<Rigidbody2D>();
		animator = sprite.GetComponent<Animator>();
		minimapSprite.SetActive(true);


		// If this is the first instance of player keep it
		if (instance == null)
			instance = this;
		else // If there is already player in the scene
			Destroy(gameObject);
			return;
	}

	void FixedUpdate() {
		if (UI.gameIsPaused)
			return;


		// Input
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");


		// Movement
		Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0).normalized;
		rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);


		// If we have a weapon, aim it and if we pressed the button shoot it
		lookDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
		if (weapon) {
			// Calculate the angle where we are looking and rotate the weapon towards it
			float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
			weapon.transform.rotation = Quaternion.identity;
			weapon.transform.Rotate(transform.forward, angle);

			// Flip weapon sprite
			if (angle < 90 && angle > -90) { weapon.GetComponentInChildren<SpriteRenderer>().flipY = false; } else { weapon.GetComponentInChildren<SpriteRenderer>().flipY = true; }

			// Shoot semi-automatic weapons
			if (Input.GetButtonDown("Fire1") && !weapon.GetComponent<Weapon>().automatic) {
				weapon.GetComponent<Weapon>().Shoot();
			}
			// Shoot automatic weapons
			if (Input.GetButton("Fire1") && weapon.GetComponent<Weapon>().automatic) {
				weapon.GetComponent<Weapon>().Shoot();
			}
		}


		// Swing the sword
		SwingSword();


		// Dash
		Vector3 currentDirection = (transform.position - prevPosition) / Time.fixedDeltaTime;
		Dash(currentDirection);


		// Animation
		if (currentDirection != Vector3.zero)
			animator.SetFloat("Speed", currentDirection.magnitude);
		else
			animator.SetFloat("Speed", 0);


		// Walk sound
		if (currentDirection != Vector3.zero)
			FindObjectOfType<AudioManager>().Play("PlayerStep");


		// Flip
		if (movement.x > 0) {
			sprite.transform.localScale = new Vector3(1, 1, 1);
		} else if (movement.x < 0) {
			sprite.transform.localScale = new Vector3(-1, 1, 1);
		}


		// Counters
		if (dashCountdown > 0) dashCountdown -= Time.fixedDeltaTime;
		if (swordCountdown > 0) swordCountdown -= Time.fixedDeltaTime;
		if (dashWait - dashCountdown > 0.2f) dashed = false;
	}

	private void Dash(Vector3 currentDirection) {
		// Spawn dash particles at the right angle
		if (oneTimeDash) {
			float a = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg + 90;
			Destroy(Instantiate(dashParticles, transform.position + Vector3.back * 3, Quaternion.Euler(0, 0, a)).gameObject, 5f);
			oneTimeDash = false;
		}
		// If all dash conditions are met (press jump, be moving, dash countown)
		if (Input.GetButtonDown("Jump") && currentDirection != Vector3.zero && dashCountdown <= 0) {
			dashed = true; dashCountdown = dashWait; oneTimeDash = true;
			rb.MovePosition(transform.position + currentDirection.normalized * dashSpeed);
			StartCoroutine(Camera.main.GetComponent<CameraFollow>().Shake(0.1f, .05f));
			FindObjectOfType<AudioManager>().Play("PlayerDash");
		}
		prevPosition = transform.position;
	}


	private void SwingSword() {
		// Animate
		if (Input.GetButtonDown("Fire2") && swordCountdown <= 0) {
			if (lookDirection.x > 0) {
				sword.GetComponent<Animator>().Play("Sword_SwingRight");
			} else {
				sword.GetComponent<Animator>().Play("Sword_SwingLeft");
			}
			FindObjectOfType<AudioManager>().Play("SwordSwing");
			weapon.SetActive(false);
			swordCountdown = swordWait;
		}

		// If it is still animating (still swinging) see what we hit
		AnimatorStateInfo animatorInfo = sword.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
		if (animatorInfo.IsName("Sword_SwingLeft") || animatorInfo.IsName("Sword_SwingRight")) {
			weapon.SetActive(false);
			// Get all colliders, and switch case them
			Collider2D[] colliders = Physics2D.OverlapCircleAll(sword.transform.position, swordRange / 2f);
			foreach (Collider2D c in colliders) {
				if (c.tag == "Barrel") Destroy(c.gameObject);
				if (c.tag == "Cell") FindObjectOfType<AudioManager>().Play("StoneHit");
				if (c.tag == "Enemy") Debug.Log("You hit enemy with the sword!");
			}
		} else { weapon.SetActive(true); }
	}

	public void ActivateShield() {
		shield.SetActive(true);
	}


	public void TakeDamage(int damage) {
		health -= damage;
		if (health <= 0) Die();
	}


	private void Die() {
		FindObjectOfType<AudioManager>().Play("StoneHit");
		FindObjectOfType<UI>().GameOver();
	}


	private void OnCollisionEnter2D(Collision2D collision) {
		if (dashed) {
			//Here  we collided while dashing
			if (collision.gameObject.tag == "Barrel") {
				Destroy(collision.gameObject);
			}
			//swordDamage
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Coin") {
			coins++;
			Destroy(collision.gameObject);
		}
	}
}