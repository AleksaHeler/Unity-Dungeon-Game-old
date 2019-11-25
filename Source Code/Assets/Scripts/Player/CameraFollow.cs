using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	Transform target;
	Vector3 prevPosition;

	public float smoothSpeed = 0.125f;
	public float zoomAmount = 0.5f;
	public float zoomSmoothness = 0.04f;
	Vector3 offset = new Vector3(0, 0, -50);
	float prevZoom = 0;
	bool shaking = false;

	void Start() {
		target = GameObject.FindGameObjectWithTag("Player").transform;
		this.transform.position = target.transform.position + offset;
	}

    void FixedUpdate()
    {
		if (!target)
			target = GameObject.FindGameObjectWithTag("Player").transform;

		// Smooth follow the player
		Vector3 desiredPosition = target.position + offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;

		// Zoom in/out depending on speed
		if (!shaking) {
			Vector3 currentDirection = (transform.position - prevPosition) / Time.deltaTime;
			Camera.main.orthographicSize = 5 + Mathf.Lerp(prevZoom, Mathf.Clamp(currentDirection.magnitude / 10, 0, 1) * zoomAmount, zoomSmoothness);
			prevZoom = Mathf.Clamp(currentDirection.magnitude / 10, 0, 1) * zoomAmount;
			prevPosition = transform.position;
		}
	}

	/// <summary>
	/// StartCoroutine(Camera.main.GetComponent<CameraFollow>().Shake(0.1f, .05f));
	/// </summary>
	public IEnumerator Shake(float duration, float magnitude) {
		Vector3 originalPos = transform.position;
		shaking = true;
		float elapsed = 0;
		while(elapsed < duration) {
			float x = Random.Range(-1f, 1f) * magnitude;
			float y = Random.Range(-1f, 1f) * magnitude;
			transform.localPosition = originalPos + new Vector3(x, y, originalPos.z);
			elapsed += Time.deltaTime;
			yield return null;
		}
		shaking = false;
		transform.localPosition = originalPos;
	}
}
